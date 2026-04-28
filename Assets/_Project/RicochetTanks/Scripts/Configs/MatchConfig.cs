using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Match Config", fileName = "MatchConfig")]
    public sealed class MatchConfig : ScriptableObject
    {
        [SerializeField] private string _playingLabel = "Round: Playing";
        [SerializeField] private string _playerWinsLabel = "Player Wins";
        [SerializeField] private string _enemyWinsLabel = "Enemy Wins";
        [SerializeField] private string _drawLabel = "Draw";
        [SerializeField] private float _resultDelaySeconds = 3f;
        [SerializeField] private bool _allowWinnerControlDuringResultDelay = true;
        [SerializeField] private bool _disableShootingAfterMatchDecided = true;
        [SerializeField] private bool _keepDeadWreck = true;

        public string PlayingLabel => _playingLabel;
        public string PlayerWinsLabel => _playerWinsLabel;
        public string EnemyWinsLabel => _enemyWinsLabel;
        public string DrawLabel => _drawLabel;
        public float ResultDelaySeconds => Mathf.Max(0f, _resultDelaySeconds);
        public bool AllowWinnerControlDuringResultDelay => _allowWinnerControlDuringResultDelay;
        public bool DisableShootingAfterMatchDecided => _disableShootingAfterMatchDecided;
        public bool KeepDeadWreck => _keepDeadWreck;
    }
}
