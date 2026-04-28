using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Combat VFX Config", fileName = "CombatVfxConfig")]
    public sealed class CombatVfxConfig : ScriptableObject
    {
        [Header("Prefab Slots")]
        [SerializeField] private GameObject _worldImpactVfxPrefab;
        [SerializeField] private GameObject _tankPenetrationVfxPrefab;
        [SerializeField] private GameObject _noPenetrationVfxPrefab;
        [SerializeField] private GameObject _ricochetSparkVfxPrefab;
        [SerializeField] private GameObject _tankDeathExplosionVfxPrefab;
        [SerializeField] private GameObject _wreckMarkerPrefab;
        [SerializeField] private GameObject _smokeVfxPrefab;

        [Header("Projectile Trail")]
        [SerializeField] private float _projectileTrailLifetime = 0.28f;
        [SerializeField] private float _projectileTrailWidth = 0.16f;
        [SerializeField] private Color _projectileTrailStartColor = new Color(1f, 0.92f, 0.18f, 1f);
        [SerializeField] private Color _projectileTrailEndColor = new Color(1f, 0.2f, 0.05f, 0f);

        [Header("Timing")]
        [SerializeField] private float _impactLifetime = 0.35f;
        [SerializeField] private float _ricochetSparkLifetime = 0.28f;
        [SerializeField] private float _deathExplosionLifetime = 0.65f;
        [SerializeField] private float _wreckLifetime = 4f;
        [SerializeField] private float _smokeLifetime = 3f;

        [Header("Scale / Placement")]
        [SerializeField] private float _vfxScale = 0.35f;
        [SerializeField] private float _wreckScale = 1f;
        [SerializeField] private Vector3 _wreckSmokeOffset = new Vector3(0f, 0.72f, 0f);
        [SerializeField] private float _effectNormalOffset = 0.06f;

        [Header("Fallback Colors")]
        [SerializeField] private Color _impactColor = new Color(1f, 0.36f, 0.16f, 0.9f);
        [SerializeField] private Color _noPenetrationColor = new Color(0.78f, 0.82f, 0.86f, 0.9f);
        [SerializeField] private Color _ricochetColor = new Color(1f, 0.82f, 0.22f, 0.95f);
        [SerializeField] private Color _deathExplosionColor = new Color(1f, 0.24f, 0.08f, 0.9f);
        [SerializeField] private Color _wreckColor = new Color(0.08f, 0.08f, 0.075f, 1f);
        [SerializeField] private Color _smokeColor = new Color(0.22f, 0.22f, 0.22f, 0.62f);

        [Header("Shot Recoil")]
        [SerializeField] private float _shotRecoilDistance = 0.16f;
        [SerializeField] private float _shotRecoilDuration = 0.12f;

        public GameObject WorldImpactVfxPrefab => _worldImpactVfxPrefab;
        public GameObject TankPenetrationVfxPrefab => _tankPenetrationVfxPrefab;
        public GameObject NoPenetrationVfxPrefab => _noPenetrationVfxPrefab;
        public GameObject RicochetSparkVfxPrefab => _ricochetSparkVfxPrefab;
        public GameObject TankDeathExplosionVfxPrefab => _tankDeathExplosionVfxPrefab;
        public GameObject WreckMarkerPrefab => _wreckMarkerPrefab;
        public GameObject SmokeVfxPrefab => _smokeVfxPrefab;
        public float ProjectileTrailLifetime => Mathf.Max(0.02f, _projectileTrailLifetime);
        public float ProjectileTrailWidth => Mathf.Max(0.001f, _projectileTrailWidth);
        public Color ProjectileTrailStartColor => _projectileTrailStartColor;
        public Color ProjectileTrailEndColor => _projectileTrailEndColor;
        public float ImpactLifetime => Mathf.Max(0.05f, _impactLifetime);
        public float RicochetSparkLifetime => Mathf.Max(0.05f, _ricochetSparkLifetime);
        public float DeathExplosionLifetime => Mathf.Max(0.05f, _deathExplosionLifetime);
        public float WreckLifetime => Mathf.Max(0.1f, _wreckLifetime);
        public float SmokeLifetime => Mathf.Max(0.05f, _smokeLifetime);
        public float VfxScale => Mathf.Max(0.01f, _vfxScale);
        public float WreckScale => Mathf.Max(0.1f, _wreckScale);
        public Vector3 WreckSmokeOffset => _wreckSmokeOffset;
        public float EffectNormalOffset => Mathf.Max(0f, _effectNormalOffset);
        public Color ImpactColor => _impactColor;
        public Color NoPenetrationColor => _noPenetrationColor;
        public Color RicochetColor => _ricochetColor;
        public Color DeathExplosionColor => _deathExplosionColor;
        public Color WreckColor => _wreckColor;
        public Color SmokeColor => _smokeColor;
        public float ShotRecoilDistance => Mathf.Max(0f, _shotRecoilDistance);
        public float ShotRecoilDuration => Mathf.Max(0f, _shotRecoilDuration);
    }
}
