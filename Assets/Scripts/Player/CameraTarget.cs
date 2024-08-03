using UnityEngine;

namespace RuneForger
{
    public class CameraTarget : MonoBehaviour
    {
        [field: SerializeField]
        public Vector2 PitchClamp { get; set; } = new(-50f, 70f);
        public Vector2 AimInput { get; set; }

        private const float Threshold = 0.1f;

        private float _cameraYaw;

        private float _cameraPitch;

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
                _cameraYaw += AimInput.x;
                _cameraPitch += AimInput.y;

                _cameraPitch = ClampAngle(_cameraPitch, PitchClamp.x, PitchClamp.y);
                _cameraYaw = ClampAngle(_cameraYaw, float.MinValue, float.MaxValue);
            }

            transform.rotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0);
        }

        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360f;
            if (angle > 360) angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }
    }
}