using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Match;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure.Composition;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Input;
using RicochetTanks.Input.Desktop;
using RicochetTanks.Input.Mobile;
using RicochetTanks.UI;
using RicochetTanks.UI.Sandbox;
using UnityEngine;

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

        [Header("Input")]
        [SerializeField] private TankInputMode _inputMode = TankInputMode.Auto;
        [SerializeField] private GameObject _mobileControlsPrefab;

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
        [SerializeField] private CombatVfxConfig _combatVfxConfig;
        [SerializeField] private Transform _combatFeedbackRoot;

        [Header("Runtime Services")]
        [SerializeField] private DesktopInputReader _inputReader;
        [SerializeField] private ProjectileFactory _projectileFactory;

        private readonly SceneLoaderService _sceneLoaderService = new SceneLoaderService();

        private SandboxGameplayEvents _gameplayEvents;
        private SandboxHudPresenter _hudPresenter;
        private SandboxMatchController _matchController;
        private CombatFeedbackComposition _combatFeedbackComposition;
        private ITankInputReader _activeInputReader;
        private MobileInputReader _mobileInputReader;
        private MobileControlsView _mobileControlsView;

        private void Start()
        {
            ComposeScene();
        }

        private void OnDestroy()
        {
            _combatFeedbackComposition?.Dispose();
            _combatFeedbackComposition = null;
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
            ConfigureTanks();
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
            ResolveCombatVfxConfigFallback();
            _combatVfxConfig = _combatVfxConfig != null ? _combatVfxConfig : ScriptableObject.CreateInstance<CombatVfxConfig>();
        }

        private void ResolveCombatVfxConfigFallback()
        {
#if UNITY_EDITOR
            if (_combatVfxConfig == null)
            {
                _combatVfxConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<CombatVfxConfig>("Assets/_Project/RicochetTanks/Configs/CombatVfxConfig.asset");
            }
#endif
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
            EnsureDesktopInputReader();

            if (ResolveInputMode() == TankInputMode.Mobile)
            {
                EnsureMobileInputReader();
                return;
            }

            SetMobileControlsVisible(false);
            _activeInputReader = _inputReader;
        }

        private void EnsureDesktopInputReader()
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

        private void EnsureMobileInputReader()
        {
            if (_mobileInputReader == null)
            {
                _mobileInputReader = GetComponent<MobileInputReader>();
            }

            if (_mobileInputReader == null)
            {
                _mobileInputReader = gameObject.AddComponent<MobileInputReader>();
            }

            if (_mobileControlsView == null)
            {
                _mobileControlsView = CreateMobileControlsView();
            }

            SetMobileControlsVisible(true);
            _mobileInputReader.Configure(_mobileControlsView);
            _activeInputReader = _mobileInputReader;
        }

        private TankInputMode ResolveInputMode()
        {
            if (_inputMode != TankInputMode.Auto)
            {
                return _inputMode;
            }

            return Application.isMobilePlatform ? TankInputMode.Mobile : TankInputMode.Desktop;
        }

        private MobileControlsView CreateMobileControlsView()
        {
            if (_mobileControlsPrefab != null)
            {
                var controlsObject = Instantiate(_mobileControlsPrefab, transform);
                controlsObject.name = _mobileControlsPrefab.name;

                if (controlsObject.TryGetComponent<MobileControlsView>(out var prefabView))
                {
                    return prefabView;
                }

                var childView = controlsObject.GetComponentInChildren<MobileControlsView>(true);
                if (childView != null)
                {
                    return childView;
                }

                Debug.LogWarning("[MOBILE_INPUT] Mobile controls prefab has no MobileControlsView. Runtime controls will be created.");
                Destroy(controlsObject);
            }

            return MobileControlsView.CreateDefault("MobileControlsCanvas", transform);
        }

        private void SetMobileControlsVisible(bool isVisible)
        {
            if (_mobileControlsView != null)
            {
                _mobileControlsView.gameObject.SetActive(isVisible);
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

        private void ConfigureTanks()
        {
            var tankFactory = new TankCompositionFactory(_camera, _activeInputReader, _projectileFactory, _projectileConfig);
            tankFactory.ConfigureTank(_playerTank, _playerSpawnPoint, _playerTankConfig, true);
            tankFactory.ConfigureTank(_enemyDummyTank, _enemySpawnPoint, _enemyTankConfig, false);
        }

        private void BindHud()
        {
            if (_hudView == null)
            {
                _hudView = new SandboxHudViewFactory().Create(_gameplayCanvas);
                _gameplayCanvas = _hudView.GetComponentInParent<Canvas>();
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
            _combatFeedbackComposition?.Dispose();
            EnsureCombatFeedbackRoot();

            _combatFeedbackComposition = new CombatFeedbackComposition(
                _worldHealthBarPrefab,
                _floatingHitTextPrefab,
                _combatVfxConfig,
                _combatFeedbackRoot,
                _camera,
                _gameplayEvents,
                _playerTank,
                _enemyDummyTank);
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

        private void BindMatch()
        {
            _matchController = GetComponent<SandboxMatchController>();
            if (_matchController == null)
            {
                _matchController = gameObject.AddComponent<SandboxMatchController>();
            }

            _matchController.Configure(_playerTank, _enemyDummyTank, _gameplayEvents, _activeInputReader, _sceneLoaderService, _matchConfig);
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

    }
}
