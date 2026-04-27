using UnityEngine.SceneManagement;

namespace RicochetTanks.Infrastructure.SceneLoading
{
    public sealed class SceneLoaderService
    {
        public const string BootstrapSceneName = "Bootstrap";
        public const string MainMenuSceneName = "MainMenu";
        public const string SandboxSceneName = "Sandbox";
        public const string DemoSceneName = "RicochetTanks_Demo";

        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ReloadActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}
