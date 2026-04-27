using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Projectile Config", fileName = "ProjectileConfig")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        [SerializeField] private float _speed = 22f;
        [SerializeField] private float _bounceSpeedMultiplier = 0.85f;
        [SerializeField] private float _cooldown = 0.8f;
        [SerializeField] private float _safeTime = 0.15f;
        [SerializeField] private float _lifetime = 8f;
        [SerializeField] private float _minSpeed = 5f;
        [SerializeField] private float _radius = 0.18f;
        [SerializeField] private float _spawnOffset = 0.35f;
        [SerializeField] private float _trailTime = 0.25f;
        [SerializeField] private int _damage = 35;
        [SerializeField] private int _maxRicochets = 3;
        [SerializeField] private int _penetration = 100;

        public float Speed => _speed;
        public float BounceSpeedMultiplier => _bounceSpeedMultiplier;
        public float Cooldown => _cooldown;
        public float SafeTime => _safeTime;
        public float Lifetime => _lifetime;
        public float MinSpeed => _minSpeed;
        public float Radius => _radius;
        public float SpawnOffset => _spawnOffset;
        public float TrailTime => _trailTime;
        public int Damage => _damage;
        public int MaxRicochets => _maxRicochets;
        public int Penetration => _penetration;
    }
}
