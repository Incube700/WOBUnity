using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Projectiles.Components;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles
{
    public sealed class ProjectileEntity
    {
        public ProjectileEntity(
            Projectile projectile,
            Transform transform,
            GameObject gameObject,
            TankFacade owner,
            ProjectileConfig config,
            SandboxGameplayEvents gameplayEvents)
        {
            Projectile = projectile;
            Transform = transform;
            GameObject = gameObject;
            Owner = owner;
            GameplayEvents = gameplayEvents;
            CollisionMask = config.CollisionMask;

            ProjectileTag = new ProjectileTagComponent();
            PreviousPosition = new PreviousPositionComponent(transform.position);
            MoveDirection = new MoveDirectionComponent(Vector3.forward);
            MoveSpeed = new MoveSpeedComponent(config.Speed);
            Damage = new DamageComponent(config.Damage, config.Penetration);
            Lifetime = new LifetimeComponent(Time.time, config.Lifetime, config.SafeTime);
            Ricochet = new RicochetComponent(
                config.MaxRicochets,
                config.Radius,
                config.Radius + config.PositionCorrectionSkin,
                config.BounceSpeedMultiplier,
                config.MinSpeed,
                config.DamageMultiplierPerBounce);
            RicochetRequest = new RicochetRequestComponent();

            OwnerColliders = owner != null ? owner.GetComponentsInChildren<Collider>() : null;
        }

        public Projectile Projectile { get; }
        public Transform Transform { get; }
        public GameObject GameObject { get; }
        public TankFacade Owner { get; }
        public SandboxGameplayEvents GameplayEvents { get; }
        public Collider[] OwnerColliders { get; }
        public int CollisionMask { get; }

        public ProjectileTagComponent ProjectileTag { get; }
        public PreviousPositionComponent PreviousPosition { get; set; }
        public MoveDirectionComponent MoveDirection { get; set; }
        public MoveSpeedComponent MoveSpeed { get; set; }
        public DamageComponent Damage { get; set; }
        public LifetimeComponent Lifetime { get; }
        public RicochetComponent Ricochet { get; set; }
        public RicochetRequestComponent RicochetRequest { get; set; }

        public bool IsDestroyRequested { get; private set; }
        public bool IsDestroyFinalized { get; private set; }
        public bool CanHitOwner => Time.time >= Lifetime.SpawnTime + Lifetime.SafeOwnerTime;
        public bool HasBouncesLeft => Ricochet.BouncesLeft > 0;

        public void InitializeDirection(Vector3 direction)
        {
            MoveDirection = new MoveDirectionComponent(NormalizePlanarDirection(direction, Transform.forward));
            UpdateRotation();
        }

        public void RequestDestroy()
        {
            IsDestroyRequested = true;
        }

        public void MarkDestroyFinalized()
        {
            IsDestroyFinalized = true;
        }

        public void UpdateRotation()
        {
            var direction = MoveDirection.Value;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
            {
                return;
            }

            Transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        public bool ShouldIgnoreHit(Collider hitCollider)
        {
            if (hitCollider == null)
            {
                return true;
            }

            if (hitCollider.transform == Transform || hitCollider.transform.IsChildOf(Transform))
            {
                return true;
            }

            if (CanHitOwner || OwnerColliders == null)
            {
                return false;
            }

            for (var index = 0; index < OwnerColliders.Length; index++)
            {
                if (OwnerColliders[index] == hitCollider)
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 NormalizePlanarDirection(Vector3 direction, Vector3 fallback)
        {
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.001f)
            {
                direction = fallback;
                direction.y = 0f;
            }

            return direction.sqrMagnitude < 0.001f ? Vector3.forward : direction.normalized;
        }
    }
}
