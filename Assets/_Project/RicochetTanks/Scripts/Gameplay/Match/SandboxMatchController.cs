using System.Collections;
using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Input;
using RicochetTanks.Input.Desktop;
using UnityEngine;

namespace RicochetTanks.Gameplay.Match
{
    public sealed class SandboxMatchController : MonoBehaviour
    {
        private enum MatchFlowState
        {
            Playing,
            Ending,
            Finished
        }

        private TankFacade _player;
        private TankFacade _enemy;
        private SandboxGameplayEvents _gameplayEvents;
        private ITankInputReader _inputReader;
        private SceneLoaderService _sceneLoaderService;
        private MatchConfig _matchConfig;
        private MatchResult _matchResult = MatchResult.Playing;
        private MatchFlowState _state = MatchFlowState.Playing;
        private Coroutine _finishRoutine;
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
            Configure(player, enemy, gameplayEvents, (ITankInputReader)inputReader, sceneLoaderService, matchConfig);
        }

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            ITankInputReader inputReader,
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
            _state = MatchFlowState.Playing;
            StopFinishRoutine();

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
            StopFinishRoutine();
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
            BeginEnding(ResolveDeathResult(MatchResult.EnemyWins), _player, _enemy);
        }

        private void OnEnemyDied(TankHealth health)
        {
            BeginEnding(ResolveDeathResult(MatchResult.PlayerWins), _enemy, _player);
        }

        private void BeginEnding(MatchResult result, TankFacade deadTank, TankFacade winnerTank)
        {
            if (_state != MatchFlowState.Playing)
            {
                return;
            }

            _state = MatchFlowState.Ending;
            _matchResult = result;

            if (result == MatchResult.Draw)
            {
                DisableTank(_player);
                DisableTank(_enemy);
            }
            else
            {
                DisableTank(deadTank);
                ConfigureWinnerDuringResultDelay(winnerTank);
            }

            _finishRoutine = StartCoroutine(FinishAfterDelay());
        }

        private IEnumerator FinishAfterDelay()
        {
            var delay = _matchConfig != null ? _matchConfig.ResultDelaySeconds : 3f;
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            Finish();
        }

        private void Finish()
        {
            if (_state != MatchFlowState.Ending)
            {
                return;
            }

            _state = MatchFlowState.Finished;
            DisableTank(_player);
            DisableTank(_enemy);

            _finishRoutine = null;
            var label = ResolveResultLabel(_matchResult);
            _gameplayEvents?.RaiseMatchFinished(_matchResult, label);
            if (_gameplayEvents == null || _gameplayEvents.ShouldLogRounds)
            {
                Debug.Log($"[ROUND] result={label}");
            }
        }

        private string ResolveResultLabel(MatchResult result)
        {
            if (_matchConfig == null)
            {
                if (result == MatchResult.PlayerWins)
                {
                    return "Player Wins";
                }

                return result == MatchResult.EnemyWins ? "Enemy Wins" : "Draw";
            }

            if (result == MatchResult.PlayerWins)
            {
                return _matchConfig.PlayerWinsLabel;
            }

            return result == MatchResult.EnemyWins ? _matchConfig.EnemyWinsLabel : _matchConfig.DrawLabel;
        }

        private MatchResult ResolveDeathResult(MatchResult fallbackResult)
        {
            return IsDead(_player) && IsDead(_enemy) ? MatchResult.Draw : fallbackResult;
        }

        private static bool IsDead(TankFacade tank)
        {
            return tank == null || tank.Health == null || !tank.Health.IsAlive;
        }

        private void ConfigureWinnerDuringResultDelay(TankFacade winnerTank)
        {
            if (winnerTank == null)
            {
                return;
            }

            if (_matchConfig != null && !_matchConfig.AllowWinnerControlDuringResultDelay)
            {
                DisableTank(winnerTank);
                return;
            }

            if (_matchConfig == null || _matchConfig.DisableShootingAfterMatchDecided)
            {
                winnerTank.Shooter?.SetCanShoot(false);
            }
        }

        private static void DisableTank(TankFacade tank)
        {
            if (tank != null)
            {
                tank.SetGameplayEnabled(false);
            }
        }

        private void StopFinishRoutine()
        {
            if (_finishRoutine == null)
            {
                return;
            }

            StopCoroutine(_finishRoutine);
            _finishRoutine = null;
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
