using RicochetTanks.Gameplay.Projectiles;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct ProjectileBouncedEvent
    {
        public ProjectileBouncedEvent(Projectile projectile, int ricochetCount, int bouncesLeft, float speed, float damage, Vector3 normal)
        {
            Projectile = projectile;
            RicochetCount = ricochetCount;
            BouncesLeft = bouncesLeft;
            Speed = speed;
            Damage = damage;
            Normal = normal;
        }

        public Projectile Projectile { get; }
        public int RicochetCount { get; }
        public int BouncesLeft { get; }
        public float Speed { get; }
        public float Damage { get; }
        public Vector3 Normal { get; }
    }
}
