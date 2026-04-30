using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Local Session Config", fileName = "LocalSessionConfig")]
    public sealed class LocalSessionConfig : ScriptableObject
    {
        [SerializeField] private int _roundsToWin = 3;
        [SerializeField] private float _roundBreakSeconds = 5f;
        [SerializeField] private string _roundOneScene = "RicochetTanks_Demo";
        [SerializeField] private string _roundTwoScene = "RicochetTanks_Demo";
        [SerializeField] private string _roundThreeScene = "RicochetTanks_Demo";

        public int RoundsToWin { get { return Mathf.Max(1, _roundsToWin); } }
        public float RoundBreakSeconds { get { return Mathf.Max(0f, _roundBreakSeconds); } }
        public string RoundOneScene { get { return _roundOneScene; } }
        public string RoundTwoScene { get { return _roundTwoScene; } }
        public string RoundThreeScene { get { return _roundThreeScene; } }

        public string GetSceneForRound(int roundNumber, string fallbackScene)
        {
            var sceneName = fallbackScene;

            if (roundNumber == 1 && !string.IsNullOrWhiteSpace(_roundOneScene))
            {
                sceneName = _roundOneScene;
            }
            else if (roundNumber == 2 && !string.IsNullOrWhiteSpace(_roundTwoScene))
            {
                sceneName = _roundTwoScene;
            }
            else if (roundNumber == 3 && !string.IsNullOrWhiteSpace(_roundThreeScene))
            {
                sceneName = _roundThreeScene;
            }

            return string.IsNullOrWhiteSpace(sceneName) ? fallbackScene : sceneName;
        }
    }
}
