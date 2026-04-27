using UnityEngine;
using UnityEngine.SceneManagement;
using RicochetTanks.Infrastructure.SceneLoading;

namespace RicochetTanks.Infrastructure.Bootstrap
{
    public static class ProjectBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var activeSceneName = SceneManager.GetActiveScene().name;
            if (activeSceneName == SceneLoaderService.SandboxSceneName)
            {
                return;
            }

            SceneManager.LoadScene(SceneLoaderService.SandboxSceneName);
        }
    }
}
