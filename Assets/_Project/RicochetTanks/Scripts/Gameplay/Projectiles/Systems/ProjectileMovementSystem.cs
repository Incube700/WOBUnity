namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class ProjectileMovementSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested)
            {
                return;
            }

            entity.Transform.position += entity.MoveDirection.Value * (entity.MoveSpeed.Value * deltaTime);
        }
    }
}
