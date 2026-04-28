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
            float currentPenetration,
            float kineticFactor,
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
                ? armor.ResolveHitInfo(projectileDirection, contactNormal, currentPenetration, kineticFactor)
                : CreateUnarmoredHitInfo(projectileDirection, contactNormal, currentPenetration, kineticFactor);

            if (IsRicochet(armorHit, contactNormal, armor))
            {
                result = HitResult.Ricochet;
                resolvedHit = new HitResolvedEvent(source, target, result, 0f, target.Health.CurrentHp, target.Health.MaxHp, armorHit);
                gameplayEvents?.RaiseHitResolved(resolvedHit);
                return true;
            }

            if (armor != null && currentPenetration < armorHit.EffectiveArmor)
            {
                result = HitResult.NoPen;
                resolvedHit = new HitResolvedEvent(source, target, result, 0f, target.Health.CurrentHp, target.Health.MaxHp, armorHit);
                gameplayEvents?.RaiseHitResolved(resolvedHit);
                return true;
            }

            result = HitResult.Penetrated;
            target.Health.ApplyDamage(damage);
            resolvedHit = new HitResolvedEvent(source, target, result, damage, target.Health.CurrentHp, target.Health.MaxHp, armorHit);
            gameplayEvents?.RaiseHitResolved(resolvedHit);
            return true;
        }

        private static bool IsRicochet(ArmorHitInfo armorHit, Vector3 contactNormal, TankArmor armor)
        {
            if (armor != null && armor.IsCornerHit(contactNormal))
            {
                return true;
            }

            return armorHit.ImpactDot > 0f && armorHit.HitAngle >= ResolveAutoRicochetAngle(armor);
        }

        private static float ResolveAutoRicochetAngle(TankArmor armor)
        {
            return armor != null ? armor.AutoRicochetAngle : DefaultAutoRicochetAngle;
        }

        private static ArmorHitInfo CreateUnarmoredHitInfo(
            Vector3 projectileDirection,
            Vector3 contactNormal,
            float currentPenetration,
            float kineticFactor)
        {
            var impactDot = projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f
                ? 1f
                : Vector3.Dot(-projectileDirection.normalized, contactNormal.normalized);
            var hitAngle = Mathf.Acos(Mathf.Clamp(impactDot, -1f, 1f)) * Mathf.Rad2Deg;
            return new ArmorHitInfo(ArmorZone.Unknown, 0f, 0f, hitAngle, impactDot, currentPenetration, kineticFactor);
        }
    }
}
