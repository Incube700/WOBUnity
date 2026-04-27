using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Projectile Config", fileName = "ProjectileConfig")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        [SerializeField] private float _speed = 22f;
        [SerializeField] private float _bounceSpeedMultiplier = 0.78f;
        [SerializeField] private float _cooldown = 0.35f;
        [SerializeField] private float _safeTime = 0.18f;
        [SerializeField] private float _lifetime = 8f;
        [SerializeField] private float _radius = 0.18f;
        [SerializeField] private float _spawnOffset = 0.35f;
        [SerializeField] private float _trailTime = 0.25f;
        [SerializeField] private int _damage = 34;
        [SerializeField] private int _maxRicochets = 3;

        public float Speed => _speed;
        public float BounceSpeedMultiplier => _bounceSpeedMultiplier;
        public float Cooldown => _cooldown;
        public float SafeTime => _safeTime;
        public float Lifetime => _lifetime;
        public float Radius => _radius;
        public float SpawnOffset => _spawnOffset;
        public float TrailTime => _trailTime;
        public int Damage => _damage;
        public int MaxRicochets => _maxRicochets;
    }
}
