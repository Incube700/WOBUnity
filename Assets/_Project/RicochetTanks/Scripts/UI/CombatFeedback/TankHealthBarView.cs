using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.CombatFeedback
{
    [DisallowMultipleComponent]
    public sealed class TankHealthBarView : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _worldOffset = new Vector3(0f, 1.35f, 0f);
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _fillRect;
        [SerializeField] private Image _fillImage;
        [SerializeField] private Text _hpText;

        private Camera _camera;
        private bool _didWarnMissingFill;

        private void Awake()
        {
            ResolveReferences();
            PrepareFill();
        }

        private void LateUpdate()
        {
            UpdatePosition();
            FaceCamera();
        }

        public void Configure(Image fillImage, Text hpText, Canvas canvas)
        {
            _fillImage = fillImage;
            _fillRect = fillImage != null ? fillImage.rectTransform : null;
            _hpText = hpText;
            _canvas = canvas;
            PrepareFill();
        }

        public void SetCamera(Camera camera)
        {
            _camera = camera;
            ResolveReferences();

            if (_canvas != null)
            {
                _canvas.worldCamera = _camera;
            }
        }

        public void Follow(Transform target)
        {
            _target = target;
            UpdatePosition();
        }

        public void SetHealth(float currentHp, float maxHp)
        {
            var safeMaxHp = Mathf.Max(1f, maxHp);
            var normalizedHp = Mathf.Clamp01(currentHp / safeMaxHp);

            if (_fillRect == null && _fillImage != null)
            {
                _fillRect = _fillImage.rectTransform;
            }

            if (_fillRect != null)
            {
                _fillRect.anchorMax = new Vector2(normalizedHp, 1f);
            }
            else
            {
                WarnMissingFill();
            }

            if (_fillImage != null)
            {
                _fillImage.type = Image.Type.Simple;
                _fillImage.fillAmount = normalizedHp;
            }

            if (_hpText != null)
            {
                _hpText.text = $"{Format(currentHp)}/{Format(maxHp)}";
            }
        }

        private void UpdatePosition()
        {
            if (_target == null)
            {
                return;
            }

            transform.position = _target.position + _worldOffset;
        }

        private void FaceCamera()
        {
            if (_camera == null)
            {
                return;
            }

            transform.rotation = _camera.transform.rotation;
        }

        private void ResolveReferences()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }

            if (_fillImage == null)
            {
                _fillImage = FindImage("Fill");
            }

            if (_fillRect == null && _fillImage != null)
            {
                _fillRect = _fillImage.rectTransform;
            }

            if (_hpText == null)
            {
                _hpText = GetComponentInChildren<Text>(true);
            }
        }

        private void PrepareFill()
        {
            if (_fillRect == null && _fillImage != null)
            {
                _fillRect = _fillImage.rectTransform;
            }

            if (_fillRect != null)
            {
                _fillRect.anchorMin = new Vector2(0f, 0f);
                _fillRect.anchorMax = new Vector2(1f, 1f);
                _fillRect.offsetMin = Vector2.zero;
                _fillRect.offsetMax = Vector2.zero;
                _fillRect.pivot = new Vector2(0f, 0.5f);
            }

            if (_fillRect == null && _fillImage == null)
            {
                WarnMissingFill();
                return;
            }

            if (_fillImage != null)
            {
                _fillImage.type = Image.Type.Simple;
            }
        }

        private void WarnMissingFill()
        {
            if (_didWarnMissingFill)
            {
                return;
            }

            Debug.LogWarning("[HP_BAR] Missing fill reference on WorldHealthBarPrefab");
            _didWarnMissingFill = true;
        }

        private Image FindImage(string objectName)
        {
            var images = GetComponentsInChildren<Image>(true);
            for (var index = 0; index < images.Length; index++)
            {
                if (images[index].name == objectName)
                {
                    return images[index];
                }
            }

            return null;
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
