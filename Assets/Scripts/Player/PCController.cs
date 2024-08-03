using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RuneForger
{
    public class PCController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInput _playerInput;
        [field: SerializeField]
        public Character.GameCharacter Character { get; set; }
        [field: SerializeField]
        public Camera ViewCamera { get; set; }
        [field: SerializeField]
        public CameraTarget CameraTarget { get; set; }

        private Vector2 _moveInput;

        private void Start()
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _playerInput.actions["ReleaseCursor"].performed += OnReleaseCursorInput;

            _playerInput.actions["Move"].performed += OnMoveInput;
            _playerInput.actions["Move"].canceled += OnMoveInput;
            _playerInput.actions["Jump"].started += OnJumpInput;
            _playerInput.actions["Jump"].canceled += OnJumpInput;
            _playerInput.actions["Look"].performed += OnLookInput;
            _playerInput.actions["Look"].canceled += OnLookInput;

            _playerInput.actions["Attack"].performed += context => Character.IsAttacking = true;
            _playerInput.actions["Attack"].canceled += context => Character.IsAttacking = false;
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