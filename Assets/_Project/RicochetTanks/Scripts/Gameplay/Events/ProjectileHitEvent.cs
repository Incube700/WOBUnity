using RicochetTanks.Gameplay.Projectiles;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct ProjectileHitEvent
    {
        public ProjectileHitEvent(Projectile projectile, Collider collider, Vector3 point, Vector3 normal, Vector3 direction)
        {
            Projectile = projectile;
            Collider = collider;
            Point = point;
            Normal = normal;
            Direction = direction;
        }

        public Projectile Projectile { get; }
        public Collider Collider { get; }
        public Vector3 Point { get; }
        public Vector3 Normal { get; }
        public Vector3 Direction { get; }
    }
}
