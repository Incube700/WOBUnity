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
            out float appliedDamage,
            out ArmorHitInfo armorHit)
        {
            target = collider.GetComponentInParent<TankFacade>();
            result = HitResult.NoPen;
            appliedDamage = 0f;
            armorHit = new ArmorHitInfo(ArmorZone.Unknown, 0, 0, 0, currentPenetration, kineticFactor);

            if (target == null || target.Health == null || !target.Health.IsAlive)
            {
                return false;
            }

            if (target == source && !canHitSource)
            {
                return false;
            }

            var armor = target.GetComponent<TankArmor>();
            armorHit = armor != null
                ? armor.ResolveHitInfo(projectileDirection, contactNormal, currentPenetration, kineticFactor)
                : new ArmorHitInfo(ArmorZone.Unknown, 0, 0, CalculateHitAngle(projectileDirection, contactNormal), currentPenetration, kineticFactor);

            if (IsRicochet(armorHit, contactNormal, armor))
            {
                result = HitResult.Ricochet;
                gameplayEvents?.RaiseHitResolved(new HitResolvedEvent(source, target, result, 0, target.Health.CurrentHp, target.Health.MaxHp, armorHit));
                return true;
            }

            if (armor != null && currentPenetration < armorHit.EffectiveArmor)
            {
                result = HitResult.NoPen;
                gameplayEvents?.RaiseHitResolved(new HitResolvedEvent(source, target, result, 0, target.Health.CurrentHp, target.Health.MaxHp, armorHit));
                return true;
            }

            result = HitResult.Penetrated;
            appliedDamage = damage;
            target.Health.ApplyDamage(appliedDamage);
            gameplayEvents?.RaiseHitResolved(new HitResolvedEvent(source, target, result, appliedDamage, target.Health.CurrentHp, target.Health.MaxHp, armorHit));
            return true;
        }

        private static bool IsRicochet(ArmorHitInfo armorHit, Vector3 contactNormal, TankArmor armor)
        {
            if (armor != null && armor.IsCornerHit(contactNormal))
            {
                return true;
            }

            return armorHit.HitAngle >= ResolveAutoRicochetAngle(armor);
        }

        private static float ResolveAutoRicochetAngle(TankArmor armor)
        {
            return armor != null ? armor.AutoRicochetAngle : DefaultAutoRicochetAngle;
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
