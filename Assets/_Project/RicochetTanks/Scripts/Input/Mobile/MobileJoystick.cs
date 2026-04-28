using UnityEngine;
using UnityEngine.EventSystems;

namespace RicochetTanks.Input.Mobile
{
    public sealed class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _knob;
        [SerializeField] private float _radius = 70f;

        private RectTransform _rectTransform;
        private Vector2 _value;

        public Vector2 Value => _value;

        private void Awake()
        {
            ResolveReferences();
            ResetKnob();
        }

        public void Configure(RectTransform knob, float radius)
        {
            _knob = knob;
            _radius = Mathf.Max(1f, radius);
            ResolveReferences();
            ResetKnob();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateValue(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _value = Vector2.zero;
            ResetKnob();
        }

        private void ResolveReferences()
        {
            if (_rectTransform == null)
            {
                _rectTransform = (RectTransform)transform;
            }
        }

        private void UpdateValue(PointerEventData eventData)
        {
            ResolveReferences();

            if (_rectTransform == null)
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out var localPoint))
            {
                return;
            }

            var clamped = Vector2.ClampMagnitude(localPoint, _radius);
            _value = clamped / _radius;

            if (_knob != null)
            {
                _knob.anchoredPosition = clamped;
            }
        }

        private void ResetKnob()
        {
            if (_knob != null)
            {
                _knob.anchoredPosition = Vector2.zero;
            }
        }
    }
}
