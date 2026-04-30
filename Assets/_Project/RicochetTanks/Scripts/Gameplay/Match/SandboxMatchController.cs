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
        private enum GameplaySessionState
        {
            Initializing,
            RoundStarting,
            RoundPlaying,
            RoundFinished,
            BetweenRounds,
            MatchFinished
        }

        private TankFacade _player;
        private TankFacade _enemy;
        private SandboxGameplayEvents _gameplayEvents;
        private ITankInputReader _inputReader;
        private SceneLoaderService _sceneLoaderService;
        private MatchConfig _matchConfig;
        private LocalSessionConfig _sessionConfig;
        private LocalMatchSessionService _sessionService;
        private MatchResult _roundResult = MatchResult.Playing;
        private GameplaySessionState _state = GameplaySessionState.Initializing;
        private Coroutine _finishRoutine;
        private Coroutine _betweenRoundsRoutine;
        private bool _isSubscribed;

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            DesktopInputReader inputReader,
            SceneLoaderService sceneLoaderService)
        {
            Configure(player, enemy, gameplayEvents, inputReader, sceneLoaderService, null, null, null);
        }

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            DesktopInputReader inputReader,
            SceneLoaderService sceneLoaderService,
            MatchConfig matchConfig)
        {
            Configure(player, enemy, gameplayEvents, inputReader, sceneLoaderService, matchConfig, null, null);
        }

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            ITankInputReader inputReader,
            SceneLoaderService sceneLoaderService,
            MatchConfig matchConfig)
        {
            Configure(player, enemy, gameplayEvents, inputReader, sceneLoaderService, matchConfig, null, null);
        }

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            ITankInputReader inputReader,
            SceneLoaderService sceneLoaderService,
            MatchConfig matchConfig,
            LocalSessionConfig sessionConfig,
            LocalMatchSessionService sessionService)
        {
            Unsubscribe();

            _player = player;
            _enemy = enemy;
            _gameplayEvents = gameplayEvents;
            _inputReader = inputReader;
            _sceneLoaderService = sceneLoaderService;
            _matchConfig = matchConfig;
            _sessionConfig = sessionConfig != null ? sessionConfig : ScriptableObject.CreateInstance<LocalSessionConfig>();
            _sessionService = sessionService ?? new LocalMatchSessionService();
            _roundResult = MatchResult.Playing;
            _state = GameplaySessionState.RoundStarting;
            StopRoutines();

            var isNewMatch = _sessionService.EnsureMatch(_sessionConfig.RoundsToWin);
            var shouldRaiseMatchStarted = _sessionService.ConsumeMatchStartedFlag();
            Subscribe();

            if (isNewMatch || shouldRaiseMatchStarted)
            {
                _gameplayEvents?.RaiseMatchStarted();
            }

            BeginRound();
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
            StopRoutines();
            Unsubscribe();
        }

        public void RequestRestart()
        {
            _gameplayEvents?.RaiseRestartRequested();
        }

        public void RequestExitToMainMenu()
        {
            _sessionService?.AbandonMatch();
            _sceneLoaderService?.Load(SceneLoaderService.MainMenuSceneName);
        }

        private void BeginRound()
        {
            _state = GameplaySessionState.RoundPlaying;
            EnableTank(_player);
            EnableTank(_enemy);
            _gameplayEvents?.RaiseRoundStarted();
            RaiseScore();
            _gameplayEvents?.RaiseSessionStatusChanged(
                "Round " + _sessionService.CurrentRound
                + " | Score Player " + _sessionService.GetScoreLabel() + " Enemy"
                + " | First to " + _sessionService.RoundsToWin);
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
            BeginRoundEnding(ResolveDeathResult(MatchResult.EnemyWins), _player, _enemy);
        }

        private void OnEnemyDied(TankHealth health)
        {
            BeginRoundEnding(ResolveDeathResult(MatchResult.PlayerWins), _enemy, _player);
        }

        private void BeginRoundEnding(MatchResult result, TankFacade deadTank, TankFacade winnerTank)
        {
            if (_state != GameplaySessionState.RoundPlaying)
            {
                return;
            }

            _state = GameplaySessionState.RoundFinished;
            _roundResult = result;

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

            _finishRoutine = StartCoroutine(FinishRoundAfterDelay());
        }

        private IEnumerator FinishRoundAfterDelay()
        {
            var delay = _matchConfig != null ? _matchConfig.ResultDelaySeconds : 3f;
            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }

            FinishRound();
        }

        private void FinishRound()
        {
            if (_state != GameplaySessionState.RoundFinished)
            {
                return;
            }

            DisableTank(_player);
            DisableTank(_enemy);
            _finishRoutine = null;

            var label = ResolveResultLabel(_roundResult);
            var completedRound = _sessionService.CurrentRound;
            _sessionService.RecordRound(_roundResult);
            RaiseScore();

            _gameplayEvents?.RaiseRoundFinished(new RoundFinishedEvent(
                _roundResult,
                label,
                _sessionService.PlayerScore,
                _sessionService.EnemyScore,
                completedRound,
                _sessionService.RoundsToWin));

            if (_gameplayEvents == null || _gameplayEvents.ShouldLogRounds)
            {
                Debug.Log("[ROUND] result=" + label + " score=" + _sessionService.GetScoreLabel());
            }

            if (_sessionService.HasWinner())
            {
                FinishMatch();
                return;
            }

            _betweenRoundsRoutine = StartCoroutine(LoadNextRoundAfterBreak(label));
        }

        private IEnumerator LoadNextRoundAfterBreak(string roundLabel)
        {
            _state = GameplaySessionState.BetweenRounds;
            var remaining = _sessionConfig != null ? _sessionConfig.RoundBreakSeconds : 5f;

            while (remaining > 0f)
            {
                _gameplayEvents?.RaiseSessionStatusChanged(
                    "Round: " + roundLabel
                    + " | Score Player " + _sessionService.GetScoreLabel() + " Enemy"
                    + " | Next round in " + Mathf.CeilToInt(remaining));
                yield return new WaitForSeconds(1f);
                remaining -= 1f;
            }

            var nextScene = _sessionConfig.GetSceneForRound(_sessionService.CurrentRound, SceneLoaderService.DemoSceneName);
            _sceneLoaderService?.Load(nextScene);
        }

        private void FinishMatch()
        {
            _state = GameplaySessionState.MatchFinished;
            var result = _sessionService.CompleteMatch();
            var label = "Match: " + ResolveResultLabel(result);
            _gameplayEvents?.RaiseSessionStatusChanged(
                label + " | Final score Player " + _sessionService.GetScoreLabel() + " Enemy");
            _gameplayEvents?.RaiseMatchFinished(result, label);
        }

        private void RaiseScore()
        {
            _gameplayEvents?.RaiseSessionScoreChanged(new SessionScoreEvent(
                _sessionService.PlayerScore,
                _sessionService.EnemyScore,
                _sessionService.CurrentRound,
                _sessionService.RoundsToWin));
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

        private static void EnableTank(TankFacade tank)
        {
            if (tank != null)
            {
                tank.SetGameplayEnabled(true);
            }
        }

        private static void DisableTank(TankFacade tank)
        {
            if (tank != null)
            {
                tank.SetGameplayEnabled(false);
            }
        }

        private void StopRoutines()
        {
            if (_finishRoutine != null)
            {
                StopCoroutine(_finishRoutine);
                _finishRoutine = null;
            }

            if (_betweenRoundsRoutine != null)
            {
                StopCoroutine(_betweenRoundsRoutine);
                _betweenRoundsRoutine = null;
            }
        }

        private void OnRestartRequested()
        {
            _sessionService?.StartNewMatch(_sessionConfig != null ? _sessionConfig.RoundsToWin : 3);
            _sceneLoaderService?.Load(SceneLoaderService.DemoSceneName);
        }
    }
}
