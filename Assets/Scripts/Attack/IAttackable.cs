using RuneForger.Character;

namespace RuneForger.Attack
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