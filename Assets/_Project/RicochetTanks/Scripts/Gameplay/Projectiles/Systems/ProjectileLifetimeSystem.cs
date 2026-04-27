using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class ProjectileLifetimeSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested)
            {
                return;
            }

            if (Time.time >= entity.Lifetime.SpawnTime + entity.Lifetime.Lifetime)
            {
                entity.RequestDestroy();
            }
        }
    }
}
