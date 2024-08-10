using System;
using UnityEngine;

namespace RuneForger.Character
{
    public class CharacterStatus : MonoBehaviour
    {
        [field: SerializeField] public float MaxHitPoints { get; private set; } = 30f;
        private float _currentHitPoints;
        public event Action CharacterDieEvent;
        public float CurrentHitPoints
        {
            get => _currentHitPoints;
            set
            {
                _currentHitPoints = Mathf.Clamp(value, 0, MaxHitPoints);
                if (_currentHitPoints <= 0)
                {
                    CharacterDieEvent?.Invoke();
                }
            }
        }

        [field: SerializeField] 
        public float MaxEnergy { get; private set; } = 50f;
        private float _currentEnergy;
        public event Action CharacterEnergyEmptyEvent;
        public float CurrentEnergy
        {
            get => _currentEnergy;
            set
            {
                _currentEnergy = Mathf.Clamp(value, 0, MaxEnergy);
                if (_currentEnergy <= 0)
                {
                    CharacterEnergyEmptyEvent?.Invoke();
                }
            }
        }
        
        private void Start()
        {
            CurrentHitPoints = MaxHitPoints;
            CurrentEnergy = MaxEnergy;
        }
    }
}
