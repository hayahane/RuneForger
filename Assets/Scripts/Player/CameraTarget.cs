using UnityEngine;

namespace RuneForger
{
    public class CameraTarget : MonoBehaviour
    {
        [field: SerializeField] public Vector2 PitchClamp { get; set; } = new(-50f, 70f);
        public Vector2 AimInput { get; set; }

        private const float Threshold = 0.1f;

        private float _cameraYaw;

        private float _cameraPitch;
        public bool IsPitchLocked { get; set; }

        private void OnEnable()
        {
            var euler = transform.eulerAngles;
            _cameraYaw = euler.y;
            _cameraPitch = euler.x;
        }

        // Update is called once per frame
        void Update()
        {
            if (AimInput.magnitude >= Threshold)
            {
                var dir = Mathf.Sign(transform.up.y);
                _cameraYaw += AimInput.x * dir;
                if (!IsPitchLocked)
                    _cameraPitch += AimInput.y * dir;

                _cameraPitch = ClampAngle(_cameraPitch, PitchClamp.x, PitchClamp.y);
                _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
            }

            transform.rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, transform.parent.rotation.eulerAngles.z);
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360f;
            if (angle > 360) angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }
        
        public void ResetPitch()
        {
            _cameraPitch = 0;
        }
    }
}