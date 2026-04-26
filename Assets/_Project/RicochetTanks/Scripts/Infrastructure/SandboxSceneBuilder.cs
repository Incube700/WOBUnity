using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.UI;
using RicochetTanks.UI.Sandbox;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RicochetTanks.Infrastructure
{
    public sealed class SandboxSceneContext
    {
        public SandboxSceneContext(TankFacade player, TankFacade enemy, SandboxHudView hudView, Camera camera)
        {
            Player = player;
            Enemy = enemy;
            HudView = hudView;
            Camera = camera;
        }

        public TankFacade Player { get; }
        public TankFacade Enemy { get; }
        public SandboxHudView HudView { get; }
        public Camera Camera { get; }
    }

    public static class SandboxSceneBuilder
    {
        private const float ArenaHalfSize = 5f;
        private const float WallThickness = 0.35f;

        public static SandboxSceneContext Build(Transform root)
        {
            ClearChildren(root);

            var camera = CreateCamera(root);
            CreateLight(root);
            CreateArena(root);

            var player = CreateTank("Player Tank", root, new Vector3(-3.75f, 0f, -3.75f), new Color(0.2f, 0.85f, 0.35f), camera, true);
            var enemy = CreateTank("Enemy Dummy Tank", root, new Vector3(3.75f, 0f, 3.75f), new Color(0.95f, 0.25f, 0.2f), camera, false);
            var hudView = CreateHud(root);
            CreateEventSystem(root);

            return new SandboxSceneContext(player, enemy, hudView, camera);
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
            var cameraObject = new GameObject("Sand Box Camera");
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

        private static void CreateArena(Transform root)
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "10x10 Arena Floor";
            floor.transform.SetParent(root, false);
            floor.transform.localScale = Vector3.one;
            floor.isStatic = true;
            Tint(floor, new Color(0.36f, 0.38f, 0.4f));

            var wallLength = ArenaHalfSize * 2f + WallThickness;
            CreateCube(root, "North Wall", new Vector3(0f, 0.5f, ArenaHalfSize + WallThickness * 0.5f), new Vector3(wallLength, 1f, WallThickness), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "South Wall", new Vector3(0f, 0.5f, -ArenaHalfSize - WallThickness * 0.5f), new Vector3(wallLength, 1f, WallThickness), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "East Wall", new Vector3(ArenaHalfSize + WallThickness * 0.5f, 0.5f, 0f), new Vector3(WallThickness, 1f, wallLength), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "West Wall", new Vector3(-ArenaHalfSize - WallThickness * 0.5f, 0.5f, 0f), new Vector3(WallThickness, 1f, wallLength), new Color(0.22f, 0.23f, 0.25f));
            CreateCube(root, "Center Square Obstacle", new Vector3(0f, 0.5f, 0f), new Vector3(2f, 1f, 2f), new Color(0.28f, 0.29f, 0.31f));
        }

        private static TankFacade CreateTank(string name, Transform parent, Vector3 position, Color color, Camera camera, bool isPlayerControlled)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.position = position;
            root.transform.rotation = Quaternion.Euler(0f, isPlayerControlled ? 45f : -135f, 0f);

            var rigidbody = root.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            var facade = root.AddComponent<TankFacade>();
            var movement = root.AddComponent<TankMovement>();
            var aiming = root.AddComponent<TurretAiming>();
            var shooter = root.AddComponent<TankShooter>();
            var health = root.AddComponent<TankHealth>();
            var controller = root.AddComponent<PlayerTankController>();

            var body = CreateCube(root.transform, "Body", new Vector3(0f, 0.28f, 0f), new Vector3(0.85f, 0.4f, 1.15f), color);
            var turret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            turret.name = "Turret";
            turret.transform.SetParent(root.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.6f, 0f);
            turret.transform.localScale = new Vector3(0.32f, 0.12f, 0.32f);
            Tint(turret, Color.Lerp(color, Color.white, 0.18f));

            var barrel = CreateCube(turret.transform, "Barrel", new Vector3(0f, 0f, 0.55f), new Vector3(0.16f, 0.16f, 0.85f), Color.Lerp(color, Color.black, 0.18f));
            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turret.transform, false);
            muzzle.localPosition = new Vector3(0f, 0f, 1.25f);

            movement.Configure(rigidbody);
            aiming.Configure(turret.transform, camera);
            shooter.Configure(muzzle, facade);
            health.Configure(100);
            facade.Configure(movement, aiming, shooter, health, controller);
            facade.SetPlayerControlled(isPlayerControlled);

            body.isStatic = false;
            barrel.isStatic = false;
            turret.isStatic = false;
            return facade;
        }

        private static SandboxHudView CreateHud(Transform root)
        {
            var canvas = UiFactory.CreateCanvas("Sand Box HUD Canvas");
            canvas.transform.SetParent(root, false);

            var playerHpText = UiFactory.CreateText(canvas.transform, "PlayerHpText", new Vector2(-330f, 190f));
            var enemyHpText = UiFactory.CreateText(canvas.transform, "EnemyHpText", new Vector2(-330f, 155f));
            var restartButton = UiFactory.CreateButton(canvas.transform, "Restart", new Vector2(310f, 180f), null);

            var hudView = canvas.gameObject.AddComponent<SandboxHudView>();
            hudView.Configure(playerHpText, enemyHpText, restartButton);
            return hudView;
        }

        private static void CreateEventSystem(Transform root)
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(root, false);
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
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
