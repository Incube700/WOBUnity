using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Arena Config", fileName = "ArenaConfig")]
    public sealed class ArenaConfig : ScriptableObject
    {
        [SerializeField] private float _halfSize = 5f;
        [SerializeField] private float _wallThickness = 0.35f;
        [SerializeField] private Vector3 _centerObstacleSize = new Vector3(2f, 1f, 2f);
        [SerializeField] private Vector3 _playerStartPosition = new Vector3(-3.75f, 0f, -3.75f);
        [SerializeField] private Vector3 _enemyStartPosition = new Vector3(3.75f, 0f, 3.75f);

        public float HalfSize => _halfSize;
        public float WallThickness => _wallThickness;
        public Vector3 CenterObstacleSize => _centerObstacleSize;
        public Vector3 PlayerStartPosition => _playerStartPosition;
        public Vector3 EnemyStartPosition => _enemyStartPosition;
    }
}
