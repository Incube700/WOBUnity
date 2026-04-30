using System;
using System.Collections.Generic;
using RicochetTanks.Gameplay.Combat;
using UnityEngine;

namespace RicochetTanks.Statistics
{
    [Serializable]
    public sealed class PlayerStatisticsData
    {
        private const int MaxRecentMatches = 10;

        [SerializeField] private int _totalMatches;
        [SerializeField] private int _wins;
        [SerializeField] private int _losses;
        [SerializeField] private int _draws;
        [SerializeField] private int _shotsFired;
        [SerializeField] private int _tankHits;
        [SerializeField] private int _penetrations;
        [SerializeField] private int _noPenetrations;
        [SerializeField] private int _ricochets;
        [SerializeField] private int _selfHits;
        [SerializeField] private float _damageDealt;
        [SerializeField] private float _damageTaken;
        [SerializeField] private List<MatchHistoryEntry> _recentMatches = new List<MatchHistoryEntry>();

        public int TotalMatches { get { return _totalMatches; } }
        public int Wins { get { return _wins; } }
        public int Losses { get { return _losses; } }
        public int Draws { get { return _draws; } }
        public int ShotsFired { get { return _shotsFired; } }
        public int TankHits { get { return _tankHits; } }
        public int Penetrations { get { return _penetrations; } }
        public int NoPenetrations { get { return _noPenetrations; } }
        public int Ricochets { get { return _ricochets; } }
        public int SelfHits { get { return _selfHits; } }
        public float DamageDealt { get { return _damageDealt; } }
        public float DamageTaken { get { return _damageTaken; } }
        public IReadOnlyList<MatchHistoryEntry> RecentMatches { get { return _recentMatches; } }

        public float WinRatePercent
        {
            get { return _totalMatches > 0 ? (float)_wins / _totalMatches * 100f : 0f; }
        }

        public float AccuracyPercent
        {
            get { return _shotsFired > 0 ? (float)_tankHits / _shotsFired * 100f : 0f; }
        }

        public float PenetrationRatePercent
        {
            get { return _tankHits > 0 ? (float)_penetrations / _tankHits * 100f : 0f; }
        }

        public float EfficiencyDamagePerShot
        {
            get { return _shotsFired > 0 ? _damageDealt / _shotsFired : 0f; }
        }

        public void ApplyMatch(MatchStatistics match, string roundsScore)
        {
            if (match == null)
            {
                return;
            }

            _totalMatches++;
            if (match.MatchResult == MatchResult.PlayerWins)
            {
                _wins++;
            }
            else if (match.MatchResult == MatchResult.EnemyWins)
            {
                _losses++;
            }
            else
            {
                _draws++;
            }

            _shotsFired += match.ShotsFired;
            _tankHits += match.TankHits;
            _penetrations += match.Penetrations;
            _noPenetrations += match.NoPenetrations;
            _ricochets += match.Ricochets;
            _selfHits += match.SelfHits;
            _damageDealt += match.DamageDealt;
            _damageTaken += match.DamageTaken;
            AddRecentMatch(match, roundsScore);
        }

        private void AddRecentMatch(MatchStatistics match, string roundsScore)
        {
            if (_recentMatches == null)
            {
                _recentMatches = new List<MatchHistoryEntry>();
            }

            _recentMatches.Insert(0, new MatchHistoryEntry
            {
                Result = FormatResult(match.MatchResult),
                RoundsScore = roundsScore,
                Shots = match.ShotsFired,
                Hits = match.TankHits,
                AccuracyPercent = match.AccuracyPercent,
                DamageDealt = match.DamageDealt,
                DamageTaken = match.DamageTaken,
                DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            });

            while (_recentMatches.Count > MaxRecentMatches)
            {
                _recentMatches.RemoveAt(_recentMatches.Count - 1);
            }
        }

        private static string FormatResult(MatchResult result)
        {
            if (result == MatchResult.PlayerWins)
            {
                return "Win";
            }

            if (result == MatchResult.EnemyWins)
            {
                return "Loss";
            }

            return "Draw";
        }
    }
}
