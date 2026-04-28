using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Input.Mobile
{
    public sealed class MobileControlsView : MonoBehaviour
    {
        private const float JoystickSize = 164f;
        private const float JoystickRadius = 70f;
        private const float JoystickMargin = 86f;
        private const float FireButtonSize = 92f;

        [SerializeField] private MobileJoystick _movementJoystick;
        [SerializeField] private MobileJoystick _aimJoystick;
        [SerializeField] private MobileFireButton _fireButton;

        public Vector2 Movement => _movementJoystick != null ? _movementJoystick.Value : Vector2.zero;
        public Vector2 Aim => _aimJoystick != null ? _aimJoystick.Value : Vector2.zero;

        public bool ConsumeFirePressed()
        {
            return _fireButton != null && _fireButton.ConsumePressed();
        }

        public void Configure(MobileJoystick movementJoystick, MobileJoystick aimJoystick, MobileFireButton fireButton)
        {
            _movementJoystick = movementJoystick;
            _aimJoystick = aimJoystick;
            _fireButton = fireButton;
        }

        public static MobileControlsView CreateDefault(string objectName, Transform parent)
        {
            var canvasObject = new GameObject(objectName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            if (parent != null)
            {
                canvasObject.transform.SetParent(parent, false);
            }

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var view = canvasObject.AddComponent<MobileControlsView>();
            var movement = CreateJoystick(canvasObject.transform, "MovementJoystick", new Vector2(0f, 0f), new Vector2(JoystickMargin, JoystickMargin));
            var aim = CreateJoystick(canvasObject.transform, "AimJoystick", new Vector2(1f, 0f), new Vector2(-JoystickMargin, JoystickMargin));
            var fire = CreateFireButton(canvasObject.transform);
            view.Configure(movement, aim, fire);
            return view;
        }

        private static MobileJoystick CreateJoystick(Transform parent, string objectName, Vector2 anchor, Vector2 anchoredPosition)
        {
            var root = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(MobileJoystick));
            var rectTransform = (RectTransform)root.transform;
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(JoystickSize, JoystickSize);

            var background = root.GetComponent<Image>();
            background.color = new Color(0.05f, 0.08f, 0.1f, 0.42f);

            var knobObject = new GameObject("Knob", typeof(RectTransform), typeof(Image));
            var knobTransform = (RectTransform)knobObject.transform;
            knobTransform.SetParent(rectTransform, false);
            knobTransform.anchorMin = new Vector2(0.5f, 0.5f);
            knobTransform.anchorMax = new Vector2(0.5f, 0.5f);
            knobTransform.pivot = new Vector2(0.5f, 0.5f);
            knobTransform.anchoredPosition = Vector2.zero;
            knobTransform.sizeDelta = new Vector2(70f, 70f);

            var knob = knobObject.GetComponent<Image>();
            knob.color = new Color(0.75f, 0.9f, 1f, 0.72f);

            var joystick = root.GetComponent<MobileJoystick>();
            joystick.Configure(knobTransform, JoystickRadius);
            return joystick;
        }

        private static MobileFireButton CreateFireButton(Transform parent)
        {
            var buttonObject = new GameObject("FireButton", typeof(RectTransform), typeof(Image), typeof(MobileFireButton));
            var rectTransform = (RectTransform)buttonObject.transform;
            rectTransform.SetParent(parent, false);
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(-96f, 244f);
            rectTransform.sizeDelta = new Vector2(FireButtonSize, FireButtonSize);

            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(1f, 0.33f, 0.18f, 0.78f);

            var textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            var textTransform = (RectTransform)textObject.transform;
            textTransform.SetParent(rectTransform, false);
            textTransform.anchorMin = Vector2.zero;
            textTransform.anchorMax = Vector2.one;
            textTransform.offsetMin = Vector2.zero;
            textTransform.offsetMax = Vector2.zero;

            var text = textObject.GetComponent<Text>();
            text.text = "FIRE";
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.fontSize = 20;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            return buttonObject.GetComponent<MobileFireButton>();
        }
    }
}
