using System;
using UnityEngine;

namespace RicochetTanks.Statistics
{
    [Serializable]
    public sealed class MatchHistoryEntry
    {
        [SerializeField] private string _result;
        [SerializeField] private string _roundsScore;
        [SerializeField] private int _shots;
        [SerializeField] private int _hits;
        [SerializeField] private float _accuracyPercent;
        [SerializeField] private float _damageDealt;
        [SerializeField] private float _damageTaken;
        [SerializeField] private string _dateTime;

        public string Result { get { return _result; } set { _result = value; } }
        public string RoundsScore { get { return _roundsScore; } set { _roundsScore = value; } }
        public int Shots { get { return _shots; } set { _shots = value; } }
        public int Hits { get { return _hits; } set { _hits = value; } }
        public float AccuracyPercent { get { return _accuracyPercent; } set { _accuracyPercent = value; } }
        public float DamageDealt { get { return _damageDealt; } set { _damageDealt = value; } }
        public float DamageTaken { get { return _damageTaken; } set { _damageTaken = value; } }
        public string DateTime { get { return _dateTime; } set { _dateTime = value; } }
    }
}
