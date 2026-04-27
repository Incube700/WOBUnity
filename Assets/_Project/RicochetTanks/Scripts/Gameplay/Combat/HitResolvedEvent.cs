using RicochetTanks.Gameplay.Tanks;

namespace RicochetTanks.Gameplay.Combat
{
    public readonly struct HitResolvedEvent
    {
        public HitResolvedEvent(TankFacade source, TankFacade target, HitResult result, float damage, float currentHp, float maxHp, ArmorHitInfo armorHit)
        {
            Source = source;
            Target = target;
            Result = result;
            Damage = damage;
            CurrentHp = currentHp;
            MaxHp = maxHp;
            ArmorHit = armorHit;
        }

        public TankFacade Source { get; }
        public TankFacade Target { get; }
        public HitResult Result { get; }
        public float Damage { get; }
        public float CurrentHp { get; }
        public float MaxHp { get; }
        public ArmorHitInfo ArmorHit { get; }
    }
}
