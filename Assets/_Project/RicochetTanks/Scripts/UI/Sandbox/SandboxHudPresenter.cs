using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using System;

namespace RicochetTanks.UI.Sandbox
{
    public sealed class SandboxHudPresenter : IDisposable
    {
        private readonly SandboxHudView _view;
        private readonly TankHealth _playerHealth;
        private readonly TankHealth _enemyHealth;
        private readonly SandboxGameplayEvents _gameplayEvents;
        private readonly Action _restartRequested;

        public SandboxHudPresenter(
            SandboxHudView view,
            TankHealth playerHealth,
            TankHealth enemyHealth,
            SandboxGameplayEvents gameplayEvents,
            Action restartRequested)
        {
            _view = view;
            _playerHealth = playerHealth;
            _enemyHealth = enemyHealth;
            _gameplayEvents = gameplayEvents;
            _restartRequested = restartRequested;

            _playerHealth.HealthChanged += OnPlayerHealthChanged;
            _enemyHealth.HealthChanged += OnEnemyHealthChanged;
            _view.RestartClicked += OnRestartClicked;
            _gameplayEvents.HitResolved += OnHitResolved;
            _gameplayEvents.MatchStarted += OnMatchStarted;
            _gameplayEvents.MatchFinished += OnMatchFinished;

            OnPlayerHealthChanged(_playerHealth.CurrentHp, _playerHealth.MaxHp);
            OnEnemyHealthChanged(_enemyHealth.CurrentHp, _enemyHealth.MaxHp);
            OnMatchStarted();
        }

        public void Dispose()
        {
            _playerHealth.HealthChanged -= OnPlayerHealthChanged;
            _enemyHealth.HealthChanged -= OnEnemyHealthChanged;
            _view.RestartClicked -= OnRestartClicked;
            _gameplayEvents.HitResolved -= OnHitResolved;
            _gameplayEvents.MatchStarted -= OnMatchStarted;
            _gameplayEvents.MatchFinished -= OnMatchFinished;
        }

        private void OnPlayerHealthChanged(int currentHp, int maxHp)
        {
            _view.SetPlayerHp(currentHp, maxHp);
        }

        private void OnEnemyHealthChanged(int currentHp, int maxHp)
        {
            _view.SetEnemyHp(currentHp, maxHp);
        }

        private void OnHitResolved(HitResolvedEvent hit)
        {
            if (hit.Target == null)
            {
                return;
            }

            _view.SetLastHitResult($"Last Hit: {hit.Target.name} {hit.Result} -{hit.Damage} HP ({hit.CurrentHp}/{hit.MaxHp})");
        }

        private void OnMatchStarted()
        {
            _view.SetLastHitResult("Last Hit: none");
            _view.SetRoundResult("Round: Playing");
            _view.SetControlsHint("W/S move  A/D turn  Mouse aim  LMB/Space fire  R restart");
        }

        private void OnMatchFinished(MatchFinishedEvent match)
        {
            _view.SetRoundResult($"Round: {match.Label}");
        }

        private void OnRestartClicked()
        {
            _restartRequested?.Invoke();
        }
    }
}
