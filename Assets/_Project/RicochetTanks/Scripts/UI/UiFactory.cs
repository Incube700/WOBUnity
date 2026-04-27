using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RicochetTanks.UI
{
    public static class UiFactory
    {
        public static Canvas CreateCanvas(string name)
        {
            var canvasObject = new GameObject(name);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static void EnsureEventSystem(string name)
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject(name);
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        public static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition, UnityAction onClick)
        {
            return CreateButton(parent, text, anchoredPosition, new Vector2(220f, 45f), onClick);
        }

        public static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition, Vector2 size, UnityAction onClick)
        {
            var buttonObject = new GameObject(text + "Button");
            buttonObject.transform.SetParent(parent, false);

            var rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = anchoredPosition;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var button = buttonObject.AddComponent<Button>();
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            var textObject = new GameObject("Label");
            textObject.transform.SetParent(buttonObject.transform, false);
            var textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var label = textObject.AddComponent<Text>();
            label.text = text;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 16;

            return button;
        }

        public static Text CreateText(Transform parent, string name, Vector2 anchoredPosition)
        {
            return CreateText(parent, name, anchoredPosition, new Vector2(260f, 30f), TextAnchor.MiddleLeft);
        }

        public static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rectTransform = textObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = anchoredPosition;
            var label = textObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.alignment = alignment;
            label.color = Color.white;
            return label;
        }
    }
}
