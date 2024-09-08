using System;
using UnityEngine;
using UnityEngine.Events;

namespace RuneForger.GravityField.GravityItem
{
    public class GravitySwitch : MonoBehaviour, IGravity
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int Inverted = Shader.PropertyToID("_Inverted");
        private static readonly int Height = Shader.PropertyToID("_Height");

        [SerializeField]
        private UnityEvent<bool> onSwitchStateChange;
        private int _gravityDirection = 1; // 1 代表正常重力向下流动，-1 代表反向重力向上流动
        private int _fieldDirection = 1;
        private bool _isInForceField;
        private int _lastIsOn = 1;
        private Material _upMaterial;
        private Material _downMaterial;
        private Material _indicatorMaterial;
        private Color _emissionColor;

        private const float _height = 0.46f;
        private readonly float changeTime = 0.1f;
        private float _timer = 0;

        [SerializeField]
        private AudioClip onSound;
        [SerializeField]
        private AudioClip offSound;
        private AudioSource _audioSource;
        
        private void Awake()
        {
            _downMaterial = transform.GetChild(1).GetComponent<MeshRenderer>().material;
            _upMaterial = transform.GetChild(2).GetComponent<MeshRenderer>().material;
            _indicatorMaterial = transform.GetChild(3).GetComponent<MeshRenderer>().materials[2];
            _emissionColor = _indicatorMaterial.GetColor(EmissionColor);
            _indicatorMaterial.SetColor(EmissionColor, Color.black);
            
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (_timer <= 0)
            {
                return;
            }
            
            _timer -= Time.deltaTime;
            _downMaterial.SetFloat(Height, (changeTime - _timer) / changeTime * _height);
            _upMaterial.SetFloat(Height, (_timer) / changeTime * _height);
        }
        
        private void OnDrawGizmos()
        {
            if (onSwitchStateChange == null) return;
            Gizmos.color = new Color(0.7f, 0.5f, 0.2f, 0.5f);
            for (var i = 0; i < onSwitchStateChange.GetPersistentEventCount(); i++)
            {
                if (onSwitchStateChange.GetPersistentTarget(i) is Component target)
                {
                    Gizmos.DrawLine(transform.position, target.transform.position);
                }
            }
        }

        private void CheckSwitchState()
        {
            var isOn = _isInForceField ? _fieldDirection : _gravityDirection;
            if (isOn == _lastIsOn)
            {
                return;
            }
            _lastIsOn = isOn;
            _audioSource.clip = isOn == -1 ? onSound : offSound;
            _audioSource.Play();
            onSwitchStateChange?.Invoke(isOn == -1);
            _upMaterial.SetFloat(Inverted, isOn);
            _upMaterial.SetFloat(Height, isOn == -1 ? _height : 0);
            _downMaterial.SetFloat(Inverted, isOn);
            _downMaterial.SetFloat(Height, isOn != -1 ? _height : 0);
            _indicatorMaterial.SetColor(EmissionColor, isOn == -1 ? _emissionColor : Color.black);
            
            _timer = _timer <= 0 ? changeTime : changeTime - _timer;
        }
        
        void IGravity.OnGravityChanged(in Vector3 oldDir, in Vector3 newDir)
        {
            _gravityDirection = newDir.y * transform.up.y < 0 ? 1 : -1;
            CheckSwitchState();
        }

        void IGravity.OnForceFieldEnter(in Vector3 fieldPos)
        {
            _isInForceField = true;
            var dir = (fieldPos - transform.position).normalized;
            _fieldDirection = dir.y * transform.up.y < 0 ? 1 : -1;
            CheckSwitchState();
        }

        void IGravity.OnForceFieldExit(in Vector3 fieldPos)
        {
            _isInForceField = false;
            CheckSwitchState();
        }

        void IGravity.OnForceFieldChanged(in Vector3 fieldPos)
        {
            var dir = (fieldPos - transform.position).normalized;
            _fieldDirection = dir.y * transform.up.y < 0 ? 1 : -1;
            CheckSwitchState();
        }
    }
}