using UnityEngine;
using System;
using System.Collections.Generic;

namespace RuneForger.GravityField
{
    [RequireComponent(typeof(BoxCollider))]
    public class GravityField : MonoBehaviour
    {
        [field: SerializeField] 
        public Vector3 Extends { get; set; } = Vector3.one * 10;
        private const float Magnitude = 9.81f;
        
        [SerializeField]
        private Vector3 _fieldDirection = Vector3.down;
        public Vector3 FieldDirection
        {
            get => _fieldDirection;
            set
            {
                var oldValue = _fieldDirection;
                _fieldDirection = value.normalized;
                OnFieldChanged(oldValue, _fieldDirection);
            }
        }
        public Vector3 Gravity => Magnitude * FieldDirection;
        
        private readonly LinkedList<IGravity> _gravityObjects = new();

        private void Initialize()
        {
            var cols = new Collider[32];
            var size = Physics.OverlapBoxNonAlloc(transform.position, Extends, cols);
            if (size <= 0 ) return;
            for (var i = 0; i < size; i++)
            {
                var gravityObject = cols[i].GetComponent<IGravity>();
                if (gravityObject != null)
                {
                    AddGravityObject(gravityObject);
                }
            }
        }

        private void OnValidate()
        {
            var col = GetComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = Extends;
        }

        private void Start()
        {
            Initialize();
        }

        private void OnFieldChanged(in Vector3 oldValue, in Vector3 newValue)
        {
            foreach (var gravityObject in _gravityObjects)
            {
                gravityObject.OnGravityChanged(oldValue, newValue);
            }
        }
        
        private void AddGravityObject(IGravity gravityObject)
        {
            if (_gravityObjects.Contains(gravityObject)) return;
            _gravityObjects.AddLast(gravityObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            var gravityObject = other.GetComponent<IGravity>();
            if (gravityObject == null) return;
            if (_gravityObjects.Contains(gravityObject)) return;
            AddGravityObject(gravityObject);
        }

        private void OnTriggerExit(Collider other)
        {
            var gravityObject = other.GetComponent<IGravity>();
            if (gravityObject == null) return;
            if (!_gravityObjects.Contains(gravityObject)) return;
            _gravityObjects.Remove(gravityObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.8f,0.5f,0.2f,0.5f);
            Gizmos.DrawCube(transform.position, Extends);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.8f,0.5f,0.2f,0.2f);
            Gizmos.DrawWireCube(transform.position, Extends);
        }
    }
}