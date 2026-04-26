using UnityEngine;
using UnityEngine.SceneManagement;

namespace RicochetTanks.Infrastructure
{
    public static class ProjectBootstrapper
    {
        private const string MainMenuSceneName = "RicochetTanks_MainMenu";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (SceneManager.GetActiveScene().name == MainMenuSceneName)
            {
                return;
            }

            SceneManager.LoadScene(MainMenuSceneName);
        }
    }
}
