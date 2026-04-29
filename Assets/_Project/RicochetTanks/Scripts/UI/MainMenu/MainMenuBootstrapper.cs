using RicochetTanks.Infrastructure.SceneLoading;
using UnityEngine;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuBootstrapper : MonoBehaviour
    {
        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        [SerializeField] private MainMenuView _view;

        private MainMenuPresenter _presenter;

        private void Start()
        {
            BuildMainMenu();
        }

        private void OnDestroy()
        {
            _presenter?.Dispose();
            _presenter = null;
        }

        private void BuildMainMenu()
        {
            UiFactory.EnsureEventSystem("Main Menu EventSystem");

            _view = _view != null ? _view : new MainMenuViewFactory().CreateFallbackView();

            _presenter?.Dispose();
            _presenter = new MainMenuPresenter(_view, _sceneLoaderService);
        }
    }
}
