using UnityEngine;

namespace RicochetTanks.Features.UI.Infrastructure
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaRoot : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            if (_lastSafeArea == Screen.safeArea && _lastScreenSize == screenSize)
            {
                return;
            }

            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            if (Screen.width > 0)
            {
                anchorMin.x /= Screen.width;
                anchorMax.x /= Screen.width;
            }

            if (Screen.height > 0)
            {
                anchorMin.y /= Screen.height;
                anchorMax.y /= Screen.height;
            }

            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            _lastSafeArea = safeArea;
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        }
    }
}
