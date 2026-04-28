using System.Collections;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    [DisallowMultipleComponent]
    public sealed class ShotRecoilView : MonoBehaviour
    {
        private Vector3 _originLocalPosition;
        private Coroutine _routine;
        private bool _hasOrigin;

        public void Play(float distance, float duration)
        {
            if (distance <= 0f || duration <= 0f)
            {
                return;
            }

            if (!_hasOrigin)
            {
                _originLocalPosition = transform.localPosition;
                _hasOrigin = true;
            }

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(Animate(distance, duration));
        }

        private void OnDisable()
        {
            if (_hasOrigin)
            {
                transform.localPosition = _originLocalPosition;
            }
        }

        private IEnumerator Animate(float distance, float duration)
        {
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var progress = Mathf.Clamp01(elapsed / duration);
                var recoil = Mathf.Sin(progress * Mathf.PI) * distance;
                transform.localPosition = _originLocalPosition + Vector3.back * recoil;
                yield return null;
            }

            transform.localPosition = _originLocalPosition;
            _routine = null;
        }
    }
}
