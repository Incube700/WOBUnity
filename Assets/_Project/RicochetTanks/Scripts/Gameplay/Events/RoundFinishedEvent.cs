using RicochetTanks.Gameplay.Combat;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct RoundFinishedEvent
    {
        public RoundFinishedEvent(MatchResult result, string label, int playerScore, int enemyScore, int roundNumber, int roundsToWin)
        {
            Result = result;
            Label = label;
            PlayerScore = playerScore;
            EnemyScore = enemyScore;
            RoundNumber = roundNumber;
            RoundsToWin = roundsToWin;
        }

        public MatchResult Result { get; }
        public string Label { get; }
        public int PlayerScore { get; }
        public int EnemyScore { get; }
        public int RoundNumber { get; }
        public int RoundsToWin { get; }
    }
}
