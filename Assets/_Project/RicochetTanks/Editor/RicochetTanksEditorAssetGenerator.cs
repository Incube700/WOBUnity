using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure.Bootstrap;
using RicochetTanks.UI;
using RicochetTanks.UI.Sandbox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Editor
{
    public static class RicochetTanksEditorAssetGenerator
    {
        private const string ProjectRoot = "Assets/_Project/RicochetTanks";
        private const string ScenesRoot = ProjectRoot + "/Scenes";
        private const string PrefabsRoot = ProjectRoot + "/Prefabs";
        private const string ConfigsRoot = ProjectRoot + "/Configs";
        private const string DemoScenePath = ScenesRoot + "/RicochetTanks_Demo.unity";

        [MenuItem("Tools/Ricochet Tanks/Generate Editor-Friendly Demo")]
        public static void GenerateEditorFriendlyDemo()
        {
            EnsureFolders();

            var playerTankConfig = EnsureTankConfig(ConfigsRoot + "/PlayerTankConfig.asset", "PlayerTankConfig", 100, 4.5f, 150f, 220f, 100, 70, 40, 70f);
            var enemyTankConfig = EnsureTankConfig(ConfigsRoot + "/EnemyTankConfig.asset", "EnemyTankConfig", 100, 0f, 0f, 0f, 100, 70, 40, 70f);
            var matchConfig = EnsureMatchConfig(ConfigsRoot + "/MatchConfig.asset");
            var cameraConfig = EnsureCameraConfig(ConfigsRoot + "/CameraConfig.asset");

            var wallPrefab = EnsureCubePrefab(PrefabsRoot + "/WallSegmentPrefab.prefab", "WallSegmentPrefab", "RicochetReflectable", new Color(0.05f, 0.07f, 0.08f));
            var blockPrefab = EnsureCubePrefab(PrefabsRoot + "/ArenaBlockPrefab.prefab", "ArenaBlockPrefab", "RicochetReflectable", new Color(0.55f, 0.58f, 0.62f));
            var projectilePrefab = EnsureProjectilePrefab(PrefabsRoot + "/ProjectilePrefab.prefab");
            var playerTankPrefab = EnsureTankPrefab(PrefabsRoot + "/PlayerTankPrefab.prefab", "PlayerTankPrefab", new Color(0f, 0.78f, 0.32f));
            var enemyTankPrefab = EnsureTankPrefab(PrefabsRoot + "/EnemyDummyTankPrefab.prefab", "EnemyDummyTankPrefab", new Color(0.95f, 0.12f, 0.08f));
            var gameplayCanvasPrefab = EnsureGameplayCanvasPrefab(PrefabsRoot + "/GameplayCanvasPrefab.prefab");
            var projectileConfig = EnsureProjectileConfig(ConfigsRoot + "/ProjectileConfig.asset", projectilePrefab);

            CreateDemoScene(
                playerTankConfig,
                enemyTankConfig,
                projectileConfig,
                matchConfig,
                cameraConfig,
                wallPrefab,
                blockPrefab,
                playerTankPrefab,
                enemyTankPrefab,
                gameplayCanvasPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Generated editor-friendly Ricochet Tanks demo scene at {DemoScenePath}.");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "_Project");
            EnsureFolder("Assets/_Project", "RicochetTanks");
            EnsureFolder(ProjectRoot, "Scenes");
            EnsureFolder(ProjectRoot, "Prefabs");
            EnsureFolder(ProjectRoot, "Configs");
        }

        private static void EnsureFolder(string parent, string child)
        {
            var path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static TankConfig EnsureTankConfig(
            string path,
            string assetName,
            int maxHp,
            float moveSpeed,
            float turnSpeed,
            float turretRotationSpeed,
            int frontArmor,
            int sideArmor,
            int rearArmor,
            float autoRicochetAngle)
        {
            var config = LoadOrCreateAsset<TankConfig>(path, assetName);
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("_maxHp").intValue = maxHp;
            serializedConfig.FindProperty("_moveSpeed").floatValue = moveSpeed;
            serializedConfig.FindProperty("_turnSpeed").floatValue = turnSpeed;
            serializedConfig.FindProperty("_turretRotationSpeed").floatValue = turretRotationSpeed;
            serializedConfig.FindProperty("_frontArmor").intValue = frontArmor;
            serializedConfig.FindProperty("_sideArmor").intValue = sideArmor;
            serializedConfig.FindProperty("_rearArmor").intValue = rearArmor;
            serializedConfig.FindProperty("_autoRicochetAngle").floatValue = autoRicochetAngle;
            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            return config;
        }

        private static ProjectileConfig EnsureProjectileConfig(string path, GameObject projectilePrefab)
        {
            var config = LoadOrCreateAsset<ProjectileConfig>(path, "ProjectileConfig");
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("_projectilePrefab").objectReferenceValue = projectilePrefab;
            serializedConfig.FindProperty("_speed").floatValue = 22f;
            serializedConfig.FindProperty("_bounceSpeedMultiplier").floatValue = 0.85f;
            serializedConfig.FindProperty("_cooldown").floatValue = 0.8f;
            serializedConfig.FindProperty("_safeTime").floatValue = 0.15f;
            serializedConfig.FindProperty("_lifetime").floatValue = 8f;
            serializedConfig.FindProperty("_minSpeed").floatValue = 5f;
            serializedConfig.FindProperty("_radius").floatValue = 0.18f;
            serializedConfig.FindProperty("_spawnOffset").floatValue = 0.35f;
            serializedConfig.FindProperty("_positionCorrectionSkin").floatValue = 0.01f;
            serializedConfig.FindProperty("_trailTime").floatValue = 0.25f;
            serializedConfig.FindProperty("_damage").floatValue = 35f;
            serializedConfig.FindProperty("_damageMultiplierPerBounce").floatValue = 0.75f;
            serializedConfig.FindProperty("_maxRicochets").intValue = 3;
            serializedConfig.FindProperty("_penetration").intValue = 100;
            serializedConfig.FindProperty("_reflectableMask").intValue = LayerMask.GetMask("RicochetReflectable", "Obstacle");
            serializedConfig.FindProperty("_hittableMask").intValue = LayerMask.GetMask("Tank");
            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            return config;
        }

        private static MatchConfig EnsureMatchConfig(string path)
        {
            var config = LoadOrCreateAsset<MatchConfig>(path, "MatchConfig");
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("_playingLabel").stringValue = "Round: Playing";
            serializedConfig.FindProperty("_playerWinsLabel").stringValue = "Player Wins";
            serializedConfig.FindProperty("_enemyWinsLabel").stringValue = "Enemy Wins";
            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            return config;
        }

        private static CameraConfig EnsureCameraConfig(string path)
        {
            var config = LoadOrCreateAsset<CameraConfig>(path, "CameraConfig");
            var serializedConfig = new SerializedObject(config);
            serializedConfig.FindProperty("_orthographic").boolValue = true;
            serializedConfig.FindProperty("_localPosition").vector3Value = new Vector3(0f, 14f, 0f);
            serializedConfig.FindProperty("_localEulerAngles").vector3Value = new Vector3(90f, 0f, 0f);
            serializedConfig.FindProperty("_orthographicSize").floatValue = 6.25f;
            serializedConfig.FindProperty("_nearClipPlane").floatValue = 0.1f;
            serializedConfig.FindProperty("_farClipPlane").floatValue = 50f;
            serializedConfig.FindProperty("_backgroundColor").colorValue = new Color(0.04f, 0.05f, 0.055f);
            serializedConfig.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(config);
            return config;
        }

        private static T LoadOrCreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<T>();
            asset.name = assetName;
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static GameObject EnsureCubePrefab(string path, string objectName, string layerName, Color color)
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = objectName;
            SetLayerRecursively(cube, LayerMask.NameToLayer(layerName));
            Tint(cube, color);
            var prefab = PrefabUtility.SaveAsPrefabAsset(cube, path);
            Object.DestroyImmediate(cube);
            return prefab;
        }

        private static GameObject EnsureProjectilePrefab(string path)
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "ProjectilePrefab";
            projectile.transform.localScale = Vector3.one * 0.36f;
            SetLayerRecursively(projectile, LayerMask.NameToLayer("Projectile"));
            Tint(projectile, new Color(1f, 0.86f, 0.05f));
            projectile.AddComponent<Projectile>();
            var prefab = PrefabUtility.SaveAsPrefabAsset(projectile, path);
            Object.DestroyImmediate(projectile);
            return prefab;
        }

        private static GameObject EnsureTankPrefab(string path, string objectName, Color color)
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var root = new GameObject(objectName);
            SetLayerRecursively(root, LayerMask.NameToLayer("Tank"));
            root.AddComponent<Rigidbody>();
            root.AddComponent<BoxCollider>();
            root.AddComponent<TankFacade>();
            root.AddComponent<TankMovement>();
            root.AddComponent<TurretAiming>();
            root.AddComponent<TankShooter>();
            root.AddComponent<TankHealth>();
            root.AddComponent<TankArmor>();
            root.AddComponent<PlayerTankController>();

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body / Hull";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.28f, 0f);
            body.transform.localScale = new Vector3(0.95f, 0.4f, 1.25f);
            RemoveCollider(body);
            Tint(body, color);

            var turret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            turret.name = "Turret";
            turret.transform.SetParent(root.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.62f, 0f);
            turret.transform.localScale = new Vector3(0.36f, 0.12f, 0.36f);
            RemoveCollider(turret);
            Tint(turret, Color.Lerp(color, Color.white, 0.18f));

            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrel.name = "Barrel / Forward Direction";
            barrel.transform.SetParent(turret.transform, false);
            barrel.transform.localPosition = new Vector3(0f, 0f, 0.62f);
            barrel.transform.localScale = new Vector3(0.18f, 0.16f, 1f);
            RemoveCollider(barrel);
            Tint(barrel, Color.Lerp(color, Color.white, 0.35f));

            var muzzle = new GameObject("Muzzle");
            muzzle.transform.SetParent(turret.transform, false);
            muzzle.transform.localPosition = new Vector3(0f, 0f, 1.25f);

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab;
        }

        private static GameObject EnsureGameplayCanvasPrefab(string path)
        {
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existingPrefab != null)
            {
                return existingPrefab;
            }

            var canvas = UiFactory.CreateCanvas("GameplayCanvasPrefab");
            var playerHpText = UiFactory.CreateText(canvas.transform, "PlayerHpText", new Vector2(20f, -20f), new Vector2(260f, 28f), TextAnchor.MiddleLeft);
            AnchorTopLeft(playerHpText.rectTransform);
            var enemyHpText = UiFactory.CreateText(canvas.transform, "EnemyHpText", new Vector2(20f, -52f), new Vector2(260f, 28f), TextAnchor.MiddleLeft);
            AnchorTopLeft(enemyHpText.rectTransform);
            var lastHitText = UiFactory.CreateText(canvas.transform, "LastHitText", new Vector2(20f, -84f), new Vector2(520f, 28f), TextAnchor.MiddleLeft);
            AnchorTopLeft(lastHitText.rectTransform);
            var roundResultText = UiFactory.CreateText(canvas.transform, "RoundResultText", new Vector2(0f, -18f), new Vector2(360f, 30f), TextAnchor.MiddleCenter);
            AnchorTopCenter(roundResultText.rectTransform);
            var controlsHintText = UiFactory.CreateText(canvas.transform, "ControlsHintText", new Vector2(0f, 22f), new Vector2(900f, 30f), TextAnchor.MiddleCenter);
            AnchorBottomCenter(controlsHintText.rectTransform);
            var restartButton = UiFactory.CreateButton(canvas.transform, "Restart", new Vector2(-20f, -20f), new Vector2(160f, 42f), null);
            AnchorTopRight((RectTransform)restartButton.transform);

            var hudView = canvas.gameObject.AddComponent<SandboxHudView>();
            hudView.Configure(playerHpText, enemyHpText, lastHitText, roundResultText, controlsHintText, restartButton);

            var prefab = PrefabUtility.SaveAsPrefabAsset(canvas.gameObject, path);
            Object.DestroyImmediate(canvas.gameObject);
            return prefab;
        }

        private static void CreateDemoScene(
            TankConfig playerTankConfig,
            TankConfig enemyTankConfig,
            ProjectileConfig projectileConfig,
            MatchConfig matchConfig,
            CameraConfig cameraConfig,
            GameObject wallPrefab,
            GameObject blockPrefab,
            GameObject playerTankPrefab,
            GameObject enemyTankPrefab,
            GameObject gameplayCanvasPrefab)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var sceneContext = new GameObject("SceneContext / GameplayEntryPoint");
            var entryPoint = sceneContext.AddComponent<GameplayEntryPoint>();
            var arenaRoot = new GameObject("ArenaRoot").transform;
            var wallsRoot = new GameObject("Walls").transform;
            var obstaclesRoot = new GameObject("Obstacles").transform;
            var spawnPointsRoot = new GameObject("SpawnPoints").transform;
            var cameraRig = new GameObject("CameraRig").transform;

            arenaRoot.SetParent(sceneContext.transform, false);
            wallsRoot.SetParent(arenaRoot, false);
            obstaclesRoot.SetParent(arenaRoot, false);
            spawnPointsRoot.SetParent(sceneContext.transform, false);
            cameraRig.SetParent(sceneContext.transform, false);

            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(arenaRoot, false);
            floor.transform.localPosition = new Vector3(0f, -0.08f, 0f);
            floor.transform.localScale = new Vector3(10f, 0.12f, 10f);
            RemoveCollider(floor);
            Tint(floor, new Color(0.17f, 0.19f, 0.2f));

            CreateWall(wallPrefab, wallsRoot, "NorthWall", new Vector3(0f, 0.45f, 5.175f), new Vector3(10.35f, 0.9f, 0.35f));
            CreateWall(wallPrefab, wallsRoot, "SouthWall", new Vector3(0f, 0.45f, -5.175f), new Vector3(10.35f, 0.9f, 0.35f));
            CreateWall(wallPrefab, wallsRoot, "EastWall", new Vector3(5.175f, 0.45f, 0f), new Vector3(0.35f, 0.9f, 10.35f));
            CreateWall(wallPrefab, wallsRoot, "WestWall", new Vector3(-5.175f, 0.45f, 0f), new Vector3(0.35f, 0.9f, 10.35f));

            var centerBlock = (GameObject)PrefabUtility.InstantiatePrefab(blockPrefab, scene);
            centerBlock.name = "CenterArenaBlock";
            centerBlock.transform.SetParent(obstaclesRoot, false);
            centerBlock.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            centerBlock.transform.localScale = new Vector3(2f, 1f, 2f);

            var playerSpawnPoint = new GameObject("PlayerSpawnPoint").transform;
            playerSpawnPoint.SetParent(spawnPointsRoot, false);
            playerSpawnPoint.localPosition = new Vector3(-3.75f, 0f, -3.75f);
            playerSpawnPoint.localRotation = Quaternion.Euler(0f, 45f, 0f);

            var enemySpawnPoint = new GameObject("EnemySpawnPoint").transform;
            enemySpawnPoint.SetParent(spawnPointsRoot, false);
            enemySpawnPoint.localPosition = new Vector3(3.75f, 0f, 3.75f);
            enemySpawnPoint.localRotation = Quaternion.Euler(0f, -135f, 0f);

            var playerTank = (GameObject)PrefabUtility.InstantiatePrefab(playerTankPrefab, scene);
            playerTank.name = "PlayerTank";
            playerTank.transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);

            var enemyTank = (GameObject)PrefabUtility.InstantiatePrefab(enemyTankPrefab, scene);
            enemyTank.name = "EnemyDummyTank";
            enemyTank.transform.SetPositionAndRotation(enemySpawnPoint.position, enemySpawnPoint.rotation);

            var cameraObject = new GameObject("GameplayCamera");
            cameraObject.transform.SetParent(cameraRig, false);
            var camera = cameraObject.AddComponent<Camera>();

            var lightObject = new GameObject("Directional Light");
            lightObject.transform.SetParent(sceneContext.transform, false);
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;

            var gameplayCanvas = (GameObject)PrefabUtility.InstantiatePrefab(gameplayCanvasPrefab, scene);
            gameplayCanvas.name = "GameplayCanvas";
            gameplayCanvas.transform.SetParent(sceneContext.transform, false);
            var hudView = gameplayCanvas.GetComponent<SandboxHudView>();

            AssignEntryPointReferences(
                entryPoint,
                playerTankConfig,
                enemyTankConfig,
                projectileConfig,
                matchConfig,
                cameraConfig,
                arenaRoot,
                playerSpawnPoint,
                enemySpawnPoint,
                playerTank.GetComponent<TankFacade>(),
                enemyTank.GetComponent<TankFacade>(),
                cameraRig,
                camera,
                gameplayCanvas.GetComponent<Canvas>(),
                hudView);

            EditorSceneManager.SaveScene(scene, DemoScenePath);
            AddSceneToBuildSettings(DemoScenePath);
        }

        private static void CreateWall(GameObject wallPrefab, Transform parent, string objectName, Vector3 localPosition, Vector3 localScale)
        {
            var wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent);
            wall.name = objectName;
            wall.transform.localPosition = localPosition;
            wall.transform.localScale = localScale;
        }

        private static void AssignEntryPointReferences(
            GameplayEntryPoint entryPoint,
            TankConfig playerTankConfig,
            TankConfig enemyTankConfig,
            ProjectileConfig projectileConfig,
            MatchConfig matchConfig,
            CameraConfig cameraConfig,
            Transform arenaRoot,
            Transform playerSpawnPoint,
            Transform enemySpawnPoint,
            TankFacade playerTank,
            TankFacade enemyTank,
            Transform cameraRig,
            Camera camera,
            Canvas gameplayCanvas,
            SandboxHudView hudView)
        {
            var serializedEntryPoint = new SerializedObject(entryPoint);
            serializedEntryPoint.FindProperty("_matchConfig").objectReferenceValue = matchConfig;
            serializedEntryPoint.FindProperty("_cameraConfig").objectReferenceValue = cameraConfig;
            serializedEntryPoint.FindProperty("_playerTankConfig").objectReferenceValue = playerTankConfig;
            serializedEntryPoint.FindProperty("_enemyTankConfig").objectReferenceValue = enemyTankConfig;
            serializedEntryPoint.FindProperty("_projectileConfig").objectReferenceValue = projectileConfig;
            serializedEntryPoint.FindProperty("_arenaRoot").objectReferenceValue = arenaRoot;
            serializedEntryPoint.FindProperty("_playerSpawnPoint").objectReferenceValue = playerSpawnPoint;
            serializedEntryPoint.FindProperty("_enemySpawnPoint").objectReferenceValue = enemySpawnPoint;
            serializedEntryPoint.FindProperty("_playerTank").objectReferenceValue = playerTank;
            serializedEntryPoint.FindProperty("_enemyDummyTank").objectReferenceValue = enemyTank;
            serializedEntryPoint.FindProperty("_cameraRig").objectReferenceValue = cameraRig;
            serializedEntryPoint.FindProperty("_camera").objectReferenceValue = camera;
            serializedEntryPoint.FindProperty("_gameplayCanvas").objectReferenceValue = gameplayCanvas;
            serializedEntryPoint.FindProperty("_hudView").objectReferenceValue = hudView;
            serializedEntryPoint.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var existingScenes = EditorBuildSettings.scenes;
            for (var index = 0; index < existingScenes.Length; index++)
            {
                if (existingScenes[index].path == scenePath)
                {
                    return;
                }
            }

            var updatedScenes = new EditorBuildSettingsScene[existingScenes.Length + 1];
            for (var index = 0; index < existingScenes.Length; index++)
            {
                updatedScenes[index] = existingScenes[index];
            }

            updatedScenes[updatedScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = updatedScenes;
        }

        private static void SetLayerRecursively(GameObject target, int layer)
        {
            if (layer < 0)
            {
                return;
            }

            target.layer = layer;
            for (var index = 0; index < target.transform.childCount; index++)
            {
                SetLayerRecursively(target.transform.GetChild(index).gameObject, layer);
            }
        }

        private static void RemoveCollider(GameObject target)
        {
            if (target.TryGetComponent<Collider>(out var collider))
            {
                Object.DestroyImmediate(collider);
            }
        }

        private static void Tint(GameObject target, Color color)
        {
            if (!target.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = new Material(shader);
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else
            {
                material.color = color;
            }

            renderer.sharedMaterial = material;
        }

        private static void AnchorTopLeft(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
        }

        private static void AnchorTopCenter(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
        }

        private static void AnchorTopRight(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
        }

        private static void AnchorBottomCenter(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f);
        }
    }
}
