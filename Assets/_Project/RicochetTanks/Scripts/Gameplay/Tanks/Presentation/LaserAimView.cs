using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks.Presentation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class LaserAimView : MonoBehaviour
    {
        private static Material _fallbackMaterial;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private LaserAimConfig _config;

        private readonly RaycastHit[] _raycastHits = new RaycastHit[16];
        private TankHealth _health;
        private Transform _ownerRoot;
        private bool _isConfigured;

        public void Configure(Transform muzzle, LaserAimConfig config, TankHealth health, Transform ownerRoot)
        {
            Unsubscribe();

            _muzzle = muzzle;
            _config = config;
            _health = health;
            _ownerRoot = ownerRoot;
            _isConfigured = _muzzle != null && _config != null;

            EnsureLineRenderer();
            ApplyConfig();

            if (_health != null)
            {
                _health.Died += OnTankDied;
                SetVisible(_health.IsAlive);
            }
            else
            {
                SetVisible(_isConfigured);
            }
        }

        private void Awake()
        {
            EnsureLineRenderer();
        }

        private void OnEnable()
        {
            SetVisible(_isConfigured && (_health == null || _health.IsAlive));
        }

        private void OnDisable()
        {
            SetVisible(false);
        }

        private void LateUpdate()
        {
            if (!_isConfigured || _lineRenderer == null || _muzzle == null)
            {
                return;
            }

            var direction = _muzzle.forward;
            if (direction.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            direction.Normalize();

            var start = _muzzle.position;
            var rayOrigin = start + direction * _config.StartOffset;
            var end = rayOrigin + direction * _config.MaxDistance;

            if (TryGetClosestHit(rayOrigin, direction, out var hit))
            {
                end = hit.point;
            }

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }

        private bool TryGetClosestHit(Vector3 origin, Vector3 direction, out RaycastHit closestHit)
        {
            closestHit = default;
            var hitCount = Physics.RaycastNonAlloc(
                origin,
                direction,
                _raycastHits,
                _config.MaxDistance,
                _config.CollisionMask,
                QueryTriggerInteraction.Ignore);

            var closestDistance = float.MaxValue;
            var hasHit = false;

            for (var index = 0; index < hitCount; index++)
            {
                var currentHit = _raycastHits[index];
                if (IsOwnerCollider(currentHit.collider) || currentHit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = currentHit.distance;
                closestHit = currentHit;
                hasHit = true;
            }

            return hasHit;
        }

        private bool IsOwnerCollider(Collider hitCollider)
        {
            return hitCollider != null && _ownerRoot != null && hitCollider.transform.IsChildOf(_ownerRoot);
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void OnTankDied(TankHealth health)
        {
            SetVisible(false);
        }

        private void EnsureLineRenderer()
        {
            if (_lineRenderer != null)
            {
                return;
            }

            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void ApplyConfig()
        {
            if (_lineRenderer == null || _config == null)
            {
                return;
            }

            _lineRenderer.positionCount = 2;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.startWidth = _config.Width;
            _lineRenderer.endWidth = _config.Width;
            _lineRenderer.startColor = _config.Color;
            _lineRenderer.endColor = new Color(_config.Color.r, _config.Color.g, _config.Color.b, 0f);
            _lineRenderer.numCapVertices = 2;
            _lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lineRenderer.receiveShadows = false;
            var material = _config.Material != null ? _config.Material : GetFallbackMaterial();
            ApplyMaterialColor(material, _config.Color);
            _lineRenderer.material = material;
        }

        private void SetVisible(bool isVisible)
        {
            if (_lineRenderer != null)
            {
                _lineRenderer.enabled = isVisible;
            }
        }

        private void Unsubscribe()
        {
            if (_health != null)
            {
                _health.Died -= OnTankDied;
                _health = null;
            }
        }

        private static void ApplyMaterialColor(Material material, Color color)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }
        }

        private static Material GetFallbackMaterial()
        {
            if (_fallbackMaterial != null)
            {
                return _fallbackMaterial;
            }

            var shader = Shader.Find("Sprites/Default");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            _fallbackMaterial = new Material(shader);
            return _fallbackMaterial;
        }
    }
}
