using System;
using RuneForger.Character;
using UnityEngine;
using UnityEngine.Serialization;

namespace RuneForger.Battle
{
    public class HitVolume : MonoBehaviour, IAttackable
    {
        private GameCharacter _character;
        public GameCharacter Character => _character;
        private CharacterStatus _status;

        private void Awake()
        {
            _character = GetComponentInParent<GameCharacter>();
            _status = _character.GetComponent<CharacterStatus>();
        }

        void IAttackable.OnHit(AttackInfo attackInfo)
        {
            _status.CurrentHitPoints -= attackInfo.Damage;
        }
    }
}