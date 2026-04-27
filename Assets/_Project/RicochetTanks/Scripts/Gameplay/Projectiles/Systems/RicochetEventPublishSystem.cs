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
                entity.Ricochet.RicochetCount,
                entity.Ricochet.BouncesLeft,
                entity.MoveSpeed.Value,
                entity.Damage.Value,
                request.HitNormal);

            Debug.Log($"[BOUNCE] count={entity.Ricochet.RicochetCount} left={entity.Ricochet.BouncesLeft} speed={Format(entity.MoveSpeed.Value)} damage={Format(entity.Damage.Value)} normal={request.HitNormal}");
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
