using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Debug Log Config", fileName = "DebugLogConfig")]
    public sealed class DebugLogConfig : ScriptableObject
    {
        [SerializeField] private bool _logShots;
        [SerializeField] private bool _logHits;
        [SerializeField] private bool _logBounces;
        [SerializeField] private bool _logRounds = true;

        public bool LogShots => _logShots;
        public bool LogHits => _logHits;
        public bool LogBounces => _logBounces;
        public bool LogRounds => _logRounds;
    }
}
