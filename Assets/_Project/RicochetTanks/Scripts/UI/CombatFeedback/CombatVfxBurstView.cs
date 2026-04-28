using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    [DisallowMultipleComponent]
    public sealed class CombatVfxBurstView : MonoBehaviour
    {
        private const float MinLifetime = 0.05f;

        private Renderer _renderer;
        private Material _material;
        private Color _color;
        private float _targetScale = 0.3f;
        private float _lifetime = 0.3f;
        private float _elapsed;
        private bool _isPlaying;

        public void Play(Color color, float targetScale, float lifetime)
        {
            ResolveRenderer();

            _color = color;
            _targetScale = Mathf.Max(0.01f, targetScale);
            _lifetime = Mathf.Max(MinLifetime, lifetime);
            _elapsed = 0f;
            _isPlaying = true;

            transform.localScale = Vector3.one * (_targetScale * 0.25f);
            SetColor(_color);
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            var progress = Mathf.Clamp01(_elapsed / _lifetime);
            var eased = 1f - ((1f - progress) * (1f - progress));

            transform.localScale = Vector3.one * Mathf.Lerp(_targetScale * 0.25f, _targetScale, eased);

            var color = _color;
            color.a *= 1f - progress;
            SetColor(color);

            if (_elapsed >= _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }

        private void ResolveRenderer()
        {
            if (_renderer == null)
            {
                _renderer = GetComponentInChildren<Renderer>(true);
            }

            if (_renderer != null && _material == null)
            {
                _material = _renderer.material;
            }
        }

        private void SetColor(Color color)
        {
            if (_material == null)
            {
                return;
            }

            if (_material.HasProperty("_BaseColor"))
            {
                _material.SetColor("_BaseColor", color);
            }

            if (_material.HasProperty("_Color"))
            {
                _material.SetColor("_Color", color);
            }
        }
    }
}
