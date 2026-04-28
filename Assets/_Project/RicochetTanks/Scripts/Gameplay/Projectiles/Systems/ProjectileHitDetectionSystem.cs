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
                    entity.Damage.CurrentPenetration,
                    entity.Damage.KineticFactor,
                    entity.CanHitOwner,
                    request.IncomingDirection,
                    request.HitNormal,
                    entity.GameplayEvents,
                    out var target,
                    out var hitResult,
                    out var appliedDamage,
                    out var armorHit))
            {
                if (hitResult == HitResult.Ricochet || hitResult == HitResult.NoPen)
                {
                    if (entity.GameplayEvents != null && entity.GameplayEvents.ShouldLogHits)
                    {
                        Debug.Log(hitResult == HitResult.Ricochet
                            ? $"[HIT] target={target.name} result=Ricochet angle={Format(armorHit.HitAngle)}"
                            : FormatNoDamageHit(target.name, armorHit));
                    }

                    if (!entity.HasBouncesLeft)
                    {
                        entity.RequestDestroy();
                        ClearRicochetRequest(entity);
                        return;
                    }

                    return;
                }

                if (entity.GameplayEvents != null && entity.GameplayEvents.ShouldLogHits)
                {
                    Debug.Log(FormatDamageHit(target.name, hitResult, appliedDamage, armorHit, target.Health.CurrentHp, target.Health.MaxHp));
                }

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

        private static string FormatHitResult(HitResult hitResult)
        {
            if (hitResult == HitResult.ReducedDamage)
            {
                return "Reduced";
            }

            return hitResult == HitResult.NoPen ? "NoPenetration" : hitResult.ToString();
        }

        private static string FormatNoDamageHit(string targetName, ArmorHitInfo armorHit)
        {
            return $"[HIT] target={targetName} zone={armorHit.Zone} result=NoPenetration penetration={Format(armorHit.Penetration)} armor={Format(armorHit.Armor)} effectiveArmor={Format(armorHit.EffectiveArmor)} damage=0";
        }

        private static string FormatDamageHit(string targetName, HitResult hitResult, float appliedDamage, ArmorHitInfo armorHit, float currentHp, float maxHp)
        {
            return $"[HIT] target={targetName} zone={armorHit.Zone} result={FormatHitResult(hitResult)} penetration={Format(armorHit.Penetration)} armor={Format(armorHit.Armor)} effectiveArmor={Format(armorHit.EffectiveArmor)} damage={Format(appliedDamage)} hp={Format(currentHp)}/{Format(maxHp)}";
        }
    }
}
