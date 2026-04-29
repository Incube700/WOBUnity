using System;
using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.UI.CombatFeedback;
using UnityEngine;

namespace RicochetTanks.Infrastructure.Composition
{
    public sealed class CombatFeedbackComposition : IDisposable
    {
        private readonly CombatFeedbackFactory _combatFeedbackFactory;
        private readonly CombatFeedbackPresenter _combatFeedbackPresenter;
        private readonly CombatVfxPresenter _combatVfxPresenter;
        private readonly TankHealthBarPresenter _playerHealthBarPresenter;
        private readonly TankHealthBarPresenter _enemyHealthBarPresenter;
        private readonly TankHealthBarView _playerHealthBarView;
        private readonly TankHealthBarView _enemyHealthBarView;

        public CombatFeedbackComposition(
            GameObject worldHealthBarPrefab,
            GameObject floatingHitTextPrefab,
            CombatVfxConfig combatVfxConfig,
            Transform root,
            Camera camera,
            SandboxGameplayEvents gameplayEvents,
            TankFacade player,
            TankFacade enemy)
        {
            _combatFeedbackFactory = new CombatFeedbackFactory(worldHealthBarPrefab, floatingHitTextPrefab, root, camera);

            _playerHealthBarView = _combatFeedbackFactory.CreateHealthBar(player);
            if (_playerHealthBarView != null)
            {
                _playerHealthBarPresenter = new TankHealthBarPresenter(_playerHealthBarView, player.Health);
            }

            _enemyHealthBarView = _combatFeedbackFactory.CreateHealthBar(enemy);
            if (_enemyHealthBarView != null)
            {
                _enemyHealthBarPresenter = new TankHealthBarPresenter(_enemyHealthBarView, enemy.Health);
            }

            _combatFeedbackPresenter = new CombatFeedbackPresenter(gameplayEvents, _combatFeedbackFactory);
            _combatVfxPresenter = new CombatVfxPresenter(
                gameplayEvents,
                new CombatVfxFactory(combatVfxConfig, root),
                player,
                enemy);
        }

        public void Dispose()
        {
            _combatVfxPresenter?.Dispose();
            _combatFeedbackPresenter?.Dispose();
            _playerHealthBarPresenter?.Dispose();
            _enemyHealthBarPresenter?.Dispose();
            DestroyHealthBar(_playerHealthBarView);
            DestroyHealthBar(_enemyHealthBarView);
        }

        private static void DestroyHealthBar(TankHealthBarView healthBarView)
        {
            if (healthBarView == null)
            {
                return;
            }

            UnityEngine.Object.Destroy(healthBarView.gameObject);
        }
    }
}
