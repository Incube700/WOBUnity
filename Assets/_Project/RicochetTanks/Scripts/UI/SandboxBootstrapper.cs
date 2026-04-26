using RicochetTanks.Gameplay;
using RicochetTanks.Infrastructure;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RicochetTanks.UI
{
    public class SandboxBootstrapper : MonoBehaviour
    {
        public const string SandBoxSceneName = "Sand Box";

        private const string SandboxSceneName = "RicochetTanks_Sandbox";

        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        [SerializeField] private TankFacade _player;
        [SerializeField] private TankFacade _enemy;
        [SerializeField] private SandboxHudView _hudView;
        [SerializeField] private Camera _camera;

        private SandboxHudPresenter _hudPresenter;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureSandboxObjects()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName != SandboxSceneName && sceneName != SandBoxSceneName)
            {
                return;
            }

            if (FindAnyObjectByType<SandboxBootstrapper>() != null)
            {
                return;
            }

            var bootstrapperObject = new GameObject(nameof(SandboxBootstrapper));
            bootstrapperObject.AddComponent<SandboxBootstrapper>();
        }

        private void Start()
        {
            EnsureSceneBuilt();
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
        }

        public void RebuildScene()
        {
            Configure(SandboxSceneBuilder.Build(transform));
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
            if (_player == null || _enemy == null || _hudView == null)
            {
                throw new InvalidOperationException("Sand Box scene did not build the required player, enemy, and HUD.");
            }

            _hudPresenter?.Dispose();
            _hudPresenter = new SandboxHudPresenter(_hudView, _player.Health, _enemy.Health, OnRestartClicked);
        }

        private void OnRestartClicked()
        {
            _sceneLoaderService.ReloadActiveScene();
        }
    }
}
