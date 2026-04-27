namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetSpendBounceSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var ricochet = entity.Ricochet;
            ricochet.BouncesLeft--;
            ricochet.RicochetCount++;
            entity.Ricochet = ricochet;
        }
    }
}
