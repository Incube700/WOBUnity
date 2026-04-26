using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public static class HitResolver
    {
        public static bool TryApplyDamage(Collider collider, TankFacade source, int damage)
        {
            var target = collider.GetComponentInParent<TankFacade>();
            if (target == null || target == source || target.Health == null || !target.Health.IsAlive)
            {
                return false;
            }

            target.Health.ApplyDamage(damage);
            return true;
        }
    }
}
