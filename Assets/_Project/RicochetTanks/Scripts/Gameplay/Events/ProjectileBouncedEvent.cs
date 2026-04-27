using RicochetTanks.Gameplay.Projectiles;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct ProjectileBouncedEvent
    {
        public ProjectileBouncedEvent(Projectile projectile, int ricochetCount, float speed, Vector3 normal)
        {
            Projectile = projectile;
            RicochetCount = ricochetCount;
            Speed = speed;
            Normal = normal;
        }

        public Projectile Projectile { get; }
        public int RicochetCount { get; }
        public float Speed { get; }
        public Vector3 Normal { get; }
    }
}
