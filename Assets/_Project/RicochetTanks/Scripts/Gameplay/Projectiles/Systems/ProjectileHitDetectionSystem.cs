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

                LogArmorHit(target, hitResult, resolvedHit, request.HitPoint, request.HitNormal, request.IncomingDirection);

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
                $"penetration={armorHit.Penetration} armor={Format(armorHit.Armor)} effectiveArmor={Format(armorHit.EffectiveArmor)} damage={Format(resolvedHit.Damage)} " +
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
