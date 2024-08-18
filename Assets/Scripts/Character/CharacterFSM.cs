using UnityEngine;
using UnityHFSM;
using Cinemachine.Utility;
using KinematicCharacterController;
using System;
using RuneForger.Attack;
using RuneForger.Player;
using UniVRM10;

namespace RuneForger.Character
{
    public interface IKcc
    {
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime);
    }

    public class CharacterState : State, IKcc
    {
        private readonly Func<Vector3, float, Vector3> _onUpdateVelocity;
        private readonly Func<Quaternion, float, Quaternion> _onUpdateRotation;


        public CharacterState(
            Func<Vector3, float, Vector3> onUpdateVelocity = null,
            Func<Quaternion, float, Quaternion> onUpdateRotation = null,
            Action<State<string, string>> onEnter = null,
            Action<State<string, string>> onLogic = null,
            Action<State<string, string>> onExit = null,
            Func<State<string, string>, bool> canExit = null,
            bool needsExitTime = false,
            bool isGhostState = false)
            : base(
                onEnter,
                onLogic,
                onExit,
                canExit,
                needsExitTime: needsExitTime,
                isGhostState: isGhostState)
        {
            _onUpdateVelocity = onUpdateVelocity;
            _onUpdateRotation = onUpdateRotation;
        }
        
        void IKcc.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_onUpdateRotation != null)
            {
                currentRotation = _onUpdateRotation(currentRotation, deltaTime);
            }
        }

        void IKcc.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_onUpdateVelocity != null)
            {
                currentVelocity = _onUpdateVelocity(currentVelocity, deltaTime);
            }
        }
    }

    public class CharacterStateBase : StateBase
    {
        protected readonly GameCharacter _character;
        public CharacterStateBase(GameCharacter character, bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
        {
            _character = character;
        }
    }
    public class CharacterFSM : StateMachine, IKcc
    {
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (ActiveState is IKcc state)
            {
                state.UpdateVelocity(ref currentVelocity, deltaTime);
            }
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (ActiveState is IKcc state)
            {
                state.UpdateRotation(ref currentRotation, deltaTime);
            }
        }
    }

    public class AttackState : CharacterStateBase, IKcc
    {
        private int _attackStage = -1;

        public ComboAsset ComboAsset { get; set; }

        private float _attackTimer = 0;
        private ComboInfo CurrentAttackInfo => ComboAsset.ComboInfos[_attackStage];

        private bool _attackInputCache = false;
        private bool _detectionTriggerd = false;
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

        public AttackState(GameCharacter character, bool isGhostState = false) : base(character, true, isGhostState)
        {
        }

        public override void OnEnter()
        {
            //Debug.Log("AttackState OnEnter");
            _character.Animator.SetTrigger("EnterAttack");
            _attackStage = 0;
            _attackTimer = 0;

            _character.RmHelper.IsRootMotionApplied = true;

            _character.Voice.PlayRandomly();
        }

        public override void OnExit()
        {
            _character.RmHelper.IsRootMotionApplied = false;
            _character.Animator.SetBool(IsAttacking, false);
            _detectionTriggerd = false;
        }

        public override void OnLogic()
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= CurrentAttackInfo.TransTime)
            {
                if (_character.IsAttacking) _attackInputCache = true;
                _character.Animator.SetBool(IsAttacking, false);
            }
            if (_attackTimer >= CurrentAttackInfo.AttackBeginTime && !_detectionTriggerd)
            {
                _detectionTriggerd = true;
                _character.AttackVolume.VolumeOrigin = _character.Animator.transform.forward * 1.2f +
                        _character.AttackVolume.VolumeOrigin.y * Vector3.up;
                var attackInfo = new AttackInfo
                {
                    Source = _character,
                    Damage = 10
                };
                _character.AttackVolume.BeginDetect(attackInfo, CurrentAttackInfo.AttackEndTime - CurrentAttackInfo.AttackBeginTime);

                // Trigger VFX
                _character.Trail.enabled = true;
            }
            if (_attackTimer >= CurrentAttackInfo.ExitTime)
            {
                _character.Trail.enabled = false;
                if (_attackInputCache)
                {
                    ChangeAttackStage();
                }
                else
                {
                    fsm.StateCanExit();
                }
            }
        }

        private void ChangeAttackStage()
        {
            _character.Animator.SetBool("IsAttacking", true);
            _attackStage++;
            if (_attackStage >= ComboAsset.ComboCount)
                _attackStage -= ComboAsset.ComboCount;
            _attackTimer = 0;
            _attackInputCache = false;
            _character.Voice.PlayRandomly();
            _detectionTriggerd = false;
        }

        void IKcc.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            currentVelocity = _character.RmHelper.StoredRootMotion.ProjectOntoPlane(_character.Motor.CharacterUp) / deltaTime;
        }

        void IKcc.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }
    }

    public class GravityChangeState : CharacterStateBase, IKcc
    {
        private const float ChangeTime = 0.2f;
        private float _changeTimeTimer;
        private Vector3 _rotatePoint;
        public Vector3 GravityTargetDir { get; set; }
        public GravityChangeState(GameCharacter character, bool needsExitTime, bool isGhostState = false) : base(character, needsExitTime, isGhostState)
        {
        }

        public override void OnEnter()
        {
            var cameraTarget = _character.GetComponentInChildren<CameraTarget>();
            cameraTarget.ResetPitch();
            cameraTarget.IsPitchLocked = true;
            cameraTarget.transform.localPosition = Vector3.zero;
            
            //Debug.Log("Enter Gravity Change State");
            _character.transform.position += _character.Motor.CharacterUp * 0.5f;
            _rotatePoint = _character.transform.position + 0.5f * _character.Motor.Capsule.height * _character.Motor.CharacterUp;
            _character.Animator.SetBool("IsOnGround", false);
            _changeTimeTimer = ChangeTime;
            _character.Motor.ForceUnground(ChangeTime);
            _character.Motor.enabled = false;
        }

        public override void OnExit()
        {
            var cameraTarget = _character.GetComponentInChildren<CameraTarget>();
            cameraTarget.IsPitchLocked = false;
            cameraTarget.transform.localPosition = new Vector3(0, 1.25f, 0);
            
            _character.Motor.enabled = true;
            _character.Motor.SetRotation(Quaternion.LookRotation(_character.transform.forward, -GravityTargetDir));
            _character.Motor.SetPosition(_rotatePoint + _character.Motor.Capsule.height * GravityTargetDir);
        }

        public override void OnLogic()
        {
            _changeTimeTimer -= Time.deltaTime;
            _character.transform.RotateAround(_rotatePoint, _character.transform.forward, 180 * (Time.deltaTime / ChangeTime));
            if (_changeTimeTimer <= 0)
            {
                fsm.StateCanExit();
            }
        }

        void IKcc.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
        }

        void IKcc.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }
    }
}