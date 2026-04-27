namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetCleanupSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (!entity.RicochetRequest.IsActive)
            {
                return;
            }

            var request = entity.RicochetRequest;
            request.Clear();
            entity.RicochetRequest = request;
        }
    }
}
