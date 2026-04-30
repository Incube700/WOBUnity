using RicochetTanks.Configs;
using RicochetTanks.Gameplay.AI;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Match;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Infrastructure.Composition;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Input;
using RicochetTanks.Input.Desktop;
using RicochetTanks.Statistics;
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
        [SerializeField] private LaserAimConfig _laserAimConfig;
        [SerializeField] private LocalSessionConfig _sessionConfig;
        [SerializeField] private EnemyAiConfig _enemyAiConfig;

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
        private EnemyTankAiController _enemyAiController;
        private CombatFeedbackComposition _combatFeedbackComposition;
        private StatisticsComposition _statisticsComposition;
        private LocalMatchSessionService _sessionService;
        private GameplaySceneReferences _sceneReferences;
        private ITankInputReader _activeInputReader;

        private void Start()
        {
            ComposeScene();
        }

        private void OnDestroy()
        {
            _combatFeedbackComposition?.Dispose();
            _combatFeedbackComposition = null;
            _statisticsComposition?.Dispose();
            _statisticsComposition = null;
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
            BindEnemyAi();
            BindCombatFeedback();
            BindStatistics();
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
            ResolveLaserAimConfigFallback();
            _laserAimConfig = _laserAimConfig != null ? _laserAimConfig : ScriptableObject.CreateInstance<LaserAimConfig>();
            _sessionConfig = _sessionConfig != null ? _sessionConfig : ScriptableObject.CreateInstance<LocalSessionConfig>();
            _enemyAiConfig = _enemyAiConfig != null ? _enemyAiConfig : ScriptableObject.CreateInstance<EnemyAiConfig>();
            ResolveCombatVfxConfigFallback();
            _combatVfxConfig = _combatVfxConfig != null ? _combatVfxConfig : ScriptableObject.CreateInstance<CombatVfxConfig>();
        }

        private void ResolveLaserAimConfigFallback()
        {
#if UNITY_EDITOR
            if (_laserAimConfig == null)
            {
                _laserAimConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<LaserAimConfig>("Assets/_Project/RicochetTanks/Configs/LaserAimConfig.asset");
            }
#endif
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
            _sceneReferences = new GameplaySceneReferences(
                _arenaRoot,
                _playerSpawnPoint,
                _enemySpawnPoint,
                _playerTank,
                _enemyDummyTank,
                _cameraRig,
                _camera,
                _gameplayCanvas,
                _hudView,
                _combatFeedbackRoot);
            _sceneReferences.ResolveMissing(transform);
            ResolveCombatFeedbackPrefabFallbacks();
        }

        private void ApplyCameraConfig()
        {
            if (_sceneReferences.Camera == null || _cameraConfig == null)
            {
                return;
            }

            if (_sceneReferences.CameraRig != null)
            {
                _sceneReferences.Camera.transform.SetParent(_sceneReferences.CameraRig, false);
            }

            _sceneReferences.Camera.transform.localPosition = _cameraConfig.LocalPosition;
            _sceneReferences.Camera.transform.localRotation = Quaternion.Euler(_cameraConfig.LocalEulerAngles);
            _sceneReferences.Camera.clearFlags = CameraClearFlags.SolidColor;
            _sceneReferences.Camera.backgroundColor = _cameraConfig.BackgroundColor;
            _sceneReferences.Camera.orthographic = _cameraConfig.Orthographic;
            _sceneReferences.Camera.orthographicSize = _cameraConfig.OrthographicSize;
            _sceneReferences.Camera.nearClipPlane = _cameraConfig.NearClipPlane;
            _sceneReferences.Camera.farClipPlane = _cameraConfig.FarClipPlane;
        }

        private void EnsureInputReader()
        {
            _activeInputReader = new InputComposition(this, _inputMode, _mobileControlsPrefab, _inputReader)
                .CreateActiveInputReader();
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
            var tankFactory = new TankCompositionFactory(_sceneReferences.Camera, _activeInputReader, _projectileFactory, _projectileConfig, _laserAimConfig);
            tankFactory.ConfigureTank(_sceneReferences.PlayerTank, _sceneReferences.PlayerSpawnPoint, _playerTankConfig, true);
            tankFactory.ConfigureTank(_sceneReferences.EnemyDummyTank, _sceneReferences.EnemySpawnPoint, _enemyTankConfig, false);
        }

        private void BindEnemyAi()
        {
            if (_sceneReferences.EnemyDummyTank == null || _sceneReferences.PlayerTank == null)
            {
                return;
            }

            _enemyAiController = _sceneReferences.EnemyDummyTank.GetComponent<EnemyTankAiController>();
            if (_enemyAiController == null)
            {
                _enemyAiController = _sceneReferences.EnemyDummyTank.gameObject.AddComponent<EnemyTankAiController>();
            }

            _enemyAiController.Configure(
                _sceneReferences.EnemyDummyTank,
                _sceneReferences.PlayerTank,
                _enemyAiConfig,
                _gameplayEvents);
        }

        private void BindHud()
        {
            if (_sceneReferences.HudView == null)
            {
                _sceneReferences.HudView = new SandboxHudViewFactory().Create(_sceneReferences.GameplayCanvas);
                _sceneReferences.SetGameplayCanvas(_sceneReferences.HudView.GetComponentInParent<Canvas>());
            }

            _hudPresenter?.Dispose();
            _hudPresenter = new SandboxHudPresenter(
                _sceneReferences.HudView,
                _sceneReferences.PlayerTank.Health,
                _sceneReferences.EnemyDummyTank.Health,
                _gameplayEvents,
                RequestRestart,
                RequestExitToMainMenu,
                _matchConfig);
        }

        private void BindStatistics()
        {
            _sessionService = _sessionService ?? new LocalMatchSessionService();
            _statisticsComposition?.Dispose();
            _statisticsComposition = new StatisticsComposition(
                _gameplayEvents,
                _sceneReferences.PlayerTank,
                _sceneReferences.EnemyDummyTank,
                _sessionService);
        }

        private void BindCombatFeedback()
        {
            _combatFeedbackComposition?.Dispose();
            EnsureCombatFeedbackRoot();

            _combatFeedbackComposition = new CombatFeedbackComposition(
                _worldHealthBarPrefab,
                _floatingHitTextPrefab,
                _combatVfxConfig,
                _sceneReferences.CombatFeedbackRoot,
                _sceneReferences.Camera,
                _gameplayEvents,
                _sceneReferences.PlayerTank,
                _sceneReferences.EnemyDummyTank);
        }

        private void EnsureCombatFeedbackRoot()
        {
            _sceneReferences.EnsureCombatFeedbackRoot(transform);
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

            _matchController.Configure(
                _sceneReferences.PlayerTank,
                _sceneReferences.EnemyDummyTank,
                _gameplayEvents,
                _activeInputReader,
                _sceneLoaderService,
                _matchConfig,
                _sessionConfig,
                _sessionService);
        }

        private void RequestRestart()
        {
            _matchController?.RequestRestart();
        }

        private void RequestExitToMainMenu()
        {
            _matchController?.RequestExitToMainMenu();
        }

    }
}
