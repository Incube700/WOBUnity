using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles
{
    public sealed class ProjectileFactory : MonoBehaviour
    {
        private ProjectileConfig _config;
        private SandboxGameplayEvents _gameplayEvents;

        public void Configure(ProjectileConfig config, SandboxGameplayEvents gameplayEvents)
        {
            _config = config;
            _gameplayEvents = gameplayEvents;
        }

        public Projectile Spawn(TankFacade owner, Transform muzzle)
        {
            if (owner == null)
            {
                throw new System.ArgumentNullException(nameof(owner));
            }

            if (muzzle == null)
            {
                throw new System.ArgumentNullException(nameof(muzzle));
            }

            if (_config == null)
            {
                throw new System.InvalidOperationException("ProjectileFactory is not configured.");
            }

            var direction = muzzle.forward;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = owner.transform.forward;
                direction.y = 0f;
            }

            direction.Normalize();

            var projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.name = "Projectile";
            projectileObject.transform.position = muzzle.position + direction * _config.SpawnOffset;
            projectileObject.transform.localScale = Vector3.one * (_config.Radius * 2f);

            TintProjectile(projectileObject);
            AddTrail(projectileObject);
            DisablePhysicsCollider(projectileObject);

            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.Configure(owner, _config, _gameplayEvents);
            projectile.Initialize(direction);

            _gameplayEvents?.RaiseProjectileSpawned(projectile, owner, projectileObject.transform.position, direction, _config.Speed);
            Debug.Log($"[SHOT] owner={owner.name} direction={direction} speed={_config.Speed}");
            return projectile;
        }

        private static void TintProjectile(GameObject projectileObject)
        {
            if (!projectileObject.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            renderer.material.color = new Color(1f, 0.86f, 0.05f);
        }

        private static void DisablePhysicsCollider(GameObject projectileObject)
        {
            if (projectileObject.TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }
        }

        private void AddTrail(GameObject projectileObject)
        {
            var trail = projectileObject.AddComponent<TrailRenderer>();
            trail.time = _config.TrailTime;
            trail.startWidth = _config.Radius * 0.8f;
            trail.endWidth = 0f;
            trail.startColor = new Color(1f, 0.95f, 0.25f, 1f);
            trail.endColor = new Color(1f, 0.25f, 0.05f, 0f);
            trail.material = new Material(Shader.Find("Sprites/Default"));
        }
    }
}
