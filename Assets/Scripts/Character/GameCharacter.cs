using KinematicCharacterController;
using UnityEngine;
using UnityHFSM;
using Cinemachine.Utility;
using System;
using UnityEngine.PlayerLoop;

namespace RuneForger.Character
{
    public class GameCharacter : MonoBehaviour, ICharacterController
    {
        [field: SerializeField, Header("Movement")]
        public float HSpeed { get; set; } = 4.5f;
        [field: SerializeField]
        public float JumpHeight { get; set; } = 1.5f;
        [field: SerializeField]
        public float JumpTime { get; set; } = 1f;
        [field: SerializeField]
        public float JumpHeightExtra { get; set; } = 0.5f;
        [SerializeField, Header("Combo")]
        private ComboAsset _comboAsset;
        public ComboAsset ComboAsset
        {
            get => _comboAsset;
            set
            {
                _comboAsset = value;
                if (_fsm != null)
                {
                    var attackState = _fsm.GetState("Attack") as AttackState;
                    attackState.ComboAsset = value;
                }
            }
        }

        public bool IsRotationLocked { get; set; }
        public Vector3 HVelocity { get; set; }
        public Vector3 VVelocity { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAttacking { get; set; }

        public KinematicCharacterMotor Motor { get; private set; }
        private readonly CharacterFSM _fsm = new();
        private Animator _animator;
        public Animator Animator => _animator;
        public RootMotionHelper RMHelper { get; private set; }

        #region MonoBehaviour Callbacks
        private void Start()
        {
            // Get the KinematicCharacterMotor component
            Motor = GetComponent<KinematicCharacterMotor>();
            // Assign this GameCharacter to the motor's CharacterController property
            Motor.CharacterController = this;
            _animator = GetComponentInChildren<Animator>();
            RMHelper = GetComponentInChildren<RootMotionHelper>();

            InitFSM();
            _fsm.Init();
        }

        private void InitFSM()
        {
            // Locomotion Layer
            var locomotionFSM = new CharacterFSM();
            locomotionFSM.AddState("Idle", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) =>
                {
                    return Vector3.zero;
                },
                onEnter: State =>
                {
                    _animator.SetBool("IsOnGround", true);
                    _animator.SetFloat("Speed", 0);
                }
            ));
            locomotionFSM.AddState("Move", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) =>
                {
                    return HVelocity * HSpeed;
                },
                onUpdateRotation: (currentRotation, deltaTime) =>
                {
                    var rot = Quaternion.Slerp(Animator.transform.rotation, Quaternion.LookRotation(HVelocity.normalized), 10 * deltaTime);
                    Animator.transform.rotation = rot;
                    return currentRotation;
                },
                onEnter: State =>
                {
                    _animator.SetBool("IsOnGround", true);
                    _animator.SetFloat("Speed", 1);
                }
            ));
            locomotionFSM.AddTransition(new Transition("Idle", "Move",
                condition: transition => { return HVelocity.magnitude > 0.1f; }
            ));
            locomotionFSM.AddTransition(new Transition("Move", "Idle",
                condition: transition => HVelocity.magnitude <= 0.1f
            ));
            _fsm.AddState("Locomotion", locomotionFSM);

            // Fall & Jump States
            _fsm.AddState("Fall", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) =>
                {
                    return currentVelocity + Vector3.down * 9.81f * deltaTime;
                },
                onEnter: State =>
                {
                    _animator.SetBool("IsOnGround", false);
                }
            ));
            _fsm.AddTransition(new Transition("Locomotion", "Fall",
                condition: transition => !Motor.GroundingStatus.IsStableOnGround
            ));
            _fsm.AddTransition(new Transition("Fall", "Locomotion",
                condition: transition => Motor.GroundingStatus.IsStableOnGround
            ));

            // Attack State
            _fsm.AddState("Attack", new AttackState(this) { ComboAsset = ComboAsset });
            _fsm.AddTransition(new Transition("Locomotion", "Attack",
                condition: transition => IsAttacking
            ));
            _fsm.AddTransition(new Transition("Attack", "Locomotion"));

            _fsm.SetStartState("Locomotion");
        }

        private void Update()
        {
            _fsm.OnLogic();
            Debug.Log(_fsm.ActiveStateName);
        }

        #endregion

        #region KCC Implementation
        public void AfterCharacterUpdate(float deltaTime)
        {
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            _fsm.UpdateRotation(ref currentRotation, deltaTime);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            _fsm.UpdateVelocity(ref currentVelocity, deltaTime);
        }
        #endregion
    }
}