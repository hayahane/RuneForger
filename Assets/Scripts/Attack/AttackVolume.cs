using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RuneForger.Attack
{
    public class AttackVolume : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _detectLayer;
        private readonly List<Collider> _excludeColliders = new List<Collider>();
        private readonly List<Collider> _handledColliders = new List<Collider>(64);
        private readonly Collider[] _det = new Collider[32];

        [field: SerializeField]
        public Vector3 VolumeOrigin {get;set;}
        [field: SerializeField]
        public float VolumeSize {get;set;} = 0.8f;
        public Action<Collider> OnAttackHit {get; set;}

        private float _detectTimer;
        private AttackInfo _attackInfo;

        private void Start()
        {
            // Add self colliders to exclude list
            _excludeColliders.Add(GetComponent<Collider>());
            var colliders = GetComponentsInChildren<HitVolume>();
            foreach (var collider in colliders)
            {
                _excludeColliders.Add(collider.GetComponent<Collider>());
            }
        }

        private void Update()
        {
            _detectTimer -= Time.deltaTime;
            if (_detectTimer < 0f)
            {
                _detectTimer = 0f;
                EndDetect();
            }
        }

        private void FixedUpdate()
        {
            var count = Physics.OverlapSphereNonAlloc(VolumeOrigin + transform.position, VolumeSize, _det, _detectLayer);
            if (count <= 0) return;
            for (var i = 0; i < count; i++)
            {
                if (_excludeColliders.Contains(_det[i])) continue;
                if (_handledColliders.Contains(_det[i])) continue;

                Debug.Log($"Hit {_det[i].name}");
                var attackables = _det[i].GetComponents<IAttackable>();
                if (attackables == null || attackables.Length <= 0) continue;
                
                foreach (var attackable in attackables)
                {
                    attackable.OnHit(_attackInfo);
                }
                _handledColliders.Add(_det[i]);
            }
        }

        private void OnDrawGizmos()
        {
            if (this.enabled == false) return;
            Gizmos.color = new Color(0.9f, 0.6f, 0f, 1f);
            Gizmos.DrawSphere(VolumeOrigin + transform.position, VolumeSize);
        }

        public void BeginDetect(AttackInfo attackInfo, float detectTime)
        {
            _detectTimer = detectTime;
            _attackInfo = attackInfo;
            this.enabled = true;
        }

        public void EndDetect()
        {
            _handledColliders.Clear();
            this.enabled = false;
        }
    }
}