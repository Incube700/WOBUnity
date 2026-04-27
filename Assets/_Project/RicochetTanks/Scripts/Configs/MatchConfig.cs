using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Match Config", fileName = "MatchConfig")]
    public sealed class MatchConfig : ScriptableObject
    {
        [SerializeField] private string _playingLabel = "Round: Playing";
        [SerializeField] private string _playerWinsLabel = "Player Wins";
        [SerializeField] private string _enemyWinsLabel = "Enemy Wins";

        public string PlayingLabel => _playingLabel;
        public string PlayerWinsLabel => _playerWinsLabel;
        public string EnemyWinsLabel => _enemyWinsLabel;
    }
}
