namespace RicochetTanks.Gameplay.Combat
{
    public interface IDamageable
    {
        int CurrentHp { get; }
        int MaxHp { get; }
        bool IsAlive { get; }
        void ApplyDamage(int damage);
    }
}
