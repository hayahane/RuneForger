using UnityEngine;
using UnityEngine.InputSystem;

namespace RuneForger
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
        }

        private void Update()
        {
            Character.HVelocity = Quaternion.Euler(0, ViewCamera.transform.eulerAngles.y, 0)
                    * new Vector3(_moveInput.x, 0, _moveInput.y);
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            _moveInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        }

        private void OnJumpInput(InputAction.CallbackContext context)
        {
            Character.IsJumping = context.started;
            if (Character.IsJumping)
                Debug.Log("Input Jumping");
        }

        private void OnLookInput(InputAction.CallbackContext context)
        {
            CameraTarget.AimInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        }

        private void OnReleaseCursorInput(InputAction.CallbackContext context)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}