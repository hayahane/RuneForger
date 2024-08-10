using System;
using KinematicCharacterController;
using UnityEngine;
using UnityHFSM;
using Cinemachine.Utility;
using RuneForger.Attack;
using RuneForger.Gameplay;
using RuneForger.SoundFX;
using UnityEngine.VFX;
using RuneForger.Interact;

namespace RuneForger.Character
{
    public class GameCharacter : MonoBehaviour, ICharacterController
    {
        [field: SerializeField, Header("Movement")]
        public float HSpeed { get; set; } = 4.5f;
        [field: SerializeField]
        public float JumpHeight { get; set; } = 1.5f;
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

        [SerializeField, Header("Sound")]
        private RandomSFX _sfx;
        [field: SerializeField]
        public RandomSFX Voice{get; private set;}

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
        public AttackVolume AttackVolume { get; private set; }
        [field: SerializeField]
        public TrailRenderer Trail { get; private set; }

        #region MonoBehaviour Callbacks
        private void Start()
        {
            // Get the KinematicCharacterMotor component
            Motor = GetComponent<KinematicCharacterMotor>();
            // Assign this GameCharacter to the motor's CharacterController property
            Motor.CharacterController = this;
            _animator = GetComponentInChildren<Animator>();
            RMHelper = GetComponentInChildren<RootMotionHelper>();

            AttackVolume = GetComponent<AttackVolume>();

            InitFsm();
            _fsm.Init();
        }

        private void InitFsm()
        {
            // Locomotion Layer
            var locomotionFsm = new CharacterFSM();
            locomotionFsm.AddState("Idle", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) => Vector3.zero,
                onEnter: state =>
                {
                    _animator.SetBool("IsOnGround", true);
                    _animator.SetFloat("Speed", 0);
                }
            ));
            locomotionFsm.AddState("Move", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) => HVelocity * HSpeed,
                onUpdateRotation: (currentRotation, deltaTime) =>
                {
                    var rot = Quaternion.Slerp(Animator.transform.rotation, Quaternion.LookRotation(HVelocity.normalized), 10 * deltaTime);
                    Animator.transform.rotation = rot;
                    return currentRotation;
                },
                onEnter: state =>
                {
                    _sfx.enabled = true;
                    _animator.SetBool("IsOnGround", true);
                    _animator.SetFloat("Speed", 1);
                },
                onExit: state =>
                {
                    _sfx.enabled = false;
                }
            ));
            locomotionFsm.AddTransition(new Transition("Idle", "Move",
                condition: transition => { return HVelocity.magnitude > 0.1f; }
            ));
            locomotionFsm.AddTransition(new Transition("Move", "Idle",
                condition: transition => HVelocity.magnitude <= 0.1f
            ));
            _fsm.AddState("Locomotion", locomotionFsm);
            locomotionFsm.SetStartState("Idle");

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
            _fsm.AddState("Jump", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) =>
                {
                    Debug.Log($"Must Unground: {Motor.MustUnground()}");
                    Motor.ForceUnground();
                    VVelocity += Physics.gravity * deltaTime;
                    return VVelocity + currentVelocity.ProjectOntoPlane(Motor.CharacterUp);
                },
                onEnter: State =>
                {
                    Voice.PlayRandomly();
                    _animator.SetTrigger("Jump");
                    VVelocity = Vector3.up * Mathf.Sqrt(2 * JumpHeight * 9.81f);
                    Motor.ForceUnground(VVelocity.magnitude / 9.81f);
                    Debug.Log($"Init Velocity: {VVelocity}");
                },
                onExit: State =>
                {
                    IsJumping = false;
                }
            ));
            _fsm.AddTransition(new Transition("Locomotion", "Jump",
                condition: transition => IsJumping
            ));
            _fsm.AddTransition(new Transition("Jump", "Fall",
                condition: transition => VVelocity.y <= 0
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
            //Debug.Log(_fsm.ActiveStateName);
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