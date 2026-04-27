using RicochetTanks.Gameplay.Tanks;
using System;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public static class HitResolver
    {
        public static event Action<HitResolvedEvent> HitResolved;

        public static bool TryApplyDamage(Collider collider, TankFacade source, int damage, bool canHitSource, out TankFacade target)
        {
            target = collider.GetComponentInParent<TankFacade>();
            if (target == null || target.Health == null || !target.Health.IsAlive)
            {
                return false;
            }

            if (target == source && !canHitSource)
            {
                return false;
            }

            target.Health.ApplyDamage(damage);
            HitResolved?.Invoke(new HitResolvedEvent(source, target, HitResult.Penetrated, damage, target.Health.CurrentHp, target.Health.MaxHp));
            return true;
        }
    }
}
