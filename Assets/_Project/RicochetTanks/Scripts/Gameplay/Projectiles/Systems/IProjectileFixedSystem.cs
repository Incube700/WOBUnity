namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public interface IProjectileFixedSystem
    {
        void Tick(ProjectileEntity entity, float deltaTime);
    }
}
