using UnityEngine;

namespace RuneForger.GravityField.GravityItem
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityRigidbody : MonoBehaviour, IGravity
    {
        private Rigidbody _rigidbody;
        private Vector3 _gravity = Vector3.down * 9.81f;
        private bool _isInForceField;
        private Vector3 _forceFieldPos;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        private void Update()
        {
            if (!_isInForceField)
                _rigidbody.AddForce(_gravity, ForceMode.Acceleration);
            else
            {
                var v = (_forceFieldPos - transform.position).normalized;
                _rigidbody.velocity = v * 10f;
            }
        }
        

        #region IGravity Implementation

        void IGravity.OnGravityChanged(in Vector3 oldValue, in Vector3 newValue)
        {
            _gravity = newValue;
        }

        void IGravity.OnForceFieldEnter()
        {
            _isInForceField = true;
        }

        void IGravity.OnForceFieldExit()
        {
            _isInForceField = false;
        }

        void IGravity.OnForceFieldChanged(in Vector3 fieldPos)
        {
            _forceFieldPos = fieldPos;
        }

        #endregion
    }
}