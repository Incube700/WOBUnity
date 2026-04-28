using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.CombatFeedback
{
    [DisallowMultipleComponent]
    public sealed class FloatingHitTextView : MonoBehaviour
    {
        private const float MinDuration = 0.1f;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Text _text;
        [SerializeField] private float _floatDistance = 0.65f;

        private Camera _camera;
        private Vector3 _startPosition;
        private Vector3 _floatDirection = Vector3.up;
        private Color _textColor = Color.white;
        private float _duration = 1f;
        private float _elapsed;
        private bool _isPlaying;

        private void Awake()
        {
            ResolveReferences();
        }

        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            var progress = Mathf.Clamp01(_elapsed / _duration);

            transform.position = _startPosition + _floatDirection * (_floatDistance * progress);
            SetAlpha(1f - progress);

            if (_elapsed >= _duration)
            {
                Destroy(gameObject);
            }
        }

        private void LateUpdate()
        {
            FaceCamera();
        }

        public void Configure(Text text, CanvasGroup canvasGroup, Canvas canvas)
        {
            _text = text;
            _canvasGroup = canvasGroup;
            _canvas = canvas;
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

        public void Play(string text, Color color, float duration)
        {
            ResolveReferences();

            _startPosition = transform.position;
            _floatDirection = ResolveFloatDirection();
            _textColor = color;
            _duration = Mathf.Max(MinDuration, duration);
            _elapsed = 0f;
            _isPlaying = true;

            if (_text != null)
            {
                _text.text = text;
                _text.color = _textColor;
            }

            SetAlpha(1f);
            FaceCamera();
        }

        private void ResolveReferences()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            if (_text == null)
            {
                _text = GetComponentInChildren<Text>(true);
            }
        }

        private Vector3 ResolveFloatDirection()
        {
            if (_camera == null || _camera.transform.up.sqrMagnitude < 0.001f)
            {
                return Vector3.up;
            }

            return _camera.transform.up.normalized;
        }

        private void FaceCamera()
        {
            if (_camera == null)
            {
                return;
            }

            transform.rotation = _camera.transform.rotation;
        }

        private void SetAlpha(float alpha)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = alpha;
                return;
            }

            if (_text == null)
            {
                return;
            }

            _textColor.a = alpha;
            _text.color = _textColor;
        }
    }
}
