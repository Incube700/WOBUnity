using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class ProjectileDestroySystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (!entity.IsDestroyRequested || entity.IsDestroyFinalized)
            {
                return;
            }

            entity.MarkDestroyFinalized();
            Object.Destroy(entity.GameObject);
        }
    }
}
