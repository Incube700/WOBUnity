using UnityEngine;
using UnityEngine.SceneManagement;

namespace RicochetTanks.Infrastructure
{
    public static class ProjectBootstrapper
    {
        private const string MainMenuSceneName = "RicochetTanks_MainMenu";
        private const string LegacySandboxSceneName = "RicochetTanks_Sandbox";
        private const string SandBoxSceneName = "Sand Box";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var activeSceneName = SceneManager.GetActiveScene().name;
            if (activeSceneName == MainMenuSceneName || activeSceneName == LegacySandboxSceneName || activeSceneName == SandBoxSceneName)
            {
                return;
            }

            SceneManager.LoadScene(MainMenuSceneName);
        }
    }
}
