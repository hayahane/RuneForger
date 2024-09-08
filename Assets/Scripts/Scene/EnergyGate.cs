using System;
using DG.Tweening;
using UnityEngine;

namespace RuneForger.Scene
{
    public class EnergyGate : MonoBehaviour
    {
        private static readonly int Vanish = Shader.PropertyToID("_Vanish");
        
        private Material _gateMaterial;
        private Collider _col;
        public bool IsOn { get; private set; } = true;
        
        [field: SerializeField]
        private float CloseTime { get; set; } = 0.5f;
        private float _timer = 0;

        private void Awake()
        {
            _gateMaterial = GetComponent<MeshRenderer>().material;
            _col = GetComponent<Collider>();
        }

        private void Update()
        {
            if (_timer <= 0) return;
            
            _timer -= Time.deltaTime;
            
            if (!(_timer <= 0)) return;
            _col.enabled = IsOn;
        }

        public void ChangeGateState()
        {
            IsOn = !IsOn;
            _timer = _timer <= 0 ? CloseTime : CloseTime - _timer;
            _gateMaterial.DOFloat(IsOn ? 1:0, Vanish, CloseTime);
        }
    }
}