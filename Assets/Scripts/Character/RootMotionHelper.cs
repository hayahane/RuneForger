using UnityEngine;

namespace RuneForger
{
    public class RootMotionHelper : MonoBehaviour
    {
        private Animator _animator;
        public Vector3 StoredRootMotion
        {
            get
            {
                var v = _storedRootMotion * (IsRootMotionApplied ? 1 : 0);
                _storedRootMotion = Vector3.zero;
                return v;
            }
        }
        private Vector3 _storedRootMotion;
        public bool IsRootMotionApplied { get; set; } = false;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorMove()
        {
            if (IsRootMotionApplied)
                _storedRootMotion = _animator.deltaPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _storedRootMotion * 100);
        }
    }
}