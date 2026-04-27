namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetRotationReflectSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            entity.UpdateRotation();
        }
    }
}
