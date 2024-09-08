using System;
using RuneForger.GravityField;
using UnityEngine;
using UnityEngine.Serialization;

namespace RuneForger.Scene
{
    [SelectionBase]
    public class DrillCar : MonoBehaviour, IGravity
    {
        private Vector3 _velocity;
        void IGravity.OnGravityChanged(in Vector3 oldDir, in Vector3 newDir)
        {
        }

        void IGravity.OnForceFieldEnter(in Vector3 fieldPos)
        {
            var dir = fieldPos - transform.position;
            dir = Vector3.Project(dir, transform.forward);
            _velocity = dir * 10f;
        }

        void IGravity.OnForceFieldExit(in Vector3 fieldPos)
        {
        }

        void IGravity.OnForceFieldChanged(in Vector3 fieldPos)
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 10);
        }

        private void OnCollisionEnter(Collision other)
        {
            _velocity = Vector3.zero;
            var fragileDoor = other.transform.GetComponentInParent<FragileDoor>();
            if (fragileDoor == null) return;
            fragileDoor.Break();
        }

        private void Update()
        {
            if (_velocity.sqrMagnitude <= 0.1f) return;
            transform.position += _velocity * Time.deltaTime;
        }
    }
}