using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
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
            ProjectileFactory projectileFactory)
        {
            Player = player;
            Enemy = enemy;
            HudView = hudView;
            Camera = camera;
            InputReader = inputReader;
            ProjectileFactory = projectileFactory;
        }

        public TankFacade Player { get; }
        public TankFacade Enemy { get; }
        public SandboxHudView HudView { get; }
        public Camera Camera { get; }
        public DesktopInputReader InputReader { get; }
        public ProjectileFactory ProjectileFactory { get; }
    }

    public static class SandboxSceneBuilder
    {
        public static SandboxSceneContext Build(Transform root)
        {
            return Build(root, null, null, null);
        }

        public static SandboxSceneContext Build(Transform root, ArenaConfig arenaConfig, TankConfig tankConfig, ProjectileConfig projectileConfig)
        {
            ClearChildren(root);

            var resolvedArenaConfig = ResolveConfig(arenaConfig);
            var resolvedTankConfig = ResolveConfig(tankConfig);
            var resolvedProjectileConfig = ResolveConfig(projectileConfig);

            var camera = CreateCamera(root);
            CreateLight(root);
            CreateArena(root, resolvedArenaConfig);
            var inputReader = CreateInput(root);
            var projectileFactory = CreateProjectileFactory(root, resolvedProjectileConfig);

            var player = CreateTank("Player Tank", root, resolvedArenaConfig.PlayerStartPosition, new Color(0.2f, 0.85f, 0.35f), camera, true, inputReader, projectileFactory, resolvedTankConfig, resolvedProjectileConfig);
            var enemy = CreateTank("Enemy Dummy Tank", root, resolvedArenaConfig.EnemyStartPosition, new Color(0.95f, 0.25f, 0.2f), camera, false, inputReader, projectileFactory, resolvedTankConfig, resolvedProjectileConfig);
            var hudView = CreateHud(root);
            UiFactory.EnsureEventSystem("Sandbox EventSystem");

            return new SandboxSceneContext(player, enemy, hudView, camera, inputReader, projectileFactory);
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
            cameraObject.transform.position = new Vector3(0f, 11f, -9f);
            cameraObject.transform.rotation = Quaternion.LookRotation(Vector3.zero - cameraObject.transform.position, Vector3.up);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.11f, 0.12f, 0.13f);
            camera.orthographic = true;
            camera.orthographicSize = 7.2f;
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
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "10x10 Arena Floor";
            floor.transform.SetParent(root, false);
            floor.transform.localScale = Vector3.one;
            floor.isStatic = true;
            Tint(floor, new Color(0.36f, 0.38f, 0.4f));

            var wallLength = config.HalfSize * 2f + config.WallThickness;
            CreateCube(root, "North Wall", new Vector3(0f, 0.5f, config.HalfSize + config.WallThickness * 0.5f), new Vector3(wallLength, 1f, config.WallThickness), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "South Wall", new Vector3(0f, 0.5f, -config.HalfSize - config.WallThickness * 0.5f), new Vector3(wallLength, 1f, config.WallThickness), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "East Wall", new Vector3(config.HalfSize + config.WallThickness * 0.5f, 0.5f, 0f), new Vector3(config.WallThickness, 1f, wallLength), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "West Wall", new Vector3(-config.HalfSize - config.WallThickness * 0.5f, 0.5f, 0f), new Vector3(config.WallThickness, 1f, wallLength), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "Center Square Obstacle", new Vector3(0f, 0.5f, 0f), config.CenterObstacleSize, new Color(0.28f, 0.29f, 0.31f));
        }

        private static DesktopInputReader CreateInput(Transform root)
        {
            var inputObject = new GameObject("Desktop Input Reader");
            inputObject.transform.SetParent(root, false);
            return inputObject.AddComponent<DesktopInputReader>();
        }

        private static ProjectileFactory CreateProjectileFactory(Transform root, ProjectileConfig config)
        {
            var factoryObject = new GameObject("Projectile Factory");
            factoryObject.transform.SetParent(root, false);
            var factory = factoryObject.AddComponent<ProjectileFactory>();
            factory.Configure(config);
            return factory;
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
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            var hitbox = root.AddComponent<BoxCollider>();
            hitbox.center = new Vector3(0f, 0.5f, 0f);
            hitbox.size = new Vector3(1.2f, 0.9f, 1.35f);

            var facade = root.AddComponent<TankFacade>();
            var movement = root.AddComponent<TankMovement>();
            var aiming = root.AddComponent<TurretAiming>();
            var shooter = root.AddComponent<TankShooter>();
            var health = root.AddComponent<TankHealth>();
            var controller = root.AddComponent<PlayerTankController>();

            var body = CreateCube(root.transform, "Body", new Vector3(0f, 0.28f, 0f), new Vector3(0.85f, 0.4f, 1.15f), color);
            RemoveCollider(body);
            var turret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            turret.name = "Turret";
            turret.transform.SetParent(root.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            turret.transform.localScale = new Vector3(0.32f, 0.12f, 0.32f);
            Tint(turret, Color.Lerp(color, Color.white, 0.18f));
            RemoveCollider(turret);

            var barrel = CreateCube(turret.transform, "Barrel", new Vector3(0f, 0f, 0.55f), new Vector3(0.16f, 0.16f, 0.85f), Color.Lerp(color, Color.black, 0.18f));
            RemoveCollider(barrel);
            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turret.transform, false);
            muzzle.localPosition = new Vector3(0f, 0f, 1.25f);

            movement.Configure(rigidbody, tankConfig.MoveSpeed, tankConfig.TurnSpeed);
            aiming.Configure(turret.transform, camera);
            shooter.Configure(muzzle, facade, projectileFactory, projectileConfig);
            health.Configure(tankConfig.MaxHp);
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

            var playerHpText = UiFactory.CreateText(canvas.transform, "PlayerHpText", new Vector2(-330f, 200f));
            var enemyHpText = UiFactory.CreateText(canvas.transform, "EnemyHpText", new Vector2(-330f, 168f));
            var lastHitText = UiFactory.CreateText(canvas.transform, "LastHitText", new Vector2(-330f, 136f), new Vector2(420f, 30f), TextAnchor.MiddleLeft);
            var roundResultText = UiFactory.CreateText(canvas.transform, "RoundResultText", new Vector2(0f, 200f), new Vector2(300f, 30f), TextAnchor.MiddleCenter);
            var controlsHintText = UiFactory.CreateText(canvas.transform, "ControlsHintText", new Vector2(0f, -210f), new Vector2(720f, 30f), TextAnchor.MiddleCenter);
            var restartButton = UiFactory.CreateButton(canvas.transform, "Restart", new Vector2(310f, 190f), null);

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

        private static void Tint(GameObject target, Color color)
        {
            if (target.TryGetComponent<Renderer>(out var renderer))
            {
                renderer.material.color = color;
            }
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
