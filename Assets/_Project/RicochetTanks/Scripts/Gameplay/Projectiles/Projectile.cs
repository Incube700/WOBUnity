using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 16f;
        [SerializeField] private int _damage = 35;
        [SerializeField] private int _maxRicochets = 3;
        [SerializeField] private float _bounceSpeedMultiplier = 0.85f;
        [SerializeField] private float _safeTime = 0.15f;
        [SerializeField] private float _lifetime = 8f;
        [SerializeField] private float _minSpeed = 5f;
        [SerializeField] private float _radius = 0.18f;
        [SerializeField] private int _penetration = 100;

        private TankFacade _owner;
        private SandboxGameplayEvents _gameplayEvents;
        private Collider[] _ownerColliders;
        private Vector3 _direction;
        private int _ricochetCount;
        private float _currentSpeed;
        private float _spawnTime;

        public void Configure(TankFacade owner, ProjectileConfig config, SandboxGameplayEvents gameplayEvents)
        {
            _owner = owner;
            _gameplayEvents = gameplayEvents;

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
            _minSpeed = config.MinSpeed;
            _radius = config.Radius;
            _penetration = config.Penetration;
        }

        public void Initialize(Vector3 direction)
        {
            _direction = direction.normalized;
            _currentSpeed = _speed;
            _spawnTime = Time.time;
            _ownerColliders = _owner != null ? _owner.GetComponentsInChildren<Collider>() : null;
        }

        private void FixedUpdate()
        {
            if (Time.time >= _spawnTime + _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            Move(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            var distance = _currentSpeed * deltaTime;
            var origin = transform.position;

            if (!TryGetFirstValidHit(origin, distance, out var hit))
            {
                transform.position = origin + _direction * distance;
                return;
            }

            transform.position = hit.point + hit.normal * (_radius + 0.01f);
            ResolveHit(hit);
        }

        private bool TryGetFirstValidHit(Vector3 origin, float distance, out RaycastHit selectedHit)
        {
            selectedHit = default;
            var hits = Physics.SphereCastAll(origin, _radius, _direction, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
            {
                return false;
            }

            SortHitsByDistance(hits);

            for (var index = 0; index < hits.Length; index++)
            {
                if (ShouldIgnoreHit(hits[index].collider))
                {
                    continue;
                }

                selectedHit = hits[index];
                return true;
            }

            return false;
        }

        private bool ShouldIgnoreHit(Collider hitCollider)
        {
            if (hitCollider == null)
            {
                return true;
            }

            if (Time.time >= _spawnTime + _safeTime || _ownerColliders == null)
            {
                return false;
            }

            for (var index = 0; index < _ownerColliders.Length; index++)
            {
                if (_ownerColliders[index] == hitCollider)
                {
                    return true;
                }
            }

            return false;
        }

        private void ResolveHit(RaycastHit hit)
        {
            var normal = hit.normal;
            _gameplayEvents?.RaiseProjectileHit(this, hit.collider, hit.point, normal, _direction);

            var canHitOwner = Time.time >= _spawnTime + _safeTime;
            if (HitResolver.TryResolveTankHit(hit.collider, _owner, _damage, _penetration, canHitOwner, _direction, normal, _gameplayEvents, out var target, out var hitResult))
            {
                if (hitResult == HitResult.Ricochet)
                {
                    if (_ricochetCount >= _maxRicochets)
                    {
                        Destroy(gameObject);
                        return;
                    }

                    Bounce(normal);
                    Debug.Log($"[HIT] target={target.name} result={hitResult} damage=0 hp={target.Health.CurrentHp}/{target.Health.MaxHp}");
                    return;
                }

                var resolvedDamage = hitResult == HitResult.Penetrated ? _damage : 0;
                Debug.Log($"[HIT] target={target.name} result={hitResult} damage={resolvedDamage} hp={target.Health.CurrentHp}/{target.Health.MaxHp}");
                Destroy(gameObject);
                return;
            }

            if (_ricochetCount >= _maxRicochets)
            {
                Destroy(gameObject);
                return;
            }

            Bounce(normal);
        }

        private void Bounce(Vector3 normal)
        {
            _direction = RicochetCalculator.Reflect(_direction, normal);
            _ricochetCount++;
            _currentSpeed = Mathf.Max(_minSpeed, _currentSpeed * _bounceSpeedMultiplier);
            _gameplayEvents?.RaiseProjectileBounced(this, _ricochetCount, _currentSpeed, normal);
            Debug.Log($"[BOUNCE] count={_ricochetCount} speed={_currentSpeed} normal={normal}");
        }

        private static void SortHitsByDistance(RaycastHit[] hits)
        {
            for (var index = 1; index < hits.Length; index++)
            {
                var current = hits[index];
                var previousIndex = index - 1;

                while (previousIndex >= 0 && hits[previousIndex].distance > current.distance)
                {
                    hits[previousIndex + 1] = hits[previousIndex];
                    previousIndex--;
                }

                hits[previousIndex + 1] = current;
            }
        }
    }
}
