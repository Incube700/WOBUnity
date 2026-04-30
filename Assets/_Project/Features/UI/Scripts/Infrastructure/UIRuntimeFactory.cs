using RicochetTanks.Features.UI.Configs;
using RicochetTanks.Features.UI.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Infrastructure
{
    public static class UIRuntimeFactory
    {
        public static UIRootSet CreateAndroidCanvas(string name, UIThemeConfig theme)
        {
            EnsureEventSystem();

            var canvasObject = new GameObject(name);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = theme != null ? theme.ReferenceResolution : new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            var safeArea = CreateStretchRect("SafeAreaRoot", canvas.transform);
            safeArea.gameObject.AddComponent<SafeAreaRoot>();

            var screensRoot = CreateStretchRect("ScreensRoot", safeArea);
            var popupsRoot = CreateStretchRect("PopupsRoot", safeArea);
            var overlayRoot = CreateStretchRect("OverlayRoot", safeArea);

            return new UIRootSet(canvas, safeArea, screensRoot, popupsRoot, overlayRoot);
        }

        public static RectTransform CreateStretchRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            return rectTransform;
        }

        public static RectTransform CreatePanel(Transform parent, string name, UIThemeConfig theme)
        {
            var rectTransform = CreateStretchRect(name, parent);
            var image = rectTransform.gameObject.AddComponent<Image>();
            image.color = theme != null ? theme.PanelColor : new Color(0.12f, 0.13f, 0.14f, 0.96f);
            image.sprite = theme != null ? theme.PanelSprite : null;
            image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            return rectTransform;
        }

        public static RectTransform CreateFixedPanel(Transform parent, string name, Vector2 size, UIThemeConfig theme)
        {
            var rectTransform = CreatePanel(parent, name, theme);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = Vector2.zero;
            return rectTransform;
        }

        public static Text CreateText(
            Transform parent,
            string name,
            string value,
            int fontSize,
            TextAnchor alignment,
            UIThemeConfig theme)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400f, 40f);

            var text = gameObject.AddComponent<Text>();
            text.text = value;
            text.font = theme != null ? theme.ResolveFont() : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = theme != null ? theme.PrimaryTextColor : Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        public static Button CreateButton(Transform parent, string name, string label, UIThemeConfig theme, out Text labelText)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = theme != null ? theme.DefaultButtonSize : new Vector2(280f, 56f);

            var image = gameObject.AddComponent<Image>();
            image.color = theme != null ? theme.ButtonColor : new Color(0.22f, 0.26f, 0.3f, 1f);
            image.sprite = theme != null ? theme.ButtonSprite : null;
            image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;

            var button = gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            labelText = CreateText(gameObject.transform, "Label", label, theme != null ? theme.ButtonFontSize : 18, TextAnchor.MiddleCenter, theme);
            Stretch(labelText.rectTransform, Vector2.zero);
            labelText.color = theme != null ? theme.ButtonTextColor : Color.white;
            return button;
        }

        public static InputField CreateInputField(Transform parent, string name, string placeholder, UIThemeConfig theme)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(320f, 48f);

            var image = gameObject.AddComponent<Image>();
            image.color = new Color(0.04f, 0.05f, 0.06f, 0.95f);

            var inputField = gameObject.AddComponent<InputField>();
            inputField.targetGraphic = image;

            var text = CreateText(gameObject.transform, "Text", string.Empty, theme != null ? theme.BodyFontSize : 18, TextAnchor.MiddleLeft, theme);
            Stretch(text.rectTransform, new Vector2(14f, 4f));
            inputField.textComponent = text;

            var placeholderText = CreateText(gameObject.transform, "Placeholder", placeholder, theme != null ? theme.BodyFontSize : 18, TextAnchor.MiddleLeft, theme);
            Stretch(placeholderText.rectTransform, new Vector2(14f, 4f));
            placeholderText.color = theme != null ? theme.SecondaryTextColor : new Color(1f, 1f, 1f, 0.55f);
            inputField.placeholder = placeholderText;
            return inputField;
        }

        public static Slider CreateProgressBar(Transform parent, string name, Color fillColor)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            var rectTransform = root.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(360f, 22f);

            var background = new GameObject("Background");
            background.transform.SetParent(root.transform, false);
            var backgroundRect = background.AddComponent<RectTransform>();
            Stretch(backgroundRect, Vector2.zero);
            var backgroundImage = background.AddComponent<Image>();
            backgroundImage.color = new Color(0.02f, 0.025f, 0.03f, 0.9f);

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(root.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            Stretch(fillAreaRect, new Vector2(2f, 2f));

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            Stretch(fillRect, Vector2.zero);
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = fillColor;

            var slider = root.AddComponent<Slider>();
            slider.transition = Selectable.Transition.None;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;
            return slider;
        }

        public static RoomCardView CreateRoomCard(Transform parent, UIThemeConfig theme)
        {
            var root = CreateFixedPanel(parent, "WOB_RoomCard", theme != null ? theme.RoomCardSize : new Vector2(560f, 88f), theme);
            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = root.GetComponent<Image>();

            var roomName = CreateText(root, "RoomName", "Room", theme != null ? theme.BodyFontSize : 18, TextAnchor.MiddleLeft, theme);
            SetAnchored(roomName.rectTransform, new Vector2(22f, 18f), new Vector2(260f, 28f), new Vector2(0f, 0.5f));

            var roomCode = CreateText(root, "RoomCode", "CODE", theme != null ? theme.BodyFontSize : 18, TextAnchor.MiddleLeft, theme);
            SetAnchored(roomCode.rectTransform, new Vector2(22f, -18f), new Vector2(160f, 28f), new Vector2(0f, 0.5f));

            var players = CreateText(root, "Players", "1/2", theme != null ? theme.BodyFontSize : 18, TextAnchor.MiddleCenter, theme);
            SetAnchored(players.rectTransform, new Vector2(350f, 0f), new Vector2(80f, 36f), new Vector2(0f, 0.5f));

            var region = CreateText(root, "Region", "Auto/Asia", theme != null ? theme.BodyFontSize : 18, TextAnchor.MiddleRight, theme);
            SetAnchored(region.rectTransform, new Vector2(-22f, 0f), new Vector2(130f, 36f), new Vector2(1f, 0.5f));

            var card = root.gameObject.AddComponent<RoomCardView>();
            card.Configure(button, roomName, roomCode, players, region);
            return card;
        }

        public static void SetAnchored(RectTransform rectTransform, Vector2 position, Vector2 size, Vector2 anchor)
        {
            rectTransform.anchorMin = anchor;
            rectTransform.anchorMax = anchor;
            rectTransform.pivot = anchor;
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = size;
        }

        public static void Stretch(RectTransform rectTransform, Vector2 padding)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = padding;
            rectTransform.offsetMax = -padding;
        }

        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("UI EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }
}
