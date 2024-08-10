using System;
using RuneForger.Gameplay;
using RuneForger.Interact;
using UnityEngine;

namespace RuneForger.Character
{
    public class CharacterInteract : MonoBehaviour
    {
        public InteractItem Interactable {get; private set;}
        private float _directionDot = 0;
        public event Action<InteractItem> OnItemChangedEvent;

        [field: SerializeField]
        public float DetectInterval {get; set;} = 0.1f;
        private float _detectTimer = 0;
        [field: SerializeField]
        public float InteractRange {get; set;} = 0.8f;
        [field: SerializeField]
        public Vector3 OriginOffset {get; set;}
        private LayerMask _interactLayer;
        private readonly Collider[] _det = new Collider[32];
        private Animator _animator;

        private void OnEnable()
        {
            _animator = GetComponentInChildren<Animator>();
            _interactLayer = LayerMask.NameToLayer("Interact");
        }

        private void OnDisable()
        {
            if (GameplayManager.Instance == null) return;
            GameplayManager.Instance.CharacterInteract = null;
        }

        private void FixedUpdate()
        {
            if (_detectTimer < DetectInterval)
            {
                _detectTimer += Time.deltaTime;
                return;
            }

            var tempInteractable = null as InteractItem;
            _detectTimer = 0f;
            _directionDot = 0f;
            var count = Physics.OverlapSphereNonAlloc(transform.position + OriginOffset,
                    InteractRange, _det, 1 << _interactLayer);
            for (int i = 0 ; i < count; i++)
            {
                var comp = _det[i].GetComponent<InteractItem>();
                if (!comp) continue;
                if (!comp.CanInteract(this.transform)) continue;
                var dotRes = Vector3.Dot((comp.transform.position - transform.position).normalized,
                        _animator.transform.forward);
                if (dotRes < _directionDot) continue;
                _directionDot = dotRes;
                tempInteractable = comp;
            }

            if (tempInteractable == Interactable) return;
            Interactable = tempInteractable;
            OnItemChangedEvent?.Invoke(Interactable);
        }

        public void Interact()
        {
            Interactable?.OnInteract();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.5f, 0.8f, 0.2f, 0.2f);
            Gizmos.DrawSphere(transform.position + OriginOffset, InteractRange);
        }
    }
}