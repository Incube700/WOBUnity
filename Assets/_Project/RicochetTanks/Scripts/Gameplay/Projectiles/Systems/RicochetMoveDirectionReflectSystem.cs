using RicochetTanks.Gameplay.Projectiles.Components;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetMoveDirectionReflectSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var request = entity.RicochetRequest;
            entity.MoveDirection = new MoveDirectionComponent(RicochetCalculator.Reflect(request.IncomingDirection, request.HitNormal));
        }
    }
}
