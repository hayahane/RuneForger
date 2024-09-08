using System;
using UnityEngine;

namespace RuneForger.GravityField.GravityItem
{
    public class Shard : MonoBehaviour, IGravity
    {
        private bool _isFieldControlled = false;
        private Vector3 _target;
        
        void IGravity.OnGravityChanged(in Vector3 oldDir, in Vector3 newDir)
        {
        }

        void IGravity.OnForceFieldEnter(in Vector3 fieldPos)
        {
            Debug.Log("Shard Get Field Controlled");
            _isFieldControlled = true;
            _target = fieldPos;
        }

        void IGravity.OnForceFieldExit(in Vector3 fieldPos)
        {
            _isFieldControlled = false;
        }

        void IGravity.OnForceFieldChanged(in Vector3 fieldPos)
        {
        }

        private void Update()
        {
            if (!_isFieldControlled) return;
            
            transform.position = Vector3.MoveTowards(transform.position, _target, 10 * Time.deltaTime);
            if (transform.position == _target)
            {
                Destroy(gameObject);
            }
        }
    }
}