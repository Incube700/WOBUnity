using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.DebugTools;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Input.Desktop;
using RicochetTanks.UI;
using RicochetTanks.UI.Sandbox;
using UnityEngine;

namespace RicochetTanks.Infrastructure
{
    public sealed class SandboxSceneContext
    {
        public SandboxSceneContext(
            TankFacade player,
            TankFacade enemy,
            SandboxHudView hudView,
            Camera camera,
            DesktopInputReader inputReader,
            ProjectileFactory projectileFactory,
            SandboxGameplayEvents gameplayEvents)
        {
            Player = player;
            Enemy = enemy;
            HudView = hudView;
            Camera = camera;
            InputReader = inputReader;
            ProjectileFactory = projectileFactory;
            GameplayEvents = gameplayEvents;
        }

        public TankFacade Player { get; }
        public TankFacade Enemy { get; }
        public SandboxHudView HudView { get; }
        public Camera Camera { get; }
        public DesktopInputReader InputReader { get; }
        public ProjectileFactory ProjectileFactory { get; }
        public SandboxGameplayEvents GameplayEvents { get; }
    }

    // Dev/procedural fallback only. The playable prototype should use saved scenes and prefabs.
    public static class SandboxSceneBuilder
    {
        public static SandboxSceneContext Build(Transform root)
        {
            return Build(root, null, null, null, null);
        }

        public static SandboxSceneContext Build(
            Transform root,
            ArenaConfig arenaConfig,
            TankConfig tankConfig,
            ProjectileConfig projectileConfig,
            DebugVisualizationConfig debugVisualizationConfig)
        {
            ClearChildren(root);

            var resolvedArenaConfig = ResolveConfig(arenaConfig);
            var resolvedTankConfig = ResolveConfig(tankConfig);
            var resolvedProjectileConfig = ResolveConfig(projectileConfig);
            var resolvedDebugVisualizationConfig = ResolveConfig(debugVisualizationConfig);

            var camera = CreateCamera(root);
            CreateLight(root);
            CreateArena(root, resolvedArenaConfig);
            var inputReader = CreateInput(root);
            var gameplayEvents = new SandboxGameplayEvents();
            var projectileFactory = CreateProjectileFactory(root, resolvedProjectileConfig, gameplayEvents);

            var player = CreateTank("Player Tank", root, resolvedArenaConfig.PlayerStartPosition, new Color(0f, 0.78f, 0.32f), camera, true, inputReader, projectileFactory, resolvedTankConfig, resolvedProjectileConfig);
            var enemy = CreateTank("Enemy Dummy Tank", root, resolvedArenaConfig.EnemyStartPosition, new Color(0.95f, 0.12f, 0.08f), camera, false, inputReader, projectileFactory, resolvedTankConfig, resolvedProjectileConfig);
            var hudView = CreateHud(root);
            CreateDebugVisualizer(root, resolvedArenaConfig, resolvedDebugVisualizationConfig, player, enemy, gameplayEvents, camera);
            UiFactory.EnsureEventSystem("Sandbox EventSystem");

            return new SandboxSceneContext(player, enemy, hudView, camera, inputReader, projectileFactory, gameplayEvents);
        }

        private static T ResolveConfig<T>(T config) where T : ScriptableObject
        {
            return config != null ? config : ScriptableObject.CreateInstance<T>();
        }

        private static void ClearChildren(Transform root)
        {
            for (var index = root.childCount - 1; index >= 0; index--)
            {
                DestroyObject(root.GetChild(index).gameObject);
            }
        }

        private static Camera CreateCamera(Transform root)
        {
            var cameraObject = new GameObject("Sandbox Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(root, false);
            cameraObject.transform.position = new Vector3(0f, 14f, 0f);
            cameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.04f, 0.05f, 0.055f);
            camera.orthographic = true;
            camera.orthographicSize = 6.25f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 50f;
            return camera;
        }

        private static void CreateLight(Transform root)
        {
            var lightObject = new GameObject("Arena Directional Light");
            lightObject.transform.SetParent(root, false);
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
        }

        private static void CreateArena(Transform root, ArenaConfig config)
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "10x10 Arena Floor";
            floor.transform.SetParent(root, false);
            floor.transform.localPosition = new Vector3(0f, -0.08f, 0f);
            floor.transform.localScale = new Vector3(config.HalfSize * 2f, 0.12f, config.HalfSize * 2f);
            floor.isStatic = true;
            Tint(floor, new Color(0.17f, 0.19f, 0.2f));
            RemoveCollider(floor);
            CreateGrid(root, config);

            var wallLength = config.HalfSize * 2f + config.WallThickness;
            CreateCube(root, "North Ricochet Wall", new Vector3(0f, 0.45f, config.HalfSize + config.WallThickness * 0.5f), new Vector3(wallLength, 0.9f, config.WallThickness), new Color(0.05f, 0.07f, 0.08f));
            CreateCube(root, "South Ricochet Wall", new Vector3(0f, 0.45f, -config.HalfSize - config.WallThickness * 0.5f), new Vector3(wallLength, 0.9f, config.WallThickness), new Color(0.05f, 0.07f, 0.08f));
            CreateCube(root, "East Ricochet Wall", new Vector3(config.HalfSize + config.WallThickness * 0.5f, 0.45f, 0f), new Vector3(config.WallThickness, 0.9f, wallLength), new Color(0.05f, 0.07f, 0.08f));
            CreateCube(root, "West Ricochet Wall", new Vector3(-config.HalfSize - config.WallThickness * 0.5f, 0.45f, 0f), new Vector3(config.WallThickness, 0.9f, wallLength), new Color(0.05f, 0.07f, 0.08f));
            CreateCube(root, "Center Square Cover / Ricochet Block", new Vector3(0f, 0.5f, 0f), config.CenterObstacleSize, new Color(0.55f, 0.58f, 0.62f));
        }

