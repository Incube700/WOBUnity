using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    [DisallowMultipleComponent]
    public sealed class CombatVfxLifetimeView : MonoBehaviour
    {
        private const float MinLifetime = 0.05f;

        private float _lifetime = 1f;
        private float _elapsed;
        private bool _isPlaying;

        public void Play(float lifetime)
        {
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
            if (_elapsed >= _lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
