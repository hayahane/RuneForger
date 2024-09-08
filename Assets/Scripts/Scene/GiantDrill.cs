using System;
using UnityEngine;
using UnityEngine.Playables;

namespace RuneForger.Scene
{
    public class GiantDrill : MonoBehaviour
    {
        private Rigidbody _rb;
        [SerializeField]
        private PlayableDirector pd;

        private float _timer = 5f;
        private bool _activated = false;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            pd.played += (_) =>
            {
                _timer = 5f;
                _activated = true;
                Debug.Log("GiantDrill Rolling!");
            };
        }

        private void Update()
        {
            if (_timer >= 0) {_timer -= Time.deltaTime;}
            if (_timer < 0 && _activated)
            {
                Debug.Log("Drill On!");
                _rb.useGravity = true;
                _rb.velocity = Vector3.down * 10;
                _activated = false;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            _rb.velocity = Vector3.zero;
            var fragileDoor = other.transform.GetComponentInParent<FragileDoor>();
            if (fragileDoor == null) return;
            fragileDoor.Break();
        }
    }
}