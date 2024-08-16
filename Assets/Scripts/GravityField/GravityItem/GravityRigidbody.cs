using UnityEngine;

namespace RuneForger.GravityField.GravityItem
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityRigidbody : MonoBehaviour, IGravity
    {
        private Rigidbody _rigidbody;
        private Vector3 _gravity = Vector3.down * 9.81f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }

        private void Update()
        {
            _rigidbody.AddForce(_gravity);
        }

        void IGravity.OnGravityChanged(in Vector3 oldValue, in Vector3 newValue)
        {
            _gravity = newValue;
        }

        void IGravity.OnForceFieldChanged(in Vector3 oldValue, in Vector3 newValue)
        {
            
        }
    }
}