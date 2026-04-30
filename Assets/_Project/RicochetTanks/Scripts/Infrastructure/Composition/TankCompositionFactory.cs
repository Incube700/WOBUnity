using System;
using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Gameplay.Tanks.Presentation;
using RicochetTanks.Input;
using UnityEngine;

namespace RicochetTanks.Infrastructure.Composition
{
    public sealed class TankCompositionFactory
    {
        private readonly Camera _camera;
        private readonly ITankInputReader _inputReader;
        private readonly ProjectileFactory _projectileFactory;
        private readonly ProjectileConfig _projectileConfig;
        private readonly LaserAimConfig _laserAimConfig;

        public TankCompositionFactory(
            Camera camera,
            ITankInputReader inputReader,
            ProjectileFactory projectileFactory,
            ProjectileConfig projectileConfig,
            LaserAimConfig laserAimConfig)
        {
            _camera = camera;
            _inputReader = inputReader;
            _projectileFactory = projectileFactory;
            _projectileConfig = projectileConfig;
            _laserAimConfig = laserAimConfig;
        }

        public void ConfigureTank(TankFacade tank, Transform spawnPoint, TankConfig tankConfig, bool isPlayerControlled)
        {
            if (tank == null)
            {
                throw new InvalidOperationException($"Missing {(isPlayerControlled ? "player" : "enemy")} tank scene reference.");
            }

            if (spawnPoint != null)
            {
                tank.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }

            var body = tank.gameObject;
            var rigidbody = GetOrAdd<Rigidbody>(body);
            rigidbody.useGravity = false;
            rigidbody.isKinematic = !isPlayerControlled;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            var hitbox = GetOrAdd<BoxCollider>(body);
            hitbox.center = new Vector3(0f, 0.5f, 0f);
            hitbox.size = new Vector3(1.2f, 0.9f, 1.35f);

            var movement = GetOrAdd<TankMovement>(body);
            var aiming = GetOrAdd<TurretAiming>(body);
            var shooter = GetOrAdd<TankShooter>(body);
            var health = GetOrAdd<TankHealth>(body);
            var armor = GetOrAdd<TankArmor>(body);
            var controller = GetOrAdd<PlayerTankController>(body);
            var turret = FindDescendant(tank.transform, "Turret");
            if (turret == null)
            {
                turret = CreateChild(tank.transform, "Turret", new Vector3(0f, 0.62f, 0f));
            }

            var muzzle = FindDescendant(turret, "Muzzle");
            if (muzzle == null)
            {
                muzzle = CreateChild(turret, "Muzzle", new Vector3(0f, 0f, 1.25f));
            }

            movement.Configure(
                rigidbody,
                tankConfig.MaxForwardSpeed,
                tankConfig.MaxReverseSpeed,
                tankConfig.Acceleration,
                tankConfig.BrakeDeceleration,
                tankConfig.CoastDeceleration,
                tankConfig.TurnSpeed,
                tankConfig.TurnSpeedAtLowVelocity,
                tankConfig.InputDeadZone);
            movement.ConfigureRecoil(
                tankConfig.ShotRecoilImpulse,
                tankConfig.ShotRecoilDecay,
                tankConfig.MaxRecoilVelocity);
            aiming.Configure(turret, _camera, tankConfig.TurretRotationSpeed);
            shooter.Configure(muzzle, tank, _projectileFactory, _projectileConfig);
            health.Configure(tankConfig.MaxHp);
            armor.Configure(tankConfig);
            controller.Configure(tank, _inputReader, _camera);
            tank.Configure(movement, aiming, shooter, health, controller);
            tank.SetPlayerControlled(isPlayerControlled);
            ConfigureLaserAim(body, muzzle, health, isPlayerControlled);
        }

        private void ConfigureLaserAim(GameObject body, Transform muzzle, TankHealth health, bool isPlayerControlled)
        {
            var shouldShowLaser = _laserAimConfig != null
                && (isPlayerControlled ? _laserAimConfig.ShowPlayerLaser : _laserAimConfig.ShowEnemyLaser);

            if (!shouldShowLaser)
            {
                if (body.TryGetComponent<LaserAimView>(out var existingLaser))
                {
                    existingLaser.enabled = false;
                }

                return;
            }

            var laserAimView = GetOrAdd<LaserAimView>(body);
            laserAimView.enabled = true;
            laserAimView.Configure(muzzle, _laserAimConfig, health, body.transform);
        }

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            if (target.TryGetComponent<T>(out var component))
            {
                return component;
            }

            return target.AddComponent<T>();
        }

        private static Transform FindDescendant(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == objectName)
            {
                return root;
            }

            for (var index = 0; index < root.childCount; index++)
            {
                var result = FindDescendant(root.GetChild(index), objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static Transform CreateChild(Transform parent, string objectName, Vector3 localPosition)
        {
            var child = new GameObject(objectName).transform;
            child.SetParent(parent, false);
            child.localPosition = localPosition;
            return child;
        }
    }
}
