namespace RicochetTanks.Gameplay.Combat
{
    public readonly struct ArmorHitInfo
    {
        public ArmorHitInfo(
            ArmorZone zone,
            float armor,
            float effectiveArmor,
            float hitAngle,
            float impactDot,
            float penetration,
            float kineticFactor)
        {
            Zone = zone;
            Armor = armor;
            EffectiveArmor = effectiveArmor;
            HitAngle = hitAngle;
            ImpactDot = impactDot;
            Penetration = penetration;
            KineticFactor = kineticFactor;
        }

        public ArmorZone Zone { get; }
        public float Armor { get; }
        public float EffectiveArmor { get; }
        public float HitAngle { get; }
        public float ImpactDot { get; }
        public float Penetration { get; }
        public float KineticFactor { get; }
    }
}
