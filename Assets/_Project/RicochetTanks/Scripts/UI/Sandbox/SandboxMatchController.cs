using RicochetTanks.Gameplay.Combat;
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
        private SandboxHudView _hudView;
        private DesktopInputReader _inputReader;
        private SceneLoaderService _sceneLoaderService;
        private MatchResult _matchResult = MatchResult.Playing;
        private bool _isSubscribed;

        public void Configure(
            TankFacade player,
            TankFacade enemy,
            SandboxHudView hudView,
            DesktopInputReader inputReader,
            SceneLoaderService sceneLoaderService)
        {
            Unsubscribe();

            _player = player;
            _enemy = enemy;
            _hudView = hudView;
            _inputReader = inputReader;
            _sceneLoaderService = sceneLoaderService;
            _matchResult = MatchResult.Playing;

            Subscribe();
            SetInitialHudState();
        }

        private void Update()
        {
            if (_inputReader != null && _inputReader.IsRestartPressed())
            {
                Restart();
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

            if (_hudView != null)
            {
                _hudView.RestartClicked += OnRestartClicked;
            }

            HitResolver.HitResolved += OnHitResolved;
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

            if (_hudView != null)
            {
                _hudView.RestartClicked -= OnRestartClicked;
            }

            HitResolver.HitResolved -= OnHitResolved;
            _isSubscribed = false;
        }

        private void SetInitialHudState()
        {
            if (_hudView == null)
            {
                return;
            }

            _hudView.SetLastHitResult("Last Hit: none");
            _hudView.SetRoundResult("Round: Playing");
            _hudView.SetControlsHint("W/S move  A/D turn  Mouse aim  LMB/Space fire  R restart");
        }

        private void OnHitResolved(HitResolvedEvent hit)
        {
            if (_hudView == null || hit.Target == null)
            {
                return;
            }

            _hudView.SetLastHitResult($"Last Hit: {hit.Target.name} {hit.Result} -{hit.Damage} HP ({hit.CurrentHp}/{hit.MaxHp})");
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
            _hudView?.SetRoundResult($"Round: {label}");
            Debug.Log($"[ROUND] result={label}");
        }

        private void OnRestartClicked()
        {
            Restart();
        }

        private void Restart()
        {
            _sceneLoaderService?.ReloadActiveScene();
        }
    }
}
