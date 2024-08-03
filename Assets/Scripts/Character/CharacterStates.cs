using UnityEngine;
using UnityHFSM;
using Cinemachine.Utility;
using KinematicCharacterController;
using System;

namespace RuneForger.Character
{

    public interface IKCC
    {
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime);
    }

    public class CharacterState : State, IKCC
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
        
        void IKCC.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_onUpdateRotation != null)
            {
                currentRotation = _onUpdateRotation(currentRotation, deltaTime);
            }
        }

        void IKCC.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
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
    public class CharacterFSM : StateMachine, IKCC
    {
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (ActiveState is IKCC state)
            {
                state.UpdateVelocity(ref currentVelocity, deltaTime);
            }
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (ActiveState is IKCC state)
            {
                state.UpdateRotation(ref currentRotation, deltaTime);
            }
        }
    }

    public class AttackState : CharacterStateBase, IKCC
    {
        private int _attackStage = -1;

        public ComboAsset ComboAsset { get; set; }

        private float _attackTimer = 0;
        private ComboInfo CurrentAttackInfo => ComboAsset.ComboInfos[_attackStage];

        private bool _attackInputCache = false;

        public AttackState(GameCharacter character, bool isGhostState = false) : base(character, true, isGhostState)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("AttackState OnEnter");
            _character.Animator.SetTrigger("EnterAttack");
            _attackStage = 0;
            _attackTimer = 0;

            _character.RMHelper.IsRootMotionApplied = true;
        }

        public override void OnExit()
        {
            _character.RMHelper.IsRootMotionApplied = false;
            _character.Animator.SetBool("IsAttacking", false);
        }

        public override void OnLogic()
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer >= CurrentAttackInfo.TransTime)
            {
                if (_character.IsAttacking) _attackInputCache = true;
                _character.Animator.SetBool("IsAttacking", false);
            }
            if (_attackTimer >= CurrentAttackInfo.ExitTime)
            {
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
        }

        void IKCC.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            currentVelocity = _character.RMHelper.StoredRootMotion.ProjectOntoPlane(_character.Motor.CharacterUp) / deltaTime;
        }

        void IKCC.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
        }
    }
}