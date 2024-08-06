using UnityEngine;
using RuneForger.Character;
using RuneForger.Attack;

namespace RuneForger.Attack
{
    public class HitVolume : MonoBehaviour, IAttackable
    {
        [SerializeField]
        private GameCharacter _character;
        public GameCharacter Character => _character;

        void IAttackable.OnHit(AttackInfo attackInfo)
        {
            throw new System.NotImplementedException();
        }
    }
}