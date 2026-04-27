using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public class TankShooter : MonoBehaviour
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _cooldown = 0.35f;

        private TankFacade _owner;
        private ProjectileFactory _projectileFactory;
        private float _nextShotTime;
        private bool _canShoot = true;

        public void Configure(Transform muzzle, TankFacade owner, ProjectileFactory projectileFactory, ProjectileConfig projectileConfig)
        {
            _muzzle = muzzle;
            _owner = owner;
            _projectileFactory = projectileFactory;

            if (projectileConfig != null)
            {
                _cooldown = projectileConfig.Cooldown;
            }
        }

        public void SetCanShoot(bool canShoot)
        {
            _canShoot = canShoot;
        }

        public void TryShoot()
        {
            if (!_canShoot || _muzzle == null || _projectileFactory == null || Time.time < _nextShotTime)
            {
                return;
            }

            _nextShotTime = Time.time + _cooldown;
            _projectileFactory.Spawn(_owner, _muzzle);
        }
    }
}
