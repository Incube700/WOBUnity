using RicochetTanks.Infrastructure.SceneLoading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuBootstrapper : MonoBehaviour
    {
        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        private MainMenuPresenter _presenter;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneLoadedCallback()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInitialMainMenuObjects()
        {
            TryCreateForScene(SceneManager.GetActiveScene().name);
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TryCreateForScene(scene.name);
        }

        private static void TryCreateForScene(string sceneName)
        {
            if (sceneName != SceneLoaderService.MainMenuSceneName)
            {
                return;
            }

            if (FindAnyObjectByType<MainMenuBootstrapper>() != null)
            {
                return;
            }

            var bootstrapperObject = new GameObject(nameof(MainMenuBootstrapper));
            bootstrapperObject.AddComponent<MainMenuBootstrapper>();
        }

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

            var canvas = UiFactory.CreateCanvas("MainMenuCanvas");
            var title = UiFactory.CreateText(canvas.transform, "Title", new Vector2(0f, 120f), new Vector2(420f, 55f), TextAnchor.MiddleCenter);
            title.text = "Ricochet Tanks";
            title.fontSize = 32;

            var playButton = UiFactory.CreateButton(canvas.transform, "Play Sandbox", new Vector2(0f, 30f), null);
            var quitButton = UiFactory.CreateButton(canvas.transform, "Quit", new Vector2(0f, -30f), null);

            var view = canvas.gameObject.AddComponent<MainMenuView>();
            view.Configure(playButton, quitButton);
            _presenter?.Dispose();
            _presenter = new MainMenuPresenter(view, _sceneLoaderService);
        }
    }
}
