using System;
using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Input.Desktop;
using RicochetTanks.UI;
using RicochetTanks.UI.CombatFeedback;
using RicochetTanks.UI.Sandbox;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Infrastructure.Bootstrap
{
    public sealed class GameplayEntryPoint : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private MatchConfig _matchConfig;
        [SerializeField] private CameraConfig _cameraConfig;
        [SerializeField] private TankConfig _playerTankConfig;
        [SerializeField] private TankConfig _enemyTankConfig;
        [SerializeField] private ProjectileConfig _projectileConfig;
        [SerializeField] private DebugLogConfig _debugLogConfig;

        [Header("Scene References")]
        [SerializeField] private Transform _arenaRoot;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private Transform _enemySpawnPoint;
        [SerializeField] private TankFacade _playerTank;
        [SerializeField] private TankFacade _enemyDummyTank;
        [SerializeField] private Transform _cameraRig;
        [SerializeField] private Camera _camera;
        [SerializeField] private Canvas _gameplayCanvas;
        [SerializeField] private SandboxHudView _hudView;

        [Header("Combat Feedback")]
        [SerializeField] private GameObject _worldHealthBarPrefab;
        [SerializeField] private GameObject _floatingHitTextPrefab;
        [SerializeField] private Transform _combatFeedbackRoot;

        [Header("Runtime Services")]
        [SerializeField] private DesktopInputReader _inputReader;
        [SerializeField] private ProjectileFactory _projectileFactory;

        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        private SandboxGameplayEvents _gameplayEvents;
        private SandboxHudPresenter _hudPresenter;
        private SandboxMatchController _matchController;
        private CombatFeedbackFactory _combatFeedbackFactory;
        private CombatFeedbackPresenter _combatFeedbackPresenter;
        private TankHealthBarPresenter _playerHealthBarPresenter;
        private TankHealthBarPresenter _enemyHealthBarPresenter;
        private TankHealthBarView _playerHealthBarView;
        private TankHealthBarView _enemyHealthBarView;

        private void Start()
        {
            ComposeScene();
        }

        private void OnDestroy()
        {
            DisposeCombatFeedback();
            _hudPresenter?.Dispose();
            _hudPresenter = null;
        }

        private void ComposeScene()
        {
            ResolveConfigFallbacks();
            ResolveSceneReferences();
            ApplyCameraConfig();

            _gameplayEvents = new SandboxGameplayEvents(_debugLogConfig);
            EnsureInputReader();
            EnsureProjectileFactory();
            ConfigureTank(_playerTank, _playerSpawnPoint, _playerTankConfig, true);
            ConfigureTank(_enemyDummyTank, _enemySpawnPoint, _enemyTankConfig, false);
            BindCombatFeedback();
            BindHud();
            BindMatch();
            UiFactory.EnsureEventSystem("Gameplay EventSystem");
        }

        private void ResolveConfigFallbacks()
        {
            _matchConfig = _matchConfig != null ? _matchConfig : ScriptableObject.CreateInstance<MatchConfig>();
            _cameraConfig = _cameraConfig != null ? _cameraConfig : ScriptableObject.CreateInstance<CameraConfig>();
            _playerTankConfig = _playerTankConfig != null ? _playerTankConfig : ScriptableObject.CreateInstance<TankConfig>();
            _enemyTankConfig = _enemyTankConfig != null ? _enemyTankConfig : _playerTankConfig;
            _projectileConfig = _projectileConfig != null ? _projectileConfig : ScriptableObject.CreateInstance<ProjectileConfig>();
        }

        private void ResolveSceneReferences()
        {
            _arenaRoot = _arenaRoot != null ? _arenaRoot : transform.Find("ArenaRoot");
            _playerSpawnPoint = _playerSpawnPoint != null ? _playerSpawnPoint : FindDescendant(transform, "PlayerSpawnPoint");
            _enemySpawnPoint = _enemySpawnPoint != null ? _enemySpawnPoint : FindDescendant(transform, "EnemySpawnPoint");
            _playerTank = _playerTank != null ? _playerTank : ResolveTankReference("PlayerTank");
            _enemyDummyTank = _enemyDummyTank != null ? _enemyDummyTank : ResolveTankReference("EnemyDummyTank");
            _cameraRig = _cameraRig != null ? _cameraRig : FindDescendant(transform, "CameraRig");
            _camera = _camera != null ? _camera : GetComponentInChildren<Camera>(true);
            _gameplayCanvas = _gameplayCanvas != null ? _gameplayCanvas : GetComponentInChildren<Canvas>(true);
            _hudView = _hudView != null ? _hudView : GetComponentInChildren<SandboxHudView>(true);
            _combatFeedbackRoot = _combatFeedbackRoot != null ? _combatFeedbackRoot : FindDescendant(transform, "CombatFeedbackRoot");
            ResolveCombatFeedbackPrefabFallbacks();
        }

        private void ApplyCameraConfig()
        {
            if (_camera == null || _cameraConfig == null)
            {
                return;
            }

            if (_cameraRig != null)
            {
                _camera.transform.SetParent(_cameraRig, false);
            }

            _camera.transform.localPosition = _cameraConfig.LocalPosition;
            _camera.transform.localRotation = Quaternion.Euler(_cameraConfig.LocalEulerAngles);
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = _cameraConfig.BackgroundColor;
            _camera.orthographic = _cameraConfig.Orthographic;
            _camera.orthographicSize = _cameraConfig.OrthographicSize;
            _camera.nearClipPlane = _cameraConfig.NearClipPlane;
            _camera.farClipPlane = _cameraConfig.FarClipPlane;
        }

        private void EnsureInputReader()
        {
            if (_inputReader == null)
            {
                _inputReader = GetComponent<DesktopInputReader>();
            }

            if (_inputReader == null)
            {
                _inputReader = gameObject.AddComponent<DesktopInputReader>();
            }
        }

        private void EnsureProjectileFactory()
        {
            if (_projectileFactory == null)
            {
                _projectileFactory = GetComponent<ProjectileFactory>();
            }

            if (_projectileFactory == null)
            {
                _projectileFactory = gameObject.AddComponent<ProjectileFactory>();
            }

            _projectileFactory.Configure(_projectileConfig, _gameplayEvents);
        }

        private void ConfigureTank(TankFacade tank, Transform spawnPoint, TankConfig tankConfig, bool isPlayerControlled)
        {
            if (tank == null)
            {
                throw new InvalidOperationException($"Missing {(isPlayerControlled ? "player" : "enemy")} tank scene reference.");
            }

            if (spawnPoint != null)
            {
                tank.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            }

            var body = tank.gameObject;
            var rigidbody = GetOrAdd<Rigidbody>(body);
            rigidbody.useGravity = false;
            rigidbody.isKinematic = !isPlayerControlled;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            var hitbox = GetOrAdd<BoxCollider>(body);
            hitbox.center = new Vector3(0f, 0.5f, 0f);
            hitbox.size = new Vector3(1.2f, 0.9f, 1.35f);

            var movement = GetOrAdd<TankMovement>(body);
            var aiming = GetOrAdd<TurretAiming>(body);
            var shooter = GetOrAdd<TankShooter>(body);
            var health = GetOrAdd<TankHealth>(body);
            var armor = GetOrAdd<TankArmor>(body);
            var controller = GetOrAdd<PlayerTankController>(body);
            var turret = FindDescendant(tank.transform, "Turret");
            if (turret == null)
            {
                turret = CreateChild(tank.transform, "Turret", new Vector3(0f, 0.62f, 0f));
            }

            var muzzle = FindDescendant(turret, "Muzzle");
            if (muzzle == null)
            {
                muzzle = CreateChild(turret, "Muzzle", new Vector3(0f, 0f, 1.25f));
            }

            movement.Configure(
                rigidbody,
                tankConfig.MaxForwardSpeed,
                tankConfig.MaxReverseSpeed,
                tankConfig.Acceleration,
                tankConfig.BrakeDeceleration,
                tankConfig.CoastDeceleration,
                tankConfig.TurnSpeed,
                tankConfig.TurnSpeedAtLowVelocity,
                tankConfig.InputDeadZone);
            aiming.Configure(turret, _camera, tankConfig.TurretRotationSpeed);
            shooter.Configure(muzzle, tank, _projectileFactory, _projectileConfig);
            health.Configure(tankConfig.MaxHp);
            armor.Configure(tankConfig);
            controller.Configure(tank, _inputReader, _camera);
            tank.Configure(movement, aiming, shooter, health, controller);
            tank.SetPlayerControlled(isPlayerControlled);
        }

        private void BindHud()
        {
            if (_hudView == null)
            {
                _hudView = CreateHudView();
            }

            _hudPresenter?.Dispose();
            _hudPresenter = new SandboxHudPresenter(
                _hudView,
                _playerTank.Health,
                _enemyDummyTank.Health,
                _gameplayEvents,
                RequestRestart,
                _matchConfig);
        }

        private void BindCombatFeedback()
        {
            DisposeCombatFeedback();
            EnsureCombatFeedbackRoot();

            _combatFeedbackFactory = new CombatFeedbackFactory(_worldHealthBarPrefab, _floatingHitTextPrefab, _combatFeedbackRoot, _camera);

            var playerHealthBar = _combatFeedbackFactory.CreateHealthBar(_playerTank);
            if (playerHealthBar != null)
            {
                _playerHealthBarView = playerHealthBar;
                _playerHealthBarPresenter = new TankHealthBarPresenter(playerHealthBar, _playerTank.Health);
            }

            var enemyHealthBar = _combatFeedbackFactory.CreateHealthBar(_enemyDummyTank);
            if (enemyHealthBar != null)
            {
                _enemyHealthBarView = enemyHealthBar;
                _enemyHealthBarPresenter = new TankHealthBarPresenter(enemyHealthBar, _enemyDummyTank.Health);
            }

            _combatFeedbackPresenter = new CombatFeedbackPresenter(_gameplayEvents, _combatFeedbackFactory);
        }

        private void DisposeCombatFeedback()
        {
            _combatFeedbackPresenter?.Dispose();
            _playerHealthBarPresenter?.Dispose();
            _enemyHealthBarPresenter?.Dispose();
            DestroyHealthBar(_playerHealthBarView);
            DestroyHealthBar(_enemyHealthBarView);
            _combatFeedbackPresenter = null;
            _playerHealthBarPresenter = null;
            _enemyHealthBarPresenter = null;
            _playerHealthBarView = null;
            _enemyHealthBarView = null;
            _combatFeedbackFactory = null;
        }

        private static void DestroyHealthBar(TankHealthBarView healthBarView)
        {
            if (healthBarView == null)
            {
                return;
            }

            Destroy(healthBarView.gameObject);
        }

        private void EnsureCombatFeedbackRoot()
        {
            if (_combatFeedbackRoot != null)
            {
                return;
            }

            _combatFeedbackRoot = CreateChild(transform, "CombatFeedbackRoot", Vector3.zero);
        }

        private void ResolveCombatFeedbackPrefabFallbacks()
        {
#if UNITY_EDITOR
            if (_worldHealthBarPrefab == null)
            {
                _worldHealthBarPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/RicochetTanks/Prefabs/UI/WorldHealthBarPrefab.prefab");
            }

            if (_floatingHitTextPrefab == null)
            {
                _floatingHitTextPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/RicochetTanks/Prefabs/UI/FloatingHitTextPrefab.prefab");
            }
#endif
        }

        private SandboxHudView CreateHudView()
        {
            var canvas = _gameplayCanvas != null ? _gameplayCanvas : UiFactory.CreateCanvas("GameplayCanvas");
            _gameplayCanvas = canvas;

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

        private void BindMatch()
        {
            _matchController = GetComponent<SandboxMatchController>();
            if (_matchController == null)
            {
                _matchController = gameObject.AddComponent<SandboxMatchController>();
            }

            _matchController.Configure(_playerTank, _enemyDummyTank, _gameplayEvents, _inputReader, _sceneLoaderService, _matchConfig);
        }

        private void RequestRestart()
        {
            _matchController?.RequestRestart();
        }

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            if (target.TryGetComponent<T>(out var component))
            {
                return component;
            }

            return target.AddComponent<T>();
        }

        private static Transform FindDescendant(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == objectName)
            {
                return root;
            }

            for (var index = 0; index < root.childCount; index++)
            {
                var result = FindDescendant(root.GetChild(index), objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static Transform CreateChild(Transform parent, string objectName, Vector3 localPosition)
        {
            var child = new GameObject(objectName).transform;
            child.SetParent(parent, false);
            child.localPosition = localPosition;
            return child;
        }

        private TankFacade ResolveTankReference(string objectName)
        {
            var child = FindDescendant(transform, objectName);
            return child != null ? GetOrAdd<TankFacade>(child.gameObject) : null;
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
