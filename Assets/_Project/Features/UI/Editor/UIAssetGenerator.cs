using System.IO;
using RicochetTanks.Features.UI.Configs;
using RicochetTanks.Features.UI.Infrastructure;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Editor
{
    public static class UIAssetGenerator
    {
        private const string ConfigsFolder = "Assets/_Project/Features/UI/Configs";
        private const string PrefabsFolder = "Assets/_Project/Features/UI/Prefabs";
        private const string ScenesFolder = "Assets/_Project/Features/UI/Scenes";
        private const string ThemePath = ConfigsFolder + "/UIThemeConfig.asset";
        private const string SandboxScenePath = ScenesFolder + "/UISandbox.unity";

        [MenuItem("Tools/World of Balance/UI/Generate UI Sandbox Assets")]
        public static void GenerateAll()
        {
            EnsureFolder(ConfigsFolder);
            EnsureFolder(PrefabsFolder);
            EnsureFolder(ScenesFolder);

            var theme = LoadOrCreateTheme();
            CreateBasePrefabs(theme);
            CreateSandboxScene(theme);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Generated World of Balance UI sandbox assets.");
        }

        private static UIThemeConfig LoadOrCreateTheme()
        {
            var theme = AssetDatabase.LoadAssetAtPath<UIThemeConfig>(ThemePath);
            if (theme != null)
            {
                return theme;
            }

            theme = ScriptableObject.CreateInstance<UIThemeConfig>();
            AssetDatabase.CreateAsset(theme, ThemePath);
            return theme;
        }

        private static void CreateBasePrefabs(UIThemeConfig theme)
        {
            var buildRoot = new GameObject("UI Prefab Build Root");

            var button = UIRuntimeFactory.CreateButton(buildRoot.transform, "WOB_Button", "Button", theme, out _);
            SavePrefab(button.gameObject, PrefabsFolder + "/WOB_Button.prefab");

            var panel = UIRuntimeFactory.CreateFixedPanel(buildRoot.transform, "WOB_Panel", new Vector2(560f, 320f), theme);
            SavePrefab(panel.gameObject, PrefabsFolder + "/WOB_Panel.prefab");

            var popup = UIRuntimeFactory.CreateFixedPanel(buildRoot.transform, "WOB_Popup", new Vector2(700f, 420f), theme);
            var popupImage = popup.GetComponent<Image>();
            if (popupImage != null)
            {
                popupImage.color = theme.PopupColor;
                popupImage.sprite = theme.PopupSprite;
                popupImage.type = popupImage.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            }

            SavePrefab(popup.gameObject, PrefabsFolder + "/WOB_Popup.prefab");

            var healthBar = UIRuntimeFactory.CreateProgressBar(buildRoot.transform, "WOB_HealthBar", theme.HealthFillColor);
            SavePrefab(healthBar.gameObject, PrefabsFolder + "/WOB_HealthBar.prefab");

            var reloadBar = UIRuntimeFactory.CreateProgressBar(buildRoot.transform, "WOB_ReloadBar", theme.ReloadFillColor);
            SavePrefab(reloadBar.gameObject, PrefabsFolder + "/WOB_ReloadBar.prefab");

            var roomCard = UIRuntimeFactory.CreateRoomCard(buildRoot.transform, theme);
            SavePrefab(roomCard.gameObject, PrefabsFolder + "/WOB_RoomCard.prefab");

            Object.DestroyImmediate(buildRoot);
        }

        private static void CreateSandboxScene(UIThemeConfig theme)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var roots = UIRuntimeFactory.CreateAndroidCanvas("UISandboxCanvas", theme);
            var views = UISandboxBuilder.Create(roots, theme);

            var bootstrapperObject = new GameObject("UISandboxBootstrapper");
            var bootstrapper = bootstrapperObject.AddComponent<UISandboxBootstrapper>();
            bootstrapper.Configure(
                theme,
                views.MainMenuView,
                views.LobbyView,
                views.RoomView,
                views.GameplayHudView,
                views.ResultView);
            views.MainMenuView.SetVisible(true);
            views.LobbyView.SetVisible(false);
            views.RoomView.SetVisible(false);
            views.GameplayHudView.SetVisible(false);
            views.ResultView.SetVisible(false);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, SandboxScenePath);
        }

        private static void SavePrefab(GameObject gameObject, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(gameObject, path);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var parent = Path.GetDirectoryName(path);
            var folderName = Path.GetFileName(path);

            if (string.IsNullOrEmpty(parent))
            {
                return;
            }

            parent = parent.Replace("\\", "/");
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
