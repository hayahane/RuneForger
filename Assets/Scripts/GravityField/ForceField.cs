using System;
using System.Collections.Generic;
using UnityEngine;

namespace RuneForger.GravityField
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField]
        private float radius = 8f;
        private readonly LinkedList<IGravity> _gravityObjects = new();

        private void OnTriggerEnter(Collider other)
        {
            var gravity = other.GetComponent<IGravity>();
            if (gravity == null) return;
            gravity.OnForceFieldEnter();
            _gravityObjects.AddLast(gravity);
        }

        private void OnTriggerExit(Collider other)
        {
            var gravity = other.GetComponent<IGravity>();
            if (gravity == null) return;
            gravity.OnForceFieldExit();
            _gravityObjects.Remove(gravity);
        }

        private void Update()
        {
            foreach (var gravity in _gravityObjects)
            {
                gravity.OnForceFieldChanged(transform.position);
            }
        }
    }
}