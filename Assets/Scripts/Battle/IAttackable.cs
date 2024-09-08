using RuneForger.Character;

namespace RuneForger.Battle
{
    public interface IAttackable
    {
        public void OnHit(AttackInfo attackInfo);
    }

    public struct AttackInfo
    {
        public GameCharacter Source;
        public float Damage;
    }
}