using KinematicCharacterController;
using UnityEngine;
using UnityHFSM;
using Cinemachine.Utility;
using RuneForger.Battle;
using RuneForger.GravityField;
using RuneForger.SoundFX;
using UnityEngine.Serialization;

namespace RuneForger.Character
{
    public class GameCharacter : MonoBehaviour, ICharacterController, IGravity
    {
        #region Movement

        [field: SerializeField, Header("Movement")]
        public float HSpeed { get; set; } = 4.5f;

        [field: SerializeField] public float JumpHeight { get; set; } = 1.5f;

        #endregion

        #region Combat

        [FormerlySerializedAs("_comboAsset")] [SerializeField, Header("Combo")]
        private ComboAsset comboAsset;

        public ComboAsset ComboAsset
        {
            get => comboAsset;
            set
            {
                comboAsset = value;
                if (_fsm != null)
                {
                    var attackState = _fsm.GetState("Attack") as AttackState;
                    attackState!.ComboAsset = value;
                }
            }
        }

        #endregion

        #region Sound&Visual Effects

        [FormerlySerializedAs("_sfx")] [SerializeField, Header("Sound")]
        private RandomSFX sfx;

        [field: SerializeField] public RandomSFX Voice { get; private set; }

        #endregion

        
        public Vector3 HVelocity { get; set; }
        public Vector3 VVelocity { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAttacking { get; set; }

        public KinematicCharacterMotor Motor { get; private set; }
        private readonly CharacterFSM _fsm = new();
        private GravityChangeState _gravityChangeState;

        #region Animation Control

        private static readonly int IsOnGround = Animator.StringToHash("IsOnGround");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Speed = Animator.StringToHash("Speed");
        public Animator Animator { get; private set; }
        public RootMotionHelper RmHelper { get; private set; }

        #endregion

        public AttackVolume AttackVolume { get; private set; }
        [field: SerializeField] public TrailRenderer Trail { get; private set; }

        private Vector3 Gravity { get; set; } = Vector3.down * 9.81f;

        #region MonoBehaviour Callbacks

        private void Start()
        {
            // Get the KinematicCharacterMotor component
            Motor = GetComponent<KinematicCharacterMotor>();
            // Assign this GameCharacter to the motor's CharacterController property
            Motor.CharacterController = this;
            Animator = GetComponentInChildren<Animator>();
            RmHelper = GetComponentInChildren<RootMotionHelper>();

            AttackVolume = GetComponent<AttackVolume>();

            InitFsm();
            _fsm.Init();
        }

        private void InitFsm()
        {
            // Locomotion Layer
            var locomotionFsm = new CharacterFSM();
            locomotionFsm.AddState("Idle", new CharacterState(
                onUpdateVelocity: (_, _) => Vector3.zero,
                onEnter: _ =>
                {
                    Animator.SetBool(IsOnGround, true);
                    Animator.SetFloat(Speed, 0);
                }
            ));
            locomotionFsm.AddState("Move", new CharacterState(
                onUpdateVelocity: (_, _) => HVelocity * HSpeed,
                onUpdateRotation: (_, deltaTime) =>
                {
                    var rot = Quaternion.Slerp(Animator.transform.rotation,
                        Quaternion.LookRotation(HVelocity.normalized, Motor.CharacterUp), 10 * deltaTime);
                    Animator.transform.rotation = rot;
                    return transform.rotation;
                },
                onEnter: _ =>
                {
                    sfx.enabled = true;
                    Animator.SetBool(IsOnGround, true);
                    Animator.SetFloat(Speed, 1);
                },
                onExit: _ => { sfx.enabled = false; }
            ));
            locomotionFsm.AddTransition(new Transition("Idle", "Move",
                condition: _ => HVelocity.magnitude > 0.1f));
            locomotionFsm.AddTransition(new Transition("Move", "Idle",
                condition: _ => HVelocity.magnitude <= 0.1f
            ));
            _fsm.AddState("Locomotion", locomotionFsm);
            locomotionFsm.SetStartState("Idle");

            // Fall & Jump States
            _fsm.AddState("Fall", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) =>
                {
                    var velocity = Vector3.Project(currentVelocity, Vector3.up);
                    return velocity + HVelocity + Gravity * deltaTime;
                },
                onEnter: _ => { Animator.SetBool(IsOnGround, false); }
            ));
            _fsm.AddTransition(new Transition("Locomotion", "Fall",
                condition: _ => !Motor.GroundingStatus.IsStableOnGround
            ));
            _fsm.AddTransition(new Transition("Fall", "Locomotion",
                condition: _ => Motor.GroundingStatus.IsStableOnGround
            ));
            _fsm.AddState("Jump", new CharacterState(
                onUpdateVelocity: (currentVelocity, deltaTime) =>
                {
                    Motor.ForceUnground();
                    VVelocity += Gravity * deltaTime;
                    return VVelocity + currentVelocity.ProjectOntoPlane(Motor.CharacterUp);
                },
                onEnter: _ =>
                {
                    Voice.PlayRandomly();
                    Animator.SetTrigger(Jump);
                    VVelocity = Motor.CharacterUp * Mathf.Sqrt(2 * JumpHeight * 9.81f);
                    Motor.ForceUnground(VVelocity.magnitude / 9.81f);
                },
                onExit: _ => { IsJumping = false; }
            ));
            _fsm.AddTransition(new Transition("Locomotion", "Jump",
                condition: _ => IsJumping
            ));
            _fsm.AddTransition(new Transition("Jump", "Fall",
                condition: _ => VVelocity.y * Motor.CharacterUp.y <= 0
            ));

            // Attack State
            _fsm.AddState("Attack", new AttackState(this) { ComboAsset = ComboAsset });
            _fsm.AddTransition(new Transition("Locomotion", "Attack",
                condition: _ => IsAttacking
            ));
            _fsm.AddTransition(new Transition("Attack", "Locomotion"));

            _fsm.SetStartState("Locomotion");
            
            // Gravity Change State
            _gravityChangeState = new GravityChangeState(this, true);
            _fsm.AddState("GravityChange", _gravityChangeState);
            _fsm.AddTriggerTransitionFromAny("GravityChange", new Transition("","GravityChange"));
            _fsm.AddTransition(new Transition("GravityChange", "Fall"));
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

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
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


        void IGravity.OnGravityChanged(in Vector3 oldDir, in Vector3 newDir)
        {
            Debug.Log("Gravity Changed");
            Gravity = newDir * 9.81f;
            _gravityChangeState.GravityTargetDir = newDir;
            _fsm.Trigger("GravityChange");
        }

        void IGravity.OnForceFieldEnter(in Vector3 fieldPos)
        {
        }

        void IGravity.OnForceFieldExit(in Vector3 fieldPos)
        {
        }

        void IGravity.OnForceFieldChanged(in Vector3 fieldPos)
        {
        }
    }
}