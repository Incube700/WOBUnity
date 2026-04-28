using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class ProjectileHitDetectionSystem : IProjectileFixedSystem
    {
        private const float DebugRayDuration = 1.25f;
        private const float DebugRayLength = 1.5f;

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

                if (entity.GameplayEvents != null && entity.GameplayEvents.ShouldLogHits)
                {
                    LogArmorHit(target, hitResult, resolvedHit, request.HitPoint, request.HitNormal, request.IncomingDirection);
                    Debug.Log(FormatHit(target.name, hitResult, resolvedHit));
                }

                if (hitResult == HitResult.Ricochet)
                {
                    if (!entity.HasBouncesLeft)
                    {
                        entity.RequestDestroy();
                        ClearRicochetRequest(entity);
                        return;
                    }

                    return;
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

        private static string FormatHit(string targetName, HitResult hitResult, HitResolvedEvent resolvedHit)
        {
            var armorHit = resolvedHit.ArmorHit;
            return $"[HIT] target={targetName} zone={armorHit.Zone} result={FormatHitResult(hitResult)} penetration={Format(armorHit.Penetration)} armor={Format(armorHit.Armor)} effectiveArmor={Format(armorHit.EffectiveArmor)} damage={Format(resolvedHit.Damage)} hp={Format(resolvedHit.CurrentHp)}/{Format(resolvedHit.MaxHp)}";
        }

        private static string FormatHitResult(HitResult hitResult)
        {
            if (hitResult == HitResult.ReducedDamage)
            {
                return "Reduced";
            }

            return hitResult == HitResult.NoPen ? "NoPenetration" : hitResult.ToString();
        }

        private static void LogArmorHit(
            TankFacade target,
            HitResult result,
            HitResolvedEvent resolvedHit,
            Vector3 hitPoint,
            Vector3 hitNormal,
            Vector3 incomingDirection)
        {
            var armorHit = resolvedHit.ArmorHit;
            var localNormal = target != null ? target.transform.InverseTransformDirection(hitNormal) : hitNormal;
            var targetName = target != null ? target.name : "null";

            Debug.Log(
                $"[ARMOR] target={targetName} zone={armorHit.Zone} result={result} angle={Format(armorHit.HitAngle)} dot={Format(armorHit.ImpactDot)} " +
                $"penetration={Format(armorHit.Penetration)} armor={Format(armorHit.Armor)} effectiveArmor={Format(armorHit.EffectiveArmor)} damage={Format(resolvedHit.Damage)} " +
                $"incoming={FormatVector(incomingDirection)} normal={FormatVector(hitNormal)} localNormal={FormatVector(localNormal)}");

            DrawArmorDebugRays(hitPoint, hitNormal, incomingDirection);
        }

        private static void DrawArmorDebugRays(Vector3 hitPoint, Vector3 hitNormal, Vector3 incomingDirection)
        {
            if (hitNormal.sqrMagnitude >= 0.001f)
            {
                Debug.DrawRay(hitPoint, hitNormal.normalized * DebugRayLength, Color.blue, DebugRayDuration);
            }

            if (incomingDirection.sqrMagnitude < 0.001f)
            {
                return;
            }

            var incoming = incomingDirection.normalized;
            Debug.DrawRay(hitPoint, incoming * DebugRayLength, Color.red, DebugRayDuration);

            if (hitNormal.sqrMagnitude >= 0.001f)
            {
                var reflected = Vector3.Reflect(incoming, hitNormal.normalized);
                Debug.DrawRay(hitPoint, reflected * DebugRayLength, Color.green, DebugRayDuration);
            }
        }

        private static string FormatVector(Vector3 value)
        {
            return $"({Format(value.x)},{Format(value.y)},{Format(value.z)})";
        }
    }
}
