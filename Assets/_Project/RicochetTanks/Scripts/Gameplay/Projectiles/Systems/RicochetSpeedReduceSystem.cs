using RicochetTanks.Gameplay.Projectiles.Components;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetSpeedReduceSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || !entity.RicochetRequest.IsActive)
            {
                return;
            }

            var speed = Mathf.Max(entity.Ricochet.MinSpeed, entity.MoveSpeed.Value * entity.Ricochet.SpeedMultiplierPerBounce);
            entity.MoveSpeed = new MoveSpeedComponent(speed);
        }
    }
}
