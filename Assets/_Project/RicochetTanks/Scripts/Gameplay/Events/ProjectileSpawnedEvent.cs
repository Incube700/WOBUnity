using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct ProjectileSpawnedEvent
    {
        public ProjectileSpawnedEvent(Projectile projectile, TankFacade owner, Vector3 position, Vector3 direction, float speed, float damage, int bouncesLeft)
        {
            Projectile = projectile;
            Owner = owner;
            Position = position;
            Direction = direction;
            Speed = speed;
            Damage = damage;
            BouncesLeft = bouncesLeft;
        }

        public Projectile Projectile { get; }
        public TankFacade Owner { get; }
        public Vector3 Position { get; }
        public Vector3 Direction { get; }
        public float Speed { get; }
        public float Damage { get; }
        public int BouncesLeft { get; }
    }
}
