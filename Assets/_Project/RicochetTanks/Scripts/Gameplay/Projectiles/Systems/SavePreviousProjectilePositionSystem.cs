using RicochetTanks.Gameplay.Projectiles.Components;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class SavePreviousProjectilePositionSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested)
            {
                return;
            }

            entity.PreviousPosition = new PreviousPositionComponent(entity.Transform.position);
        }
    }
}
