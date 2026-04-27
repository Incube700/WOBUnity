namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetPositionCorrectionSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var request = entity.RicochetRequest;
            entity.Transform.position = request.HitPoint + request.HitNormal * entity.Ricochet.PositionOffset;
        }
    }
}