        private static void CreateGrid(Transform root, ArenaConfig config)
        {
            var lineColor = new Color(0.24f, 0.27f, 0.29f);
            var arenaSize = config.HalfSize * 2f;

            for (var index = 0; index <= arenaSize; index++)
            {
                var offset = -config.HalfSize + index;
                CreateVisualCube(root, "Grid Line X", new Vector3(offset, 0.01f, 0f), new Vector3(0.025f, 0.025f, arenaSize), lineColor);
                CreateVisualCube(root, "Grid Line Z", new Vector3(0f, 0.012f, offset), new Vector3(arenaSize, 0.025f, 0.025f), lineColor);
            }
        }

        private static DesktopInputReader CreateInput(Transform root)
        {
            var inputObject = new GameObject("Desktop Input Reader");
            inputObject.transform.SetParent(root, false);
            return inputObject.AddComponent<DesktopInputReader>();
        }

        private static ProjectileFactory CreateProjectileFactory(Transform root, ProjectileConfig config, SandboxGameplayEvents gameplayEvents)
        {
            var factoryObject = new GameObject("Projectile Factory");
            factoryObject.transform.SetParent(root, false);
            var factory = factoryObject.AddComponent<ProjectileFactory>();
            factory.Configure(config, gameplayEvents);
            return factory;
        }

        private static void CreateDebugVisualizer(
            Transform root,
            ArenaConfig arenaConfig,
            DebugVisualizationConfig debugVisualizationConfig,
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            Camera camera)
        {
            if (debugVisualizationConfig == null || !debugVisualizationConfig.IsEnabled)
            {
                return;
            }

            var debugObject = new GameObject("Sandbox Debug Visualizer");
            debugObject.transform.SetParent(root, false);
            var visualizer = debugObject.AddComponent<SandboxDebugVisualizer>();
            visualizer.Configure(arenaConfig, debugVisualizationConfig, player, enemy, gameplayEvents, camera);
        }

        private static TankFacade CreateTank(
            string name,
            Transform parent,
            Vector3 position,
            Color color,
            Camera camera,
            bool isPlayerControlled,
            DesktopInputReader inputReader,
            ProjectileFactory projectileFactory,
            TankConfig tankConfig,
            ProjectileConfig projectileConfig)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;
            root.transform.rotation = Quaternion.Euler(0f, isPlayerControlled ? 45f : -135f, 0f);

            var rigidbody = root.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = !isPlayerControlled;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            var hitbox = root.AddComponent<BoxCollider>();
            hitbox.center = new Vector3(0f, 0.5f, 0f);
            hitbox.size = new Vector3(1.2f, 0.9f, 1.35f);

            var facade = root.AddComponent<TankFacade>();
            var movement = root.AddComponent<TankMovement>();
            var aiming = root.AddComponent<TurretAiming>();
            var shooter = root.AddComponent<TankShooter>();
            var health = root.AddComponent<TankHealth>();
            var armor = root.AddComponent<TankArmor>();
            var controller = root.AddComponent<PlayerTankController>();

            var body = CreateCube(root.transform, "Body / Hull", new Vector3(0f, 0.28f, 0f), new Vector3(0.95f, 0.4f, 1.25f), color);
            RemoveCollider(body);
            var turret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            turret.name = "Turret";
            turret.transform.SetParent(root.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.62f, 0f);
            turret.transform.localScale = new Vector3(0.36f, 0.12f, 0.36f);
            Tint(turret, Color.Lerp(color, Color.white, 0.18f));
            RemoveCollider(turret);

            var barrel = CreateCube(turret.transform, "Barrel / Forward Direction", new Vector3(0f, 0f, 0.62f), new Vector3(0.18f, 0.16f, 1f), Color.Lerp(color, Color.white, 0.35f));
            RemoveCollider(barrel);
            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turret.transform, false);
            muzzle.localPosition = new Vector3(0f, 0f, 1.25f);

            movement.Configure(rigidbody, tankConfig.MoveSpeed, tankConfig.TurnSpeed);
            aiming.Configure(turret.transform, camera, tankConfig.TurretRotationSpeed);
            shooter.Configure(muzzle, facade, projectileFactory, projectileConfig);
            health.Configure(tankConfig.MaxHp);
            armor.Configure(tankConfig);
            controller.Configure(facade, inputReader, camera);
            facade.Configure(movement, aiming, shooter, health, controller);
            facade.SetPlayerControlled(isPlayerControlled);

            body.isStatic = false;
            barrel.isStatic = false;
            turret.isStatic = false;
            return facade;
        }

        private static SandboxHudView CreateHud(Transform root)
        {
            var canvas = UiFactory.CreateCanvas("Sandbox HUD Canvas");
            canvas.transform.SetParent(root, false);

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
            return hudView;
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Color color)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPosition;
            cube.transform.localScale = localScale;
            cube.isStatic = true;
            Tint(cube, color);
            return cube;
        }

        private static GameObject CreateVisualCube(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Color color)
        {
            var cube = CreateCube(parent, name, localPosition, localScale, color);
            RemoveCollider(cube);
            return cube;
        }

        private static void Tint(GameObject target, Color color)
        {
            if (target.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.material = CreateMaterial(color);
            }
        }

        private static Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

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

            return material;
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

        private static void RemoveCollider(GameObject target)
        {
            if (!target.TryGetComponent<Collider>(out var collider))
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(collider);
                return;
            }
#endif
            Object.Destroy(collider);
        }

        private static void DestroyObject(GameObject target)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(target);
                return;
            }
#endif
            Object.Destroy(target);
        }
    }
}
