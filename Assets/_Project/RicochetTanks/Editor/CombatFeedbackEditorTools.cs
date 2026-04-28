using RicochetTanks.UI;
using RicochetTanks.UI.CombatFeedback;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Editor
{
    public static class CombatFeedbackEditorTools
    {
        private const string ProjectRoot = "Assets/_Project/RicochetTanks";
        private const string PrefabsRoot = ProjectRoot + "/Prefabs";
        private const string PrefabsUiRoot = PrefabsRoot + "/UI";
        private const string WorldHealthBarPrefabPath = PrefabsUiRoot + "/WorldHealthBarPrefab.prefab";
        private const string FloatingHitTextPrefabPath = PrefabsUiRoot + "/FloatingHitTextPrefab.prefab";

        [MenuItem("Tools/Ricochet Tanks/Create Combat Feedback Prefabs")]
        public static void CreateCombatFeedbackPrefabs()
        {
            EnsureFolders();
            EnsureWorldHealthBarPrefab();
            EnsureFloatingHitTextPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Created Ricochet Tanks combat feedback prefabs.");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "RicochetTanks");
            EnsureFolder(ProjectRoot, "Prefabs");
            EnsureFolder(PrefabsRoot, "UI");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static GameObject EnsureWorldHealthBarPrefab()
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(WorldHealthBarPrefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var canvas = CreateWorldCanvasRoot("WorldHealthBarPrefab", new Vector2(120f, 28f), 10);
            var background = CreateImage(canvas.transform, "Background", new Color(0.03f, 0.035f, 0.04f, 0.85f));
            Stretch(background.rectTransform);

            var fill = CreateImage(canvas.transform, "Fill", new Color(0.1f, 0.9f, 0.32f, 0.9f));
            Stretch(fill.rectTransform);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;

            var hpText = UiFactory.CreateText(canvas.transform, "HpText", Vector2.zero, new Vector2(120f, 28f), TextAnchor.MiddleCenter);
            hpText.fontSize = 12;
            hpText.fontStyle = FontStyle.Bold;
            hpText.color = Color.white;
            Stretch(hpText.rectTransform);

            var view = canvas.gameObject.AddComponent<TankHealthBarView>();
            view.Configure(fill, hpText, canvas);

            var prefab = PrefabUtility.SaveAsPrefabAsset(canvas.gameObject, WorldHealthBarPrefabPath);
            Object.DestroyImmediate(canvas.gameObject);
            return prefab;
        }

        private static GameObject EnsureFloatingHitTextPrefab()
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(FloatingHitTextPrefabPath);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var canvas = CreateWorldCanvasRoot("FloatingHitTextPrefab", new Vector2(160f, 42f), 20);
            var canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
            var hitText = UiFactory.CreateText(canvas.transform, "HitText", Vector2.zero, new Vector2(160f, 42f), TextAnchor.MiddleCenter);
            hitText.fontSize = 22;
            hitText.fontStyle = FontStyle.Bold;
            hitText.color = Color.white;
            Stretch(hitText.rectTransform);

            var outline = hitText.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
            outline.effectDistance = new Vector2(1.4f, -1.4f);

            var view = canvas.gameObject.AddComponent<FloatingHitTextView>();
            view.Configure(hitText, canvasGroup, canvas);

            var prefab = PrefabUtility.SaveAsPrefabAsset(canvas.gameObject, FloatingHitTextPrefabPath);
            Object.DestroyImmediate(canvas.gameObject);
            return prefab;
        }

        private static Canvas CreateWorldCanvasRoot(string objectName, Vector2 size, int sortingOrder)
        {
            var canvasObject = new GameObject(objectName);
            var rectTransform = canvasObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            canvasObject.transform.localScale = Vector3.one * 0.01f;

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = sortingOrder;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static Image CreateImage(Transform parent, string objectName, Color color)
        {
            var imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);
            imageObject.AddComponent<RectTransform>();
            var image = imageObject.AddComponent<Image>();
            image.color = color;
            return image;
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
