using RicochetTanks.Gameplay.Tanks;

namespace RicochetTanks.Gameplay.Combat
{
    public readonly struct HitResolvedEvent
    {
        public HitResolvedEvent(TankFacade source, TankFacade target, HitResult result, int damage, int currentHp, int maxHp, ArmorHitInfo armorHit)
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
        public int Damage { get; }
        public int CurrentHp { get; }
        public int MaxHp { get; }
        public ArmorHitInfo ArmorHit { get; }
    }
}
