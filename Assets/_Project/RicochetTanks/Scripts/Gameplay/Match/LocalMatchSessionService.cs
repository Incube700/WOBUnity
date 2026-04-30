using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Statistics;

namespace RicochetTanks.Gameplay.Match
{
    public sealed class LocalMatchSessionService
    {
        private static LocalMatchSessionState _state;

        public bool HasActiveMatch
        {
            get { return _state != null && _state.IsActive; }
        }

        public int PlayerScore
        {
            get { return _state != null ? _state.PlayerScore : 0; }
        }

        public int EnemyScore
        {
            get { return _state != null ? _state.EnemyScore : 0; }
        }

        public int CurrentRound
        {
            get { return _state != null ? _state.CurrentRound : 1; }
        }

        public int RoundsToWin
        {
            get { return _state != null ? _state.RoundsToWin : 3; }
        }

        public MatchResult FinalResult
        {
            get { return _state != null ? _state.FinalResult : MatchResult.Playing; }
        }

        public MatchStatistics CurrentStatistics
        {
            get
            {
                EnsureMatch(3);
                return _state.CurrentStatistics;
            }
        }

        public void StartNewMatch(int roundsToWin)
        {
            _state = new LocalMatchSessionState(roundsToWin);
        }

        public bool EnsureMatch(int roundsToWin)
        {
            if (_state == null || !_state.IsActive)
            {
                StartNewMatch(roundsToWin);
                return true;
            }

            _state.RoundsToWin = roundsToWin > 0 ? roundsToWin : _state.RoundsToWin;
            return false;
        }

        public bool ConsumeMatchStartedFlag()
        {
            if (_state == null || !_state.ShouldRaiseMatchStarted)
            {
                return false;
            }

            _state.ShouldRaiseMatchStarted = false;
            return true;
        }

        public void ResetStatistics()
        {
            EnsureMatch(RoundsToWin);
            _state.CurrentStatistics = new MatchStatistics();
            _state.IsStatisticsSaved = false;
        }

        public void RecordRound(MatchResult result)
        {
            EnsureMatch(RoundsToWin);

            if (result == MatchResult.PlayerWins)
            {
                _state.PlayerScore++;
            }
            else if (result == MatchResult.EnemyWins)
            {
                _state.EnemyScore++;
            }

            _state.CurrentRound++;
        }

        public bool HasWinner()
        {
            return _state != null
                && (_state.PlayerScore >= _state.RoundsToWin || _state.EnemyScore >= _state.RoundsToWin);
        }

        public MatchResult CompleteMatch()
        {
            EnsureMatch(RoundsToWin);

            if (_state.PlayerScore > _state.EnemyScore)
            {
                _state.FinalResult = MatchResult.PlayerWins;
            }
            else if (_state.EnemyScore > _state.PlayerScore)
            {
                _state.FinalResult = MatchResult.EnemyWins;
            }
            else
            {
                _state.FinalResult = MatchResult.Draw;
            }

            _state.CurrentStatistics.MatchResult = _state.FinalResult;
            return _state.FinalResult;
        }

        public bool TryMarkStatisticsSaved()
        {
            if (_state == null || _state.IsStatisticsSaved)
            {
                return false;
            }

            _state.IsStatisticsSaved = true;
            return true;
        }

        public void AbandonMatch()
        {
            _state = null;
        }

        public string GetScoreLabel()
        {
            return PlayerScore + " : " + EnemyScore;
        }

        private sealed class LocalMatchSessionState
        {
            public LocalMatchSessionState(int roundsToWin)
            {
                RoundsToWin = roundsToWin > 0 ? roundsToWin : 3;
                CurrentRound = 1;
                CurrentStatistics = new MatchStatistics();
                FinalResult = MatchResult.Playing;
                ShouldRaiseMatchStarted = true;
                IsActive = true;
            }

            public bool IsActive { get; private set; }
            public int PlayerScore { get; set; }
            public int EnemyScore { get; set; }
            public int CurrentRound { get; set; }
            public int RoundsToWin { get; set; }
            public MatchResult FinalResult { get; set; }
            public MatchStatistics CurrentStatistics { get; set; }
            public bool ShouldRaiseMatchStarted { get; set; }
            public bool IsStatisticsSaved { get; set; }
        }
    }
}
