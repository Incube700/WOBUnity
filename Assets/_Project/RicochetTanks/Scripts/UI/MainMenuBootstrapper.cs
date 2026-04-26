using RicochetTanks.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RicochetTanks.UI
{
    public class MainMenuBootstrapper : MonoBehaviour
    {
        private const string MainMenuSceneName = "RicochetTanks_MainMenu";
        private const string SandboxSceneName = "RicochetTanks_Sandbox";

        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureMainMenuObjects()
        {
            if (SceneManager.GetActiveScene().name != MainMenuSceneName)
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

        private void BuildMainMenu()
        {
            var canvas = UiFactory.CreateCanvas("MainMenuCanvas");
            UiFactory.CreateButton(canvas.transform, "Play Sandbox", new Vector2(0, 30), OnPlaySandboxClicked);
            UiFactory.CreateButton(canvas.transform, "Quit", new Vector2(0, -30), OnQuitClicked);
        }

        private void OnPlaySandboxClicked()
        {
            _sceneLoaderService.Load(SandboxSceneName);
        }

        private static void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
