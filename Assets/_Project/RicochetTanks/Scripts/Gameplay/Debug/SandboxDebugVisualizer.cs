using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.DebugTools
{
    public sealed class SandboxDebugVisualizer : MonoBehaviour
    {
        private const float LabelHeight = 22f;

        private ArenaConfig _arenaConfig;
        private DebugVisualizationConfig _config;
        private TankFacade _player;
        private TankFacade _enemy;
        private SandboxGameplayEvents _gameplayEvents;
        private Camera _camera;
        private Vector3 _playerSpawn;
        private Vector3 _enemySpawn;
        private Vector3 _lastProjectilePosition;
        private Vector3 _lastProjectileDirection;
        private Vector3 _lastHitPoint;
        private Vector3 _lastHitNormal;
        private int _lastBounceCount;
        private int _lastBouncesLeft;
        private float _lastProjectileDamage;
        private ArmorHitInfo _lastArmorHit;
        private string _lastHitLabel = "Hit: none";
        private string _enemyFsmState = "DummyIdle";
        private bool _hasProjectile;
        private bool _hasHit;
        private bool _isSubscribed;

        public void Configure(
            ArenaConfig arenaConfig,
            DebugVisualizationConfig config,
            TankFacade player,
            TankFacade enemy,
            SandboxGameplayEvents gameplayEvents,
            Camera camera)
        {
            Unsubscribe();

            _arenaConfig = arenaConfig;
            _config = config;
            _player = player;
            _enemy = enemy;
            _gameplayEvents = gameplayEvents;
            _camera = camera;

            if (_arenaConfig != null)
            {
                _playerSpawn = _arenaConfig.PlayerStartPosition;
                _enemySpawn = _arenaConfig.EnemyStartPosition;
            }

            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void OnDrawGizmos()
        {
            if (!IsEnabled())
            {
                return;
            }

            DrawArenaBounds();
            DrawSpawnPoints();
            DrawProjectileDebug();
            DrawHitNormal();
        }

        private void OnGUI()
        {
            if (!IsEnabled() || _config == null || !_config.DrawLabels)
            {
                return;
            }

            DrawLabel(_playerSpawn + Vector3.up * 0.25f, "Player Spawn");
            DrawLabel(_enemySpawn + Vector3.up * 0.25f, "Enemy Spawn");

            if (_enemy != null)
            {
                DrawLabel(_enemy.transform.position + Vector3.up * 0.95f, $"Enemy FSM: {_enemyFsmState}");
            }

            if (_hasProjectile)
            {
                DrawLabel(_lastProjectilePosition + Vector3.up * 0.55f, $"Bounce: {_lastBounceCount}  Left: {_lastBouncesLeft}  Damage: {_lastProjectileDamage:0.##}");
            }

            if (_hasHit)
            {
                DrawLabel(_lastHitPoint + Vector3.up * 0.65f, _lastHitLabel);
            }
        }

        private void Subscribe()
        {
            if (_isSubscribed || _gameplayEvents == null)
            {
                return;
            }

            _gameplayEvents.ProjectileSpawned += OnProjectileSpawned;
            _gameplayEvents.ProjectileHit += OnProjectileHit;
            _gameplayEvents.ProjectileBounced += OnProjectileBounced;
            _gameplayEvents.HitResolved += OnHitResolved;
            _gameplayEvents.MatchStarted += OnMatchStarted;
            _gameplayEvents.MatchFinished += OnMatchFinished;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _gameplayEvents == null)
            {
                return;
            }

            _gameplayEvents.ProjectileSpawned -= OnProjectileSpawned;
            _gameplayEvents.ProjectileHit -= OnProjectileHit;
            _gameplayEvents.ProjectileBounced -= OnProjectileBounced;
            _gameplayEvents.HitResolved -= OnHitResolved;
            _gameplayEvents.MatchStarted -= OnMatchStarted;
            _gameplayEvents.MatchFinished -= OnMatchFinished;
            _isSubscribed = false;
        }

        private void DrawArenaBounds()
        {
            if (_arenaConfig == null)
            {
                return;
            }

            var size = _arenaConfig.HalfSize * 2f;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size, 0.05f, size));
        }

        private void DrawSpawnPoints()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_playerSpawn + Vector3.up * 0.2f, 0.35f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_enemySpawn + Vector3.up * 0.2f, 0.35f);
        }

        private void DrawProjectileDebug()
        {
            if (!_hasProjectile)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_lastProjectilePosition, _lastProjectilePosition + _lastProjectileDirection * 1.25f);

            if (_config != null && _config.DrawPredictedNextSegment)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(_lastProjectilePosition, _lastProjectilePosition + _lastProjectileDirection * _config.PredictedSegmentLength);
            }
        }

        private void DrawHitNormal()
        {
            if (!_hasHit)
            {
                return;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_lastHitPoint, _lastHitPoint + _lastHitNormal * 1.2f);
            Gizmos.DrawWireSphere(_lastHitPoint, 0.12f);
        }

        private void DrawLabel(Vector3 worldPosition, string text)
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
            {
                return;
            }

            var screenPosition = _camera.WorldToScreenPoint(worldPosition);
            if (screenPosition.z <= 0f)
            {
                return;
            }

            var labelSize = GUI.skin.label.CalcSize(new GUIContent(text));
            var rect = new Rect(
                screenPosition.x - labelSize.x * 0.5f,
                Screen.height - screenPosition.y - LabelHeight * 0.5f,
                labelSize.x + 8f,
                LabelHeight);
            GUI.Label(rect, text);
        }

        private void OnProjectileSpawned(ProjectileSpawnedEvent projectile)
        {
            _hasProjectile = true;
            _lastProjectilePosition = projectile.Position;
            _lastProjectileDirection = projectile.Direction.normalized;
            _lastBounceCount = 0;
            _lastBouncesLeft = projectile.BouncesLeft;
            _lastProjectileDamage = projectile.Damage;
        }

        private void OnProjectileHit(ProjectileHitEvent projectileHit)
        {
            _hasHit = true;
            _lastHitPoint = projectileHit.Point;
            _lastHitNormal = projectileHit.Normal.normalized;
            _lastProjectilePosition = projectileHit.Point;
            _lastProjectileDirection = projectileHit.Direction.normalized;
        }

        private void OnProjectileBounced(ProjectileBouncedEvent projectileBounce)
        {
            _lastBounceCount = projectileBounce.RicochetCount;
            _lastBouncesLeft = projectileBounce.BouncesLeft;
            _lastProjectileDamage = projectileBounce.Damage;

            if (projectileBounce.Projectile != null)
            {
                _lastProjectilePosition = projectileBounce.Projectile.transform.position;
            }
        }

        private void OnHitResolved(HitResolvedEvent hit)
        {
            _lastArmorHit = hit.ArmorHit;
            _lastHitLabel = $"Zone: {_lastArmorHit.Zone}  Angle: {_lastArmorHit.HitAngle:0}  Pen: {_lastArmorHit.Penetration}  Armor: {_lastArmorHit.EffectiveArmor}";
        }

        private void OnMatchStarted()
        {
            _enemyFsmState = "DummyIdle";
            _lastHitLabel = "Hit: none";
            _hasProjectile = false;
            _hasHit = false;
            _lastBounceCount = 0;
            _lastBouncesLeft = 0;
            _lastProjectileDamage = 0f;
        }

        private void OnMatchFinished(MatchFinishedEvent match)
        {
            _enemyFsmState = "Disabled";
        }

        private bool IsEnabled()
        {
            return _config != null && _config.IsEnabled;
        }
    }
}
