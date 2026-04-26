using UnityEngine.SceneManagement;

namespace RicochetTanks.Infrastructure.SceneLoading
{
    public sealed class SceneLoaderService
    {
        public void Load(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ReloadActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }
    }
}
