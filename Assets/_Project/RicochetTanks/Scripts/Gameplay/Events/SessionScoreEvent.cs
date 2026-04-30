namespace RicochetTanks.Gameplay.Events
{
    public readonly struct SessionScoreEvent
    {
        public SessionScoreEvent(int playerScore, int enemyScore, int roundNumber, int roundsToWin)
        {
            PlayerScore = playerScore;
            EnemyScore = enemyScore;
            RoundNumber = roundNumber;
            RoundsToWin = roundsToWin;
        }

        public int PlayerScore { get; }
        public int EnemyScore { get; }
        public int RoundNumber { get; }
        public int RoundsToWin { get; }
    }
}
