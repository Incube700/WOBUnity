using RicochetTanks.Gameplay.Projectiles.Components;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetDamageReduceSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var damage = entity.Damage.Value * entity.Ricochet.DamageMultiplierPerBounce;
            entity.Damage = new DamageComponent(damage, entity.Damage.Penetration);
        }
    }
}
