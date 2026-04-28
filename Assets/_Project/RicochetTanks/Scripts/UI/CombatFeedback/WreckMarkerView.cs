using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    [DisallowMultipleComponent]
    public sealed class WreckMarkerView : MonoBehaviour
    {
        private const float MinLifetime = 0.1f;

        private Material[] _materials;
        private Color[] _baseColors;
        private Vector3 _startPosition;
        private float _lifetime = 4f;
        private float _elapsed;
        private bool _isPlaying;

        public void Play(float lifetime)
        {
            ResolveMaterials();

            _startPosition = transform.position;
            _lifetime = Mathf.Max(MinLifetime, lifetime);
            _elapsed = 0f;
            _isPlaying = true;
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            var progress = Mathf.Clamp01(_elapsed / _lifetime);

            transform.position = _startPosition + Vector3.up * (0.2f * progress);
            SetAlpha(1f - progress);

            if (_elapsed >= _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_materials == null)
            {
                return;
            }

            for (var index = 0; index < _materials.Length; index++)
            {
                if (_materials[index] != null)
                {
                    Destroy(_materials[index]);
                }
            }
        }

        private void ResolveMaterials()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            _materials = new Material[renderers.Length];
            _baseColors = new Color[renderers.Length];

            for (var index = 0; index < renderers.Length; index++)
            {
                _materials[index] = renderers[index].material;
                _baseColors[index] = ResolveColor(_materials[index]);
            }
        }

        private void SetAlpha(float alpha)
        {
            if (_materials == null || _baseColors == null)
            {
                return;
            }

            for (var index = 0; index < _materials.Length; index++)
            {
                var material = _materials[index];
                if (material == null)
                {
                    continue;
                }

                var color = _baseColors[index];
                color.a *= alpha;

                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", color);
                }

                if (material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", color);
                }
            }
        }

        private static Color ResolveColor(Material material)
        {
            if (material.HasProperty("_BaseColor"))
            {
                return material.GetColor("_BaseColor");
            }

            return material.HasProperty("_Color") ? material.GetColor("_Color") : Color.white;
        }
    }
}
