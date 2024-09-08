using RuneForger.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuneForger.Player
{
    public class PCController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput playerInput;
        [field: SerializeField]
        public Character.GameCharacter Character { get; set; }
        [field: SerializeField]
        public Character.CharacterInteract Interact { get; set; }
        [field: SerializeField]
        public Camera ViewCamera { get; set; }
        [field: SerializeField]
        public CameraTarget CameraTarget { get; set; }

        private Vector2 _moveInput;

        private void Start()
        {
            if (playerInput == null)
            {
                playerInput = GetComponent<PlayerInput>();
            }
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            playerInput.actions["ReleaseCursor"].performed += OnReleaseCursorInput;

            playerInput.actions["Move"].performed += OnMoveInput;
            playerInput.actions["Move"].canceled += OnMoveInput;
            playerInput.actions["Jump"].started += OnJumpInput;
            playerInput.actions["Jump"].canceled += OnJumpInput;
            playerInput.actions["Look"].performed += OnLookInput;
            playerInput.actions["Look"].canceled += OnLookInput;

            playerInput.actions["Attack"].performed += _ => Character.IsAttacking = true;
            playerInput.actions["Attack"].canceled += _ => Character.IsAttacking = false;

            playerInput.actions["Interact"].performed += _ => Interact.Interact();
            
            playerInput.actions["ForceField"].started += OnForceFieldControl;
            playerInput.actions["ForceField"].canceled += OnForceFieldControl;
        }

        private void Update()
        {
            var up = Vector3.Project(ViewCamera.transform.up, Vector3.up).normalized;
            var forward = Vector3.ProjectOnPlane(ViewCamera.transform.forward, Vector3.up).normalized;
            Character.HVelocity = Quaternion.LookRotation(forward, up)
                    * new Vector3(_moveInput.x, 0, _moveInput.y);
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        }

        private void OnJumpInput(InputAction.CallbackContext context)
        {
            Character.IsJumping = context.started;
        }

        private void OnLookInput(InputAction.CallbackContext context)
        {
            CameraTarget.AimInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        }

        private void OnReleaseCursorInput(InputAction.CallbackContext context)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        private void OnForceFieldControl(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (GameplayManager.Instance.ForceFieldEmitter.IsFieldActive)
                    GameplayManager.Instance.ForceFieldEmitter.Dissolve();
                else
                    GameplayManager.Instance.ForceFieldEmitter.BeginAiming();
            }

            if (context.canceled)
            {   
                if (GameplayManager.Instance.ForceFieldEmitter.IsAiming)
                     GameplayManager.Instance.ForceFieldEmitter.Emit();
            }
        }
    }
}