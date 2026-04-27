namespace RicochetTanks.Gameplay.Combat
{
    public interface IDamageable
    {
        float CurrentHp { get; }
        float MaxHp { get; }
        bool IsAlive { get; }
        void ApplyDamage(float damage);
    }
}
