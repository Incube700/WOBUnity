using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.UI.Core;
using System;

namespace RicochetTanks.UI.Sandbox
{
    public sealed class SandboxHudPresenter : IPresenter
    {
        private readonly SandboxHudView _view;
        private readonly TankHealth _playerHealth;
        private readonly TankHealth _enemyHealth;
        private readonly SandboxGameplayEvents _gameplayEvents;
        private readonly Action _restartRequested;
        private readonly Action _exitToMenuRequested;
        private readonly MatchConfig _matchConfig;

        public SandboxHudPresenter(
            SandboxHudView view,
            TankHealth playerHealth,
            TankHealth enemyHealth,
            SandboxGameplayEvents gameplayEvents,
            Action restartRequested)
            : this(view, playerHealth, enemyHealth, gameplayEvents, restartRequested, null, null)
        {
        }

        public SandboxHudPresenter(
            SandboxHudView view,
            TankHealth playerHealth,
            TankHealth enemyHealth,
            SandboxGameplayEvents gameplayEvents,
            Action restartRequested,
            MatchConfig matchConfig)
            : this(view, playerHealth, enemyHealth, gameplayEvents, restartRequested, null, matchConfig)
        {
        }

        public SandboxHudPresenter(
            SandboxHudView view,
            TankHealth playerHealth,
            TankHealth enemyHealth,
            SandboxGameplayEvents gameplayEvents,
            Action restartRequested,
            Action exitToMenuRequested,
            MatchConfig matchConfig)
        {
            _view = view;
            _playerHealth = playerHealth;
            _enemyHealth = enemyHealth;
            _gameplayEvents = gameplayEvents;
            _restartRequested = restartRequested;
            _exitToMenuRequested = exitToMenuRequested;
            _matchConfig = matchConfig;

            _playerHealth.HealthChanged += OnPlayerHealthChanged;
            _enemyHealth.HealthChanged += OnEnemyHealthChanged;
            _view.RestartClicked += OnRestartClicked;
            _view.ExitToMenuClicked += OnExitToMenuClicked;
            _gameplayEvents.HitResolved += OnHitResolved;
            _gameplayEvents.MatchStarted += OnMatchStarted;
            _gameplayEvents.RoundStarted += OnRoundStarted;
            _gameplayEvents.RoundFinished += OnRoundFinished;
            _gameplayEvents.SessionScoreChanged += OnSessionScoreChanged;
            _gameplayEvents.SessionStatusChanged += OnSessionStatusChanged;
            _gameplayEvents.MatchFinished += OnMatchFinished;

            OnPlayerHealthChanged(_playerHealth.CurrentHp, _playerHealth.MaxHp);
            OnEnemyHealthChanged(_enemyHealth.CurrentHp, _enemyHealth.MaxHp);
            OnRoundStarted();
        }

        public void Dispose()
        {
            _playerHealth.HealthChanged -= OnPlayerHealthChanged;
            _enemyHealth.HealthChanged -= OnEnemyHealthChanged;
            _view.RestartClicked -= OnRestartClicked;
            _view.ExitToMenuClicked -= OnExitToMenuClicked;
            _gameplayEvents.HitResolved -= OnHitResolved;
            _gameplayEvents.MatchStarted -= OnMatchStarted;
            _gameplayEvents.RoundStarted -= OnRoundStarted;
            _gameplayEvents.RoundFinished -= OnRoundFinished;
            _gameplayEvents.SessionScoreChanged -= OnSessionScoreChanged;
            _gameplayEvents.SessionStatusChanged -= OnSessionStatusChanged;
            _gameplayEvents.MatchFinished -= OnMatchFinished;
        }

        private void OnPlayerHealthChanged(float currentHp, float maxHp)
        {
            _view.SetPlayerHp(currentHp, maxHp);
        }

        private void OnEnemyHealthChanged(float currentHp, float maxHp)
        {
            _view.SetEnemyHp(currentHp, maxHp);
        }

        private void OnHitResolved(HitResolvedEvent hit)
        {
            if (hit.Target == null)
            {
                return;
            }

            _view.SetLastHitResult($"Last Hit: {hit.Target.name} {hit.Result} -{Format(hit.Damage)} HP ({Format(hit.CurrentHp)}/{Format(hit.MaxHp)})");
        }

        private void OnMatchStarted()
        {
            _view.SetResultControlsVisible(false);
        }

        private void OnRoundStarted()
        {
            _view.SetLastHitResult("Last Hit: none");
            _view.SetRoundResult(_matchConfig != null ? _matchConfig.PlayingLabel : "Round: Playing");
            _view.SetResultControlsVisible(false);
            _view.SetControlsHint("W/S move  A/D turn  Mouse aim  LMB/Space fire  R restart");
        }

        private void OnRoundFinished(RoundFinishedEvent round)
        {
            _view.SetRoundResult("Round: " + round.Label);
            _view.SetScore("Score: Player " + round.PlayerScore + " : " + round.EnemyScore + " Enemy");
        }

        private void OnSessionScoreChanged(SessionScoreEvent score)
        {
            _view.SetScore("Score: Player " + score.PlayerScore + " : " + score.EnemyScore + " Enemy");
        }

        private void OnSessionStatusChanged(string status)
        {
            _view.SetRoundResult(status);
        }

        private void OnMatchFinished(MatchFinishedEvent match)
        {
            _view.SetRoundResult(match.Label);
            _view.SetResultControlsVisible(true);
        }

        private void OnRestartClicked()
        {
            _restartRequested?.Invoke();
        }

        private void OnExitToMenuClicked()
        {
            _exitToMenuRequested?.Invoke();
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
