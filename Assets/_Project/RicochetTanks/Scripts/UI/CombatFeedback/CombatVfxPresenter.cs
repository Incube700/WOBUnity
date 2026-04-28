using System;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    public sealed class CombatVfxPresenter : IDisposable
    {
        private readonly SandboxGameplayEvents _gameplayEvents;
        private readonly CombatVfxFactory _factory;
        private readonly TankFacade _player;
        private readonly TankFacade _enemy;
        private bool _isDisposed;

        public CombatVfxPresenter(
            SandboxGameplayEvents gameplayEvents,
            CombatVfxFactory factory,
            TankFacade player,
            TankFacade enemy)
        {
            _gameplayEvents = gameplayEvents;
            _factory = factory;
            _player = player;
            _enemy = enemy;

            Subscribe();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            Unsubscribe();
            _isDisposed = true;
        }

        private void Subscribe()
        {
            if (_gameplayEvents != null)
            {
                _gameplayEvents.ProjectileSpawned += OnProjectileSpawned;
                _gameplayEvents.ProjectileHit += OnProjectileHit;
                _gameplayEvents.ProjectileBounced += OnProjectileBounced;
                _gameplayEvents.CombatFeedbackRequested += OnCombatFeedbackRequested;
            }

            SubscribeTank(_player);
            SubscribeTank(_enemy);
        }

        private void Unsubscribe()
        {
            if (_gameplayEvents != null)
            {
                _gameplayEvents.ProjectileSpawned -= OnProjectileSpawned;
                _gameplayEvents.ProjectileHit -= OnProjectileHit;
                _gameplayEvents.ProjectileBounced -= OnProjectileBounced;
                _gameplayEvents.CombatFeedbackRequested -= OnCombatFeedbackRequested;
            }

            UnsubscribeTank(_player);
            UnsubscribeTank(_enemy);
        }

        private void SubscribeTank(TankFacade tank)
        {
            if (tank != null && tank.Health != null)
            {
                tank.Health.Died += OnTankDied;
            }
        }

        private void UnsubscribeTank(TankFacade tank)
        {
            if (tank != null && tank.Health != null)
            {
                tank.Health.Died -= OnTankDied;
            }
        }

        private void OnProjectileSpawned(ProjectileSpawnedEvent projectileSpawned)
        {
            _factory?.ConfigureProjectileTrail(projectileSpawned.Projectile);
            _factory?.PlayShotRecoil(projectileSpawned.Owner);
        }

        private void OnProjectileHit(ProjectileHitEvent projectileHit)
        {
            if (projectileHit.Collider != null && projectileHit.Collider.GetComponentInParent<TankFacade>() != null)
            {
                return;
            }

            _factory?.CreateWorldImpact(projectileHit.Point, projectileHit.Normal);
        }

        private void OnProjectileBounced(ProjectileBouncedEvent projectileBounced)
        {
            if (projectileBounced.Projectile == null)
            {
                return;
            }

            _factory?.CreateRicochet(projectileBounced.Projectile.transform.position, projectileBounced.Normal);
        }

        private void OnCombatFeedbackRequested(CombatFeedbackEvent feedback)
        {
            _factory?.CreateTankHit(feedback.WorldPoint, feedback.WorldNormal, feedback.Result);
        }

        private void OnTankDied(TankHealth health)
        {
            if (health == null)
            {
                return;
            }

            var healthTransform = health.transform;
            _factory?.CreateTankDeath(healthTransform.position, healthTransform.rotation);
        }
    }
}
