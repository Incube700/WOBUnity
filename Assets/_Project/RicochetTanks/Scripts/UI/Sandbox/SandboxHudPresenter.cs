using RicochetTanks.Gameplay.Combat;
using System;

namespace RicochetTanks.UI.Sandbox
{
    public sealed class SandboxHudPresenter : IDisposable
    {
        private readonly SandboxHudView _view;
        private readonly TankHealth _playerHealth;
        private readonly TankHealth _enemyHealth;

        public SandboxHudPresenter(
            SandboxHudView view,
            TankHealth playerHealth,
            TankHealth enemyHealth)
        {
            _view = view;
            _playerHealth = playerHealth;
            _enemyHealth = enemyHealth;

            _playerHealth.HealthChanged += OnPlayerHealthChanged;
            _enemyHealth.HealthChanged += OnEnemyHealthChanged;

            OnPlayerHealthChanged(_playerHealth.CurrentHp, _playerHealth.MaxHp);
            OnEnemyHealthChanged(_enemyHealth.CurrentHp, _enemyHealth.MaxHp);
        }

        public void Dispose()
        {
            _playerHealth.HealthChanged -= OnPlayerHealthChanged;
            _enemyHealth.HealthChanged -= OnEnemyHealthChanged;
        }

        private void OnPlayerHealthChanged(int currentHp, int maxHp)
        {
            _view.SetPlayerHp(currentHp, maxHp);
        }

        private void OnEnemyHealthChanged(int currentHp, int maxHp)
        {
            _view.SetEnemyHp(currentHp, maxHp);
        }
    }
}
