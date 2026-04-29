using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Input.Mobile
{
    public sealed class MobileControlsView : MonoBehaviour
    {
        private const float JoystickSize = 300f;
        private const float JoystickKnobSize = 130f;
        private const float JoystickRadius = 125f;
        private const float JoystickHorizontalOffset = 240f;
        private const float JoystickVerticalOffset = 240f;
        private const float FireButtonHorizontalOffset = 170f;
        private const float FireButtonVerticalOffset = 440f;
        private const float FireButtonSize = 210f;

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
            canvas.sortingOrder = 50;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var view = canvasObject.AddComponent<MobileControlsView>();
            var movement = CreateJoystick(
                canvasObject.transform,
                "MovementJoystick",
                "MOVE",
                new Vector2(0f, 0f),
                new Vector2(JoystickHorizontalOffset, JoystickVerticalOffset));
            var aim = CreateJoystick(
                canvasObject.transform,
                "AimJoystick",
                "AIM",
                new Vector2(1f, 0f),
                new Vector2(-JoystickHorizontalOffset, JoystickVerticalOffset));
            var fire = CreateFireButton(canvasObject.transform);
            view.Configure(movement, aim, fire);
            return view;
        }

        private static MobileJoystick CreateJoystick(Transform parent, string objectName, string labelText, Vector2 anchor, Vector2 anchoredPosition)
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
            background.color = new Color(0.04f, 0.07f, 0.09f, 0.74f);

            var knobObject = new GameObject("Knob", typeof(RectTransform), typeof(Image));
            var knobTransform = (RectTransform)knobObject.transform;
            knobTransform.SetParent(rectTransform, false);
            knobTransform.anchorMin = new Vector2(0.5f, 0.5f);
            knobTransform.anchorMax = new Vector2(0.5f, 0.5f);
            knobTransform.pivot = new Vector2(0.5f, 0.5f);
            knobTransform.anchoredPosition = Vector2.zero;
            knobTransform.sizeDelta = new Vector2(JoystickKnobSize, JoystickKnobSize);

            var knob = knobObject.GetComponent<Image>();
            knob.color = new Color(0.75f, 0.92f, 1f, 0.92f);

            CreateLabel(rectTransform, labelText, new Vector2(0f, -JoystickSize * 0.58f), 34);

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
            rectTransform.anchoredPosition = new Vector2(-FireButtonHorizontalOffset, FireButtonVerticalOffset);
            rectTransform.sizeDelta = new Vector2(FireButtonSize, FireButtonSize);

            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(1f, 0.28f, 0.13f, 0.9f);

            var text = CreateLabel(rectTransform, "FIRE", Vector2.zero, 42);
            Stretch(text.rectTransform);

            return buttonObject.GetComponent<MobileFireButton>();
        }

        private static Text CreateLabel(RectTransform parent, string textValue, Vector2 anchoredPosition, int fontSize)
        {
            var textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            var textTransform = (RectTransform)textObject.transform;
            textTransform.SetParent(parent, false);
            textTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textTransform.pivot = new Vector2(0.5f, 0.5f);
            textTransform.anchoredPosition = anchoredPosition;
            textTransform.sizeDelta = new Vector2(180f, 52f);

            var text = textObject.GetComponent<Text>();
            text.text = textValue;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 1f, 1f, 0.92f);
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
