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
            int damage,
            int penetration,
            bool canHitSource,
            Vector3 projectileDirection,
            Vector3 contactNormal,
            SandboxGameplayEvents gameplayEvents,
            out TankFacade target,
            out HitResult result)
        {
            target = collider.GetComponentInParent<TankFacade>();
            result = HitResult.NoPen;

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
                : new ArmorHitInfo(ArmorZone.Unknown, 0, CalculateHitAngle(projectileDirection, contactNormal), penetration);

            if (IsRicochet(projectileDirection, contactNormal, armor))
            {
                result = HitResult.Ricochet;
                gameplayEvents?.RaiseHitResolved(new HitResolvedEvent(source, target, result, 0, target.Health.CurrentHp, target.Health.MaxHp, armorHit));
                return true;
            }

            if (armor != null && penetration < armorHit.EffectiveArmor)
            {
                result = HitResult.NoPen;
                gameplayEvents?.RaiseHitResolved(new HitResolvedEvent(source, target, result, 0, target.Health.CurrentHp, target.Health.MaxHp, armorHit));
                return true;
            }

            result = HitResult.Penetrated;
            target.Health.ApplyDamage(damage);
            gameplayEvents?.RaiseHitResolved(new HitResolvedEvent(source, target, result, damage, target.Health.CurrentHp, target.Health.MaxHp, armorHit));
            return true;
        }

        private static bool IsRicochet(Vector3 projectileDirection, Vector3 contactNormal, TankArmor armor)
        {
            projectileDirection.y = 0f;
            contactNormal.y = 0f;

            if (projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f)
            {
                return false;
            }

            var incomingDot = Vector3.Dot(-projectileDirection.normalized, contactNormal.normalized);
            var autoRicochetAngle = armor != null ? armor.AutoRicochetAngle : DefaultAutoRicochetAngle;
            var ricochetDotThreshold = Mathf.Cos(autoRicochetAngle * Mathf.Deg2Rad);
            return incomingDot > 0f && incomingDot <= ricochetDotThreshold;
        }

        private static float CalculateHitAngle(Vector3 projectileDirection, Vector3 contactNormal)
        {
            projectileDirection.y = 0f;
            contactNormal.y = 0f;

            if (projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f)
            {
                return 0f;
            }

            return Vector3.Angle(-projectileDirection.normalized, contactNormal.normalized);
        }
    }
}
