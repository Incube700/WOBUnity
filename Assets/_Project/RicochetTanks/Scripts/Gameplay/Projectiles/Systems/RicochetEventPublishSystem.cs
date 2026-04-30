using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetEventPublishSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var request = entity.RicochetRequest;
            entity.GameplayEvents?.RaiseProjectileBounced(
                entity.Projectile,
                entity.Owner,
                entity.Ricochet.RicochetCount,
                entity.Ricochet.BouncesLeft,
                entity.MoveSpeed.Value,
                entity.Damage.Value,
                request.HitNormal);

            if (entity.GameplayEvents != null && entity.GameplayEvents.ShouldLogBounces)
            {
                Debug.Log($"[BOUNCE] source={ResolveSource(request.Collider)} count={entity.Ricochet.RicochetCount} left={entity.Ricochet.BouncesLeft} speed={Format(entity.MoveSpeed.Value)} damage={Format(entity.Damage.Value)} normal={request.HitNormal}");
            }
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }

        private static string ResolveSource(Collider collider)
        {
            return collider != null && collider.GetComponentInParent<TankFacade>() != null ? "Tank" : "World";
        }
    }
}
