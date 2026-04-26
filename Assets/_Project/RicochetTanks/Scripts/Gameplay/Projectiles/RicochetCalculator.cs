using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles
{
    public static class RicochetCalculator
    {
        public static Vector3 Reflect(Vector3 direction, Vector3 normal)
        {
            var reflected = Vector3.Reflect(direction.normalized, normal.normalized);
            reflected.y = 0f;
            return reflected.normalized;
        }
    }
}
