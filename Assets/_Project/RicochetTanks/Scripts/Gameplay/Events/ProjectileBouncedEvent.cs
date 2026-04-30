using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct ProjectileBouncedEvent
    {
        public ProjectileBouncedEvent(Projectile projectile, TankFacade owner, int ricochetCount, int bouncesLeft, float speed, float damage, Vector3 normal)
        {
            Projectile = projectile;
            Owner = owner;
            RicochetCount = ricochetCount;
            BouncesLeft = bouncesLeft;
            Speed = speed;
            Damage = damage;
            Normal = normal;
        }

        public Projectile Projectile { get; }
        public TankFacade Owner { get; }
        public int RicochetCount { get; }
        public int BouncesLeft { get; }
        public float Speed { get; }
        public float Damage { get; }
        public Vector3 Normal { get; }
    }
}
