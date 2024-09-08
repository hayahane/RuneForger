using System;
using UnityEngine;

namespace RuneForger.Interact
{
    [SelectionBase]
    public class BonusChest : InteractItem
    {
        private bool _isOpen;
        public override string InteractText => "打开宝箱";
        private Rigidbody _upHalf;
        private AudioSource _audioSource;

        private void Awake()
        {
            _upHalf = GetComponentInChildren<Rigidbody>();
            _upHalf.constraints = RigidbodyConstraints.FreezeAll;

            _audioSource = GetComponent<AudioSource>();
        }

        public override void OnInteract()
        {
            _isOpen = true;
            _upHalf.constraints = RigidbodyConstraints.None;
            _upHalf.AddForceAtPosition(Vector3.up * 2, _upHalf.position + new Vector3(0.1f,0,0),ForceMode.Impulse);
            _audioSource.Play();
        }

        public override bool CanInteract(Transform trans)
        {
            return !_isOpen;
        }
    }
}