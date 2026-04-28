using RicochetTanks.Gameplay.Combat;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class ProjectileHitDetectionSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var request = entity.RicochetRequest;

            if (HitResolver.TryResolveTankHit(
                    request.Collider,
                    entity.Owner,
                    entity.Damage.Value,
                    entity.Damage.Penetration,
                    entity.CanHitOwner,
                    request.IncomingDirection,
                    request.HitNormal,
                    entity.GameplayEvents,
                    out var target,
                    out var hitResult,
                    out var resolvedHit))
            {
                entity.GameplayEvents?.RaiseCombatFeedbackRequested(
                    request.HitPoint,
                    request.HitNormal,
                    resolvedHit.Source,
                    resolvedHit.Target,
                    resolvedHit.Result,
                    resolvedHit.Damage,
                    resolvedHit.CurrentHp,
                    resolvedHit.MaxHp,
                    resolvedHit.ArmorHit);

                if (hitResult == HitResult.Ricochet)
                {
                    if (!entity.HasBouncesLeft)
                    {
                        entity.RequestDestroy();
                        ClearRicochetRequest(entity);
                        return;
                    }

                    Debug.Log($"[HIT] target={target.name} result={hitResult} damage=0 hp={Format(target.Health.CurrentHp)}/{Format(target.Health.MaxHp)}");
                    return;
                }

                var resolvedDamage = hitResult == HitResult.Penetrated ? entity.Damage.Value : 0f;
                Debug.Log($"[HIT] target={target.name} result={hitResult} damage={Format(resolvedDamage)} hp={Format(target.Health.CurrentHp)}/{Format(target.Health.MaxHp)}");
                entity.RequestDestroy();
                ClearRicochetRequest(entity);
                return;
            }

            if (!entity.HasBouncesLeft)
            {
                entity.RequestDestroy();
                ClearRicochetRequest(entity);
            }
        }

        private static void ClearRicochetRequest(ProjectileEntity entity)
        {
            var request = entity.RicochetRequest;
            request.Clear();
            entity.RicochetRequest = request;
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
