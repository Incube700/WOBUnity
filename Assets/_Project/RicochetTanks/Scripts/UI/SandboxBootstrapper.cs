using RicochetTanks.Gameplay;
using RicochetTanks.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RicochetTanks.UI
{
    public class SandboxBootstrapper : MonoBehaviour
    {
        private const string SandboxSceneName = "RicochetTanks_Sandbox";

        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();
        private Text _playerHpText;
        private Text _enemyHpText;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureSandboxObjects()
        {
            if (SceneManager.GetActiveScene().name != SandboxSceneName)
            {
                return;
            }

            var bootstrapperObject = new GameObject(nameof(SandboxBootstrapper));
            bootstrapperObject.AddComponent<SandboxBootstrapper>();
        }

        private void Start()
        {
            SetupCamera();
            BuildArena();
            var player = CreateTank("PlayerTank", new Vector3(-4f, 0.5f, -4f), Color.green);
            var enemy = CreateTank("EnemyTank", new Vector3(4f, 0.5f, 4f), Color.red);
            enemy.GetComponent<PlayerTankController>().enabled = false;

            BuildHud(player.GetComponent<TankHealth>(), enemy.GetComponent<TankHealth>());
        }

        private static void SetupCamera()
        {
            var cameraObject = new GameObject("SandboxCamera");
            var camera = cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0f, 16f, 0f);
            cameraObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.09f, 0.09f, 0.09f);
            camera.orthographic = true;
            camera.orthographicSize = 6f;
        }

        private static void BuildArena()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "ArenaFloor";
            floor.transform.localScale = new Vector3(1f, 1f, 1f);

            CreateWall(new Vector3(0f, 0.5f, 5f), new Vector3(10f, 1f, 1f));
            CreateWall(new Vector3(0f, 0.5f, -5f), new Vector3(10f, 1f, 1f));
            CreateWall(new Vector3(5f, 0.5f, 0f), new Vector3(1f, 1f, 10f));
            CreateWall(new Vector3(-5f, 0.5f, 0f), new Vector3(1f, 1f, 10f));
            CreateWall(new Vector3(0f, 0.5f, 0f), new Vector3(2f, 1f, 2f));
        }

        private static void CreateWall(Vector3 position, Vector3 scale)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material.color = Color.gray;
        }

        private static GameObject CreateTank(string name, Vector3 position, Color bodyColor)
        {
            var root = new GameObject(name);
            root.transform.position = position;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(0.9f, 0.5f, 1.2f);
            body.GetComponent<Renderer>().material.color = bodyColor;

            var turret = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            turret.transform.SetParent(root.transform, false);
            turret.transform.localScale = new Vector3(0.25f, 0.15f, 0.25f);
            turret.transform.localPosition = new Vector3(0f, 0.4f, 0f);

            var muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(turret.transform, false);
            muzzle.localPosition = new Vector3(0f, 0f, 0.8f);

            var rigidbody = root.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.useGravity = false;

            var movement = root.AddComponent<TankMovement>();
            var aiming = root.AddComponent<TurretAiming>();
            var shooter = root.AddComponent<TankShooter>();
            var health = root.AddComponent<TankHealth>();
            var controller = root.AddComponent<PlayerTankController>();

            var aimingType = typeof(TurretAiming);
            aimingType.GetField("_turret", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(aiming, turret.transform);
            var shooterType = typeof(TankShooter);
            shooterType.GetField("_muzzle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(shooter, muzzle);

            var controllerType = typeof(PlayerTankController);
            controllerType.GetField("_movement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, movement);
            controllerType.GetField("_aiming", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, aiming);
            controllerType.GetField("_shooter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, shooter);

            root.AddComponent<BoxCollider>();

            return root;
        }

        private void BuildHud(TankHealth playerHealth, TankHealth enemyHealth)
        {
            var canvas = UiFactory.CreateCanvas("SandboxHudCanvas");
            _playerHpText = UiFactory.CreateText(canvas.transform, "PlayerHpText", new Vector2(-200f, 140f));
            _enemyHpText = UiFactory.CreateText(canvas.transform, "EnemyHpText", new Vector2(-200f, 110f));
            UiFactory.CreateButton(canvas.transform, "Restart", new Vector2(250f, 130f), OnRestartClicked);

            playerHealth.HealthChanged += OnPlayerHealthChanged;
            enemyHealth.HealthChanged += OnEnemyHealthChanged;

            OnPlayerHealthChanged(playerHealth.CurrentHp, 100);
            OnEnemyHealthChanged(enemyHealth.CurrentHp, 100);
        }

        private void OnPlayerHealthChanged(int currentHp, int maxHp)
        {
            _playerHpText.text = $"Player HP: {currentHp}/{maxHp}";
        }

        private void OnEnemyHealthChanged(int currentHp, int maxHp)
        {
            _enemyHpText.text = $"Enemy HP: {currentHp}/{maxHp}";
        }

        private void OnRestartClicked()
        {
            _sceneLoaderService.ReloadActiveScene();
        }
    }
}
