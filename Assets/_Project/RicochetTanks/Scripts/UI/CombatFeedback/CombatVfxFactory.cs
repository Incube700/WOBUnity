using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    public sealed class CombatVfxFactory
    {
        private readonly ProjectileTrailVfxFactory _projectileTrailFactory;
        private readonly ImpactVfxFactory _impactFactory;
        private readonly DeathVfxFactory _deathFactory;
        private readonly ShotRecoilVfxPlayer _shotRecoilPlayer;

        public CombatVfxFactory(CombatVfxConfig config, Transform root)
        {
            _projectileTrailFactory = new ProjectileTrailVfxFactory(config);
            _impactFactory = new ImpactVfxFactory(config, root);
            _deathFactory = new DeathVfxFactory(config, root);
            _shotRecoilPlayer = new ShotRecoilVfxPlayer(config);
        }

        public void ConfigureProjectileTrail(Projectile projectile)
        {
            _projectileTrailFactory.ConfigureProjectileTrail(projectile);
        }

        public void CreateWorldImpact(Vector3 point, Vector3 normal)
        {
            _impactFactory.CreateWorldImpact(point, normal);
        }

        public void CreateTankHit(Vector3 point, Vector3 normal, HitResult result)
        {
            _impactFactory.CreateTankHit(point, normal, result);
        }

        public void CreateRicochet(Vector3 point, Vector3 normal)
        {
            _impactFactory.CreateRicochet(point, normal);
        }

        public void CreateTankDeath(Vector3 position, Quaternion rotation)
        {
            _deathFactory.CreateTankDeath(position, rotation);
        }

        public void PlayShotRecoil(TankFacade owner)
        {
            _shotRecoilPlayer.PlayShotRecoil(owner);
        }
    }
}
