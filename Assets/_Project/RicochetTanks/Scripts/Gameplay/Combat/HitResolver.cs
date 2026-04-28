using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public static class HitResolver
    {
        private const float DefaultAutoRicochetAngle = 70f;

        public static bool TryResolveTankHit(
            Collider collider,
            TankFacade source,
            float damage,
            int penetration,
            bool canHitSource,
            Vector3 projectileDirection,
            Vector3 contactNormal,
            SandboxGameplayEvents gameplayEvents,
            out TankFacade target,
            out HitResult result,
            out HitResolvedEvent resolvedHit)
        {
            target = collider.GetComponentInParent<TankFacade>();
            result = HitResult.NoPen;
            resolvedHit = default;

            if (target == null || target.Health == null || !target.Health.IsAlive)
            {
                return false;
            }

            if (target == source && !canHitSource)
            {
                return false;
            }

            var armor = target.GetComponent<TankArmor>();
            var armorHit = armor != null
                ? armor.ResolveHitInfo(projectileDirection, contactNormal, penetration)
                : CreateUnarmoredHitInfo(projectileDirection, contactNormal, penetration);

            if (IsRicochet(projectileDirection, contactNormal, armor))
            {
                result = HitResult.Ricochet;
                resolvedHit = new HitResolvedEvent(source, target, result, 0, target.Health.CurrentHp, target.Health.MaxHp, armorHit);
                gameplayEvents?.RaiseHitResolved(resolvedHit);
                return true;
            }

            if (armor != null && penetration < armorHit.EffectiveArmor)
            {
                result = HitResult.NoPen;
                resolvedHit = new HitResolvedEvent(source, target, result, 0, target.Health.CurrentHp, target.Health.MaxHp, armorHit);
                gameplayEvents?.RaiseHitResolved(resolvedHit);
                return true;
            }

            result = HitResult.Penetrated;
            target.Health.ApplyDamage(damage);
            resolvedHit = new HitResolvedEvent(source, target, result, damage, target.Health.CurrentHp, target.Health.MaxHp, armorHit);
            gameplayEvents?.RaiseHitResolved(resolvedHit);
            return true;
        }

        private static bool IsRicochet(Vector3 projectileDirection, Vector3 contactNormal, TankArmor armor)
        {
            if (projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f)
            {
                return false;
            }

            var incomingDot = CalculateImpactDot(projectileDirection, contactNormal);
            var autoRicochetAngle = armor != null ? armor.AutoRicochetAngle : DefaultAutoRicochetAngle;
            var ricochetDotThreshold = Mathf.Cos(autoRicochetAngle * Mathf.Deg2Rad);
            return incomingDot > 0f && incomingDot <= ricochetDotThreshold;
        }

        private static ArmorHitInfo CreateUnarmoredHitInfo(Vector3 projectileDirection, Vector3 contactNormal, int penetration)
        {
            var impactDot = CalculateImpactDot(projectileDirection, contactNormal);
            var hitAngle = Mathf.Acos(Mathf.Clamp(impactDot, -1f, 1f)) * Mathf.Rad2Deg;
            return new ArmorHitInfo(ArmorZone.Unknown, 0f, 0f, hitAngle, impactDot, penetration);
        }

        private static float CalculateImpactDot(Vector3 projectileDirection, Vector3 contactNormal)
        {
            return projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f
                ? 1f
                : Vector3.Dot(-projectileDirection.normalized, contactNormal.normalized);
        }
    }
}
