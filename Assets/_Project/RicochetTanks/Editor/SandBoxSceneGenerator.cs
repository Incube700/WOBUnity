using RicochetTanks.UI;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RicochetTanks.Editor
{
    public static class SandBoxSceneGenerator
    {
        private const string ScenePath = "Assets/_Project/RicochetTanks/Scenes/Sand Box.unity";

        [MenuItem("Tools/Ricochet Tanks/Generate Sand Box Scene")]
        public static void Generate()
        {
            EnsureFolder("Assets/_Project", "RicochetTanks");
            EnsureFolder("Assets/_Project/RicochetTanks", "Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var bootstrapperObject = new GameObject(nameof(SandboxBootstrapper));
            var bootstrapper = bootstrapperObject.AddComponent<SandboxBootstrapper>();
            bootstrapper.RebuildScene();

            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                Debug.LogError($"Failed to save Sand Box scene to {ScenePath}.");
                return;
            }

            AssetDatabase.ImportAsset(ScenePath);
            EnsureSceneInBuildSettings(ScenePath);
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            Debug.Log($"Generated Sand Box scene at {ScenePath}.");
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            var path = $"{parent}/{folderName}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static void EnsureSceneInBuildSettings(string scenePath)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            for (var index = 0; index < scenes.Count; index++)
            {
                if (scenes[index].path != scenePath)
                {
                    continue;
                }

                scenes[index] = new EditorBuildSettingsScene(scenePath, true);
                EditorBuildSettings.scenes = scenes.ToArray();
                return;
            }

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
