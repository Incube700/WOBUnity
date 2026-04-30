using RicochetTanks.Gameplay.Combat;

namespace RicochetTanks.Statistics
{
    public sealed class MatchStatistics
    {
        public int ShotsFired { get; set; }
        public int TankHits { get; set; }
        public int Penetrations { get; set; }
        public int NoPenetrations { get; set; }
        public int Ricochets { get; set; }
        public int SelfHits { get; set; }
        public float DamageDealt { get; set; }
        public float DamageTaken { get; set; }
        public MatchResult MatchResult { get; set; } = MatchResult.Playing;

        public float AccuracyPercent
        {
            get { return ShotsFired > 0 ? (float)TankHits / ShotsFired * 100f : 0f; }
        }
    }
}
