using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Projectiles.Systems;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private ProjectileEntity _entity;
        private ProjectileSystemPipeline _pipeline;

        public void Configure(TankFacade owner, ProjectileConfig config, SandboxGameplayEvents gameplayEvents)
        {
            if (config == null)
            {
                throw new System.ArgumentNullException(nameof(config));
            }

            _entity = new ProjectileEntity(this, transform, gameObject, owner, config, gameplayEvents);
            _pipeline = ProjectileSystemPipeline.CreateDefault();
        }

        public void Initialize(Vector3 direction)
        {
            if (_entity == null)
            {
                throw new System.InvalidOperationException("Projectile must be configured before initialization.");
            }

            _entity.InitializeDirection(direction);
        }

        private void FixedUpdate()
        {
            if (_entity == null || _pipeline == null || _entity.IsDestroyFinalized)
            {
                return;
            }

            _pipeline.Tick(_entity, Time.fixedDeltaTime);
        }
    }
}
