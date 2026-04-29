using System;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.UI.Core;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuPresenter : IPresenter
    {
        private readonly MainMenuView _view;
        private readonly SceneLoaderService _sceneLoaderService;

        public MainMenuPresenter(MainMenuView view, SceneLoaderService sceneLoaderService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _sceneLoaderService = sceneLoaderService ?? throw new ArgumentNullException(nameof(sceneLoaderService));

            _view.PlayClicked += OnPlayClicked;
            _view.QuitClicked += OnQuitClicked;
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
            _view.QuitClicked -= OnQuitClicked;
        }

        private void OnPlayClicked()
        {
            _sceneLoaderService.Load(SceneLoaderService.DemoSceneName);
        }

        private static void OnQuitClicked()
        {
            SceneLoaderService.QuitGame();
        }
    }
}
