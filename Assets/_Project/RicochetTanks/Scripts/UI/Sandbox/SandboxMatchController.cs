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
        private MatchResult _matchResult = MatchResult.Playing;
        private bool _isSubscribed;

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            DesktopInputReader inputReader,
            SceneLoaderService sceneLoaderService)
        {
            Unsubscribe();

            _player = player;
            _enemy = enemy;
            _gameplayEvents = gameplayEvents;
            _inputReader = inputReader;
            _sceneLoaderService = sceneLoaderService;
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

            var label = result == MatchResult.PlayerWins ? "Player Wins" : "Enemy Wins";
            _gameplayEvents?.RaiseMatchFinished(result, label);
            Debug.Log($"[ROUND] result={label}");
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
