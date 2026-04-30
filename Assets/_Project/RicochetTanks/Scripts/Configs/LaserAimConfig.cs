using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Laser Aim Config", fileName = "LaserAimConfig")]
    public sealed class LaserAimConfig : ScriptableObject
    {
        private const int DefaultCollisionMask = (1 << 0) | (1 << 8) | (1 << 9) | (1 << 11);

        [SerializeField] private bool _showPlayerLaser = true;
        [SerializeField] private bool _showEnemyLaser;
        [SerializeField] private float _maxDistance = 20f;
        [SerializeField] private float _width = 0.025f;
        [SerializeField] private Color _color = new Color(1f, 0.12f, 0.08f, 0.35f);
        [SerializeField] private LayerMask _collisionMask = DefaultCollisionMask;
        [SerializeField] private float _startOffset = 0.15f;
        [SerializeField] private Material _material;

        public bool ShowPlayerLaser => _showPlayerLaser;
        public bool ShowEnemyLaser => _showEnemyLaser;
        public float MaxDistance => Mathf.Max(0.1f, _maxDistance);
        public float Width => Mathf.Max(0.001f, _width);
        public Color Color => _color;
        public LayerMask CollisionMask => _collisionMask;
        public float StartOffset => Mathf.Max(0f, _startOffset);
        public Material Material => _material;
    }
}
