using System;
using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Match;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Input.Desktop;
using UnityEngine;

namespace RicochetTanks.UI.Sandbox
{
    public sealed class SandboxBootstrapper : MonoBehaviour
    {
        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        [SerializeField] private ArenaConfig _arenaConfig;
        [SerializeField] private TankConfig _tankConfig;
        [SerializeField] private ProjectileConfig _projectileConfig;
        [SerializeField] private DebugVisualizationConfig _debugVisualizationConfig;
        [SerializeField] private TankFacade _player;
        [SerializeField] private TankFacade _enemy;
        [SerializeField] private SandboxHudView _hudView;
        [SerializeField] private Camera _camera;
        [SerializeField] private DesktopInputReader _inputReader;

        private SandboxGameplayEvents _gameplayEvents;
        private SandboxHudPresenter _hudPresenter;
        private SandboxMatchController _matchController;

        private void Start()
        {
            EnsureSceneBuilt();
            BindMatch();
            BindHud();
        }

        private void OnDestroy()
        {
            _hudPresenter?.Dispose();
            _hudPresenter = null;
        }

        public void Configure(SandboxSceneContext context)
        {
            _player = context.Player;
            _enemy = context.Enemy;
            _hudView = context.HudView;
            _camera = context.Camera;
            _inputReader = context.InputReader;
            _gameplayEvents = context.GameplayEvents;
        }

        public void RebuildScene()
        {
            Configure(SandboxSceneBuilder.Build(transform, _arenaConfig, _tankConfig, _projectileConfig, _debugVisualizationConfig));
        }

        private void EnsureSceneBuilt()
        {
            if (_player != null && _enemy != null && _hudView != null && _camera != null)
            {
                return;
            }

            RebuildScene();
        }

        private void BindHud()
        {
            if (_player == null || _enemy == null || _hudView == null || _gameplayEvents == null)
            {
                throw new InvalidOperationException("Sandbox scene did not build the required player, enemy, and HUD.");
            }

            _hudPresenter?.Dispose();
            _hudPresenter = new SandboxHudPresenter(_hudView, _player.Health, _enemy.Health, _gameplayEvents, RequestRestart);
        }

        private void BindMatch()
        {
            if (_player == null || _enemy == null || _gameplayEvents == null || _inputReader == null)
            {
                throw new InvalidOperationException("Sandbox scene did not build the required match wiring.");
            }

            _matchController = GetComponent<SandboxMatchController>();
            if (_matchController == null)
            {
                _matchController = gameObject.AddComponent<SandboxMatchController>();
            }

            _matchController.Configure(_player, _enemy, _gameplayEvents, _inputReader, _sceneLoaderService);
        }

        private void RequestRestart()
        {
            _matchController?.RequestRestart();
        }
    }
}
