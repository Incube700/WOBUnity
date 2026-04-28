using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Input.Desktop;
using UnityEngine;

namespace RicochetTanks.UI.Sandbox
{
    public sealed class SandboxMatchController : MonoBehaviour
    {
        private TankFacade _player;
        private TankFacade _enemy;
        private SandboxGameplayEvents _gameplayEvents;
        private DesktopInputReader _inputReader;
        private SceneLoaderService _sceneLoaderService;
        private MatchConfig _matchConfig;
        private MatchResult _matchResult = MatchResult.Playing;
        private bool _isSubscribed;

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            DesktopInputReader inputReader,
            SceneLoaderService sceneLoaderService)
        {
            Configure(player, enemy, gameplayEvents, inputReader, sceneLoaderService, null);
        }

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            DesktopInputReader inputReader,
            SceneLoaderService sceneLoaderService,
            MatchConfig matchConfig)
        {
            Unsubscribe();

            _player = player;
            _enemy = enemy;
            _gameplayEvents = gameplayEvents;
            _inputReader = inputReader;
            _sceneLoaderService = sceneLoaderService;
            _matchConfig = matchConfig;
            _matchResult = MatchResult.Playing;

            Subscribe();
            _gameplayEvents?.RaiseMatchStarted();
        }

        private void Update()
        {
            if (_inputReader != null && _inputReader.IsRestartPressed())
            {
                RequestRestart();
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (_player != null && _player.Health != null)
            {
                _player.Health.Died += OnPlayerDied;
            }

            if (_enemy != null && _enemy.Health != null)
            {
                _enemy.Health.Died += OnEnemyDied;
            }

            if (_gameplayEvents != null)
            {
                _gameplayEvents.RestartRequested += OnRestartRequested;
            }

            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_player != null && _player.Health != null)
            {
                _player.Health.Died -= OnPlayerDied;
            }

            if (_enemy != null && _enemy.Health != null)
            {
                _enemy.Health.Died -= OnEnemyDied;
            }

            if (_gameplayEvents != null)
            {
                _gameplayEvents.RestartRequested -= OnRestartRequested;
            }

            _isSubscribed = false;
        }

        private void OnPlayerDied(TankHealth health)
        {
            Finish(MatchResult.EnemyWins);
        }

        private void OnEnemyDied(TankHealth health)
        {
            Finish(MatchResult.PlayerWins);
        }

        private void Finish(MatchResult result)
        {
            if (_matchResult != MatchResult.Playing)
            {
                return;
            }

            _matchResult = result;

            if (_player != null)
            {
                _player.SetGameplayEnabled(false);
            }

            if (_enemy != null)
            {
                _enemy.SetGameplayEnabled(false);
            }

            var label = ResolveResultLabel(result);
            _gameplayEvents?.RaiseMatchFinished(result, label);
            if (_gameplayEvents == null || _gameplayEvents.ShouldLogRounds)
            {
                Debug.Log($"[ROUND] result={label}");
            }
        }

        private string ResolveResultLabel(MatchResult result)
        {
            if (_matchConfig == null)
            {
                return result == MatchResult.PlayerWins ? "Player Wins" : "Enemy Wins";
            }

            return result == MatchResult.PlayerWins ? _matchConfig.PlayerWinsLabel : _matchConfig.EnemyWinsLabel;
        }

        public void RequestRestart()
        {
            _gameplayEvents?.RaiseRestartRequested();
        }

        private void OnRestartRequested()
        {
            _sceneLoaderService?.ReloadActiveScene();
        }
    }
}
