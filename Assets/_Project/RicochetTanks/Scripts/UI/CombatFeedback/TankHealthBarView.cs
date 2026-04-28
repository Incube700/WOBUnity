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
        [SerializeField] private Image _fillImage;
        [SerializeField] private Text _hpText;

        private Camera _camera;

        private void Awake()
        {
            ResolveReferences();
            PrepareFillImage();
        }

        private void LateUpdate()
        {
            UpdatePosition();
            FaceCamera();
        }

        public void Configure(Image fillImage, Text hpText, Canvas canvas)
        {
            _fillImage = fillImage;
            _hpText = hpText;
            _canvas = canvas;
            PrepareFillImage();
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

            if (_fillImage != null)
            {
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

            if (_hpText == null)
            {
                _hpText = GetComponentInChildren<Text>(true);
            }
        }

        private void PrepareFillImage()
        {
            if (_fillImage == null)
            {
                return;
            }

            _fillImage.type = Image.Type.Filled;
            _fillImage.fillMethod = Image.FillMethod.Horizontal;
            _fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
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

            return images.Length > 0 ? images[0] : null;
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
