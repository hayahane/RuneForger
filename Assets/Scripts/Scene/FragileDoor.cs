using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RuneForger.Scene
{
    public class FragileDoor : MonoBehaviour
    {
        private Rigidbody[] _rigidbodies;
        private bool _isBroken;
        private float _vanishTimer = 5f;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
            foreach (var rb in _rigidbodies)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void Update()
        {
            if (!_isBroken) return;
            if (_vanishTimer <= 0)
            {
                Destroy(this.gameObject);
                _vanishTimer = 10;
                return;
            }

            _vanishTimer -= Time.deltaTime;
        }

        public void Break()
        {
            if (_isBroken) return;
            _isBroken = true;
            _audioSource.Play();
            Debug.Log($"{gameObject.name}Broken");
            
            foreach (var rb in _rigidbodies)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.useGravity = true;
                rb.AddForce(-transform.forward * 50f, ForceMode.Acceleration);
            }
        }
    }
}