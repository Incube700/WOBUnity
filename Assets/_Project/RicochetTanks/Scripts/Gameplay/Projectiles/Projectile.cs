using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 16f;
        [SerializeField] private int _damage = 34;
        [SerializeField] private int _maxRicochets = 3;
        [SerializeField] private float _bounceSpeedMultiplier = 0.78f;
        [SerializeField] private float _safeTime = 0.18f;
        [SerializeField] private float _lifetime = 8f;

        private TankFacade _owner;
        private Rigidbody _rigidbody;
        private Collider _collider;
        private Collider[] _ownerColliders;
        private Vector3 _direction;
        private int _ricochetCount;
        private float _currentSpeed;
        private float _spawnTime;
        private bool _hasRestoredOwnerCollisions;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void Configure(TankFacade owner, ProjectileConfig config)
        {
            _owner = owner;

            if (config == null)
            {
                return;
            }

            _damage = config.Damage;
            _maxRicochets = config.MaxRicochets;
            _speed = config.Speed;
            _bounceSpeedMultiplier = config.BounceSpeedMultiplier;
            _safeTime = config.SafeTime;
            _lifetime = config.Lifetime;
        }

        public void Initialize(Vector3 direction)
        {
            _direction = direction.normalized;
            _currentSpeed = _speed;
            _spawnTime = Time.time;
            _ownerColliders = _owner != null ? _owner.GetComponentsInChildren<Collider>() : null;
            SetOwnerCollisionIgnored(true);
            ApplyVelocity();
        }

        private void FixedUpdate()
        {
            RestoreOwnerCollisionsAfterSafeTime();
            ApplyVelocity();
        }

        private void Update()
        {
            if (Time.time >= _spawnTime + _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            SetOwnerCollisionIgnored(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            RestoreOwnerCollisionsAfterSafeTime();

            var canHitOwner = Time.time >= _spawnTime + _safeTime;
            if (HitResolver.TryApplyDamage(other.collider, _owner, _damage, canHitOwner, out var target))
            {
                Debug.Log($"[HIT] target={target.name} damage={_damage} hp={target.Health.CurrentHp}/{target.Health.MaxHp}");
                Destroy(gameObject);
                return;
            }

            if (_ricochetCount >= _maxRicochets || other.contactCount == 0)
            {
                Destroy(gameObject);
                return;
            }

            var normal = other.GetContact(0).normal;
            _direction = RicochetCalculator.Reflect(_direction, normal);
            _ricochetCount++;
            _currentSpeed *= _bounceSpeedMultiplier;
            ApplyVelocity();
            Debug.Log($"[BOUNCE] count={_ricochetCount} speed={_currentSpeed} normal={normal}");
        }

        private void ApplyVelocity()
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.linearVelocity = _direction * _currentSpeed;
        }

        private void RestoreOwnerCollisionsAfterSafeTime()
        {
            if (_hasRestoredOwnerCollisions || Time.time < _spawnTime + _safeTime)
            {
                return;
            }

            SetOwnerCollisionIgnored(false);
            _hasRestoredOwnerCollisions = true;
        }

        private void SetOwnerCollisionIgnored(bool isIgnored)
        {
            if (_collider == null || _ownerColliders == null)
            {
                return;
            }

            for (var index = 0; index < _ownerColliders.Length; index++)
            {
                if (_ownerColliders[index] != null)
                {
                    Physics.IgnoreCollision(_collider, _ownerColliders[index], isIgnored);
                }
            }
        }
    }
}
