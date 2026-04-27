namespace RicochetTanks.Gameplay.Combat
{
    public readonly struct ArmorHitInfo
    {
        public ArmorHitInfo(ArmorZone zone, int effectiveArmor, float hitAngle, int penetration)
        {
            Zone = zone;
            EffectiveArmor = effectiveArmor;
            HitAngle = hitAngle;
            Penetration = penetration;
        }

        public ArmorZone Zone { get; }
        public int EffectiveArmor { get; }
        public float HitAngle { get; }
        public int Penetration { get; }
    }
}
