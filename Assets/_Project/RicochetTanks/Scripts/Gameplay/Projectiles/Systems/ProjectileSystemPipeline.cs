namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class ProjectileSystemPipeline
    {
        private readonly IProjectileFixedSystem[] _systems;

        public ProjectileSystemPipeline(IProjectileFixedSystem[] systems)
        {
            _systems = systems;
        }

        public static ProjectileSystemPipeline CreateDefault()
        {
            return new ProjectileSystemPipeline(new IProjectileFixedSystem[]
            {
                new SavePreviousProjectilePositionSystem(),
                new ProjectileMovementSystem(),
                new RicochetDetectionSystem(),
                new ProjectileHitDetectionSystem(),
                new RicochetPositionCorrectionSystem(),
                new RicochetMoveDirectionReflectSystem(),
                new RicochetRotationReflectSystem(),
                new RicochetSpendBounceSystem(),
                new RicochetSpeedReduceSystem(),
                new RicochetDamageReduceSystem(),
                new RicochetEventPublishSystem(),
                new RicochetCleanupSystem(),
                new ProjectileLifetimeSystem(),
                new ProjectileDestroySystem()
            });
        }

        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            for (var index = 0; index < _systems.Length; index++)
            {
                _systems[index].Tick(entity, deltaTime);

                if (entity.IsDestroyFinalized)
                {
                    return;
                }
            }
        }
    }
}
