using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Projectile Config", fileName = "ProjectileConfig")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _speed = 22f;
        [SerializeField] private float _bounceSpeedMultiplier = 0.78f;
        [SerializeField] private float _cooldown = 0.8f;
        [SerializeField] private float _safeTime = 0.15f;
        [SerializeField] private float _lifetime = 8f;
        [SerializeField] private float _minSpeed = 5f;
        [SerializeField] private float _radius = 0.18f;
        [SerializeField] private float _spawnOffset = 0.35f;
        [SerializeField] private float _positionCorrectionSkin = 0.01f;
        [SerializeField] private float _trailTime = 0.25f;
        [SerializeField] private float _damage = 110f;
        [SerializeField] private float _damageMultiplierPerBounce = 0.75f;
        [SerializeField] private int _maxRicochets = 3;
        [SerializeField] private int _penetration = 45;
        [SerializeField] private float _kineticFactor = 1f;
        [SerializeField] private LayerMask _reflectableMask = -1;
        [SerializeField] private LayerMask _hittableMask = -1;

        public GameObject ProjectilePrefab => _projectilePrefab;
        public float Speed => _speed;
        public float BounceSpeedMultiplier => _bounceSpeedMultiplier;
        public float Cooldown => _cooldown;
        public float SafeTime => _safeTime;
        public float Lifetime => _lifetime;
        public float MinSpeed => _minSpeed;
        public float Radius => _radius;
        public float SpawnOffset => _spawnOffset;
        public float PositionCorrectionSkin => _positionCorrectionSkin;
        public float TrailTime => _trailTime;
        public float Damage => _damage;
        public float BaseDamage => _damage;
        public float DamageMultiplierPerBounce => _damageMultiplierPerBounce;
        public int MaxRicochets => _maxRicochets;
        public int Penetration => _penetration;
        public int BasePenetration => _penetration;
        public float KineticFactor => Mathf.Max(0f, _kineticFactor);
        public LayerMask ReflectableMask => _reflectableMask;
        public LayerMask HittableMask => _hittableMask;
        public int CollisionMask => _reflectableMask.value | _hittableMask.value;
    }
}
