using UnityEngine;

namespace RicochetTanks.Gameplay.AI
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Enemy AI Config", fileName = "EnemyAiConfig")]
    public sealed class EnemyAiConfig : ScriptableObject
    {
        [SerializeField] private float _detectionDistance = 25f;
        [SerializeField] private float _preferredDistance = 8f;
        [SerializeField] private float _minDistance = 5f;
        [SerializeField] private float _maxDistance = 14f;
        [SerializeField] private float _fireDistance = 20f;
        [SerializeField] private float _fireCooldownMultiplier = 1f;
        [SerializeField] private float _aimAngleToShoot = 8f;
        [SerializeField] private float _repositionInterval = 2f;
        [SerializeField] private float _repositionDuration = 1f;
        [SerializeField] private float _obstacleRayDistance = 2.5f;
        [SerializeField] private float _obstacleAvoidTurn = 1f;
        [SerializeField] private float _moveThrottle = 0.75f;
        [SerializeField] private float _retreatThrottle = -0.4f;
        [SerializeField] private float _aimPredictionStrength;
        [SerializeField] private LayerMask _lineOfSightMask = ~0;
        [SerializeField] private LayerMask _obstacleMask = ~0;
        [SerializeField] private bool _debugLogs;

        public float DetectionDistance { get { return Mathf.Max(0f, _detectionDistance); } }
        public float PreferredDistance { get { return Mathf.Max(0f, _preferredDistance); } }
        public float MinDistance { get { return Mathf.Max(0f, _minDistance); } }
        public float MaxDistance { get { return Mathf.Max(_minDistance, _maxDistance); } }
        public float FireDistance { get { return Mathf.Max(0f, _fireDistance); } }
        public float FireCooldownMultiplier { get { return Mathf.Max(0.1f, _fireCooldownMultiplier); } }
        public float AimAngleToShoot { get { return Mathf.Max(0f, _aimAngleToShoot); } }
        public float RepositionInterval { get { return Mathf.Max(0.1f, _repositionInterval); } }
        public float RepositionDuration { get { return Mathf.Max(0.1f, _repositionDuration); } }
        public float ObstacleRayDistance { get { return Mathf.Max(0f, _obstacleRayDistance); } }
        public float ObstacleAvoidTurn { get { return Mathf.Clamp(_obstacleAvoidTurn, -1f, 1f); } }
        public float MoveThrottle { get { return Mathf.Clamp(_moveThrottle, -1f, 1f); } }
        public float RetreatThrottle { get { return Mathf.Clamp(_retreatThrottle, -1f, 1f); } }
        public float AimPredictionStrength { get { return Mathf.Max(0f, _aimPredictionStrength); } }
        public LayerMask LineOfSightMask { get { return _lineOfSightMask; } }
        public LayerMask ObstacleMask { get { return _obstacleMask; } }
        public bool DebugLogs { get { return _debugLogs; } }
    }
}
