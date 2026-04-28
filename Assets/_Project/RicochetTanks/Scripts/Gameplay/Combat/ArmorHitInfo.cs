namespace RicochetTanks.Gameplay.Combat
{
    public readonly struct ArmorHitInfo
    {
        public ArmorHitInfo(ArmorZone zone, float armor, float effectiveArmor, float hitAngle, float impactDot, int penetration)
        {
            Zone = zone;
            Armor = armor;
            EffectiveArmor = effectiveArmor;
            HitAngle = hitAngle;
            ImpactDot = impactDot;
            Penetration = penetration;
        }

        public ArmorZone Zone { get; }
        public float Armor { get; }
        public float EffectiveArmor { get; }
        public float HitAngle { get; }
        public float ImpactDot { get; }
        public int Penetration { get; }
    }
}
