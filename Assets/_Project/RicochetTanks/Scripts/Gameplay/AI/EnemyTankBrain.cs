using System;
using RicochetTanks.Gameplay.AI.States;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.AI
{
    public sealed class EnemyTankBrain : IDisposable
    {
        private const float AimHeight = 0.6f;
        private const float ShootRequestBaseInterval = 0.2f;
        private const float MovementFullTurnAngle = 90f;
        private const float MovementFullThrottleAngle = 25f;
        private const float MovementZeroThrottleAngle = 140f;
        private const int RaycastBufferSize = 8;

        private readonly TankFacade _enemy;
        private readonly TankFacade _target;
        private readonly EnemyAiConfig _config;
        private readonly SandboxGameplayEvents _gameplayEvents;
        private readonly EnemyAiStateMachine _stateMachine;
        private readonly IdleState _idleState;
        private readonly AimAndShootState _aimAndShootState;
        private readonly KeepDistanceState _keepDistanceState;
        private readonly RepositionState _repositionState;
        private readonly AvoidObstacleState _avoidObstacleState;
        private readonly DeadState _deadState;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[RaycastBufferSize];

        private Vector3 _lastTargetPosition;
        private Vector3 _targetVelocity;
        private float _repositionTimer;
        private float _nextShootRequestTime;
        private float _shootReadyTime;
        private float _repositionTurnDirection = 1f;
        private float _avoidTurnDirection = 1f;
        private float _currentAimErrorAngle;
        private float _nextAimErrorChangeTime;
        private bool _hasTargetPositionSnapshot;
        private bool _isShootReadinessStarted;
        private bool _isEnabled;
        private bool _isDisposed;

        public EnemyTankBrain(
            TankFacade enemy,
            TankFacade target,
            EnemyAiConfig config,
            SandboxGameplayEvents gameplayEvents)
        {
            _enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _gameplayEvents = gameplayEvents;
            _stateMachine = new EnemyAiStateMachine();
            _idleState = new IdleState(this);
            _aimAndShootState = new AimAndShootState(this);
            _keepDistanceState = new KeepDistanceState(this);
            _repositionState = new RepositionState(this);
            _avoidObstacleState = new AvoidObstacleState(this);
            _deadState = new DeadState(this);
            _repositionTimer = _config.RepositionInterval;

            if (_enemy.Health != null)
            {
                _enemy.Health.Died += OnEnemyDied;
            }

            if (_gameplayEvents != null)
            {
                _gameplayEvents.RoundStarted += OnRoundStarted;
                _gameplayEvents.RoundFinished += OnRoundFinished;
                _gameplayEvents.MatchFinished += OnMatchFinished;
            }

            if (_enemy.Health != null && !_enemy.Health.IsAlive)
            {
                ChangeState(_deadState);
            }
            else
            {
                ChangeState(_idleState);
            }

            SetEnabled(false);
        }

        public EnemyAiConfig Config { get { return _config; } }
        public IEnemyAiState IdleState { get { return _idleState; } }
        public IEnemyAiState AimAndShootState { get { return _aimAndShootState; } }
        public IEnemyAiState KeepDistanceState { get { return _keepDistanceState; } }
        public IEnemyAiState RepositionState { get { return _repositionState; } }
        public IEnemyAiState AvoidObstacleState { get { return _avoidObstacleState; } }
        public IEnemyAiState DeadState { get { return _deadState; } }
        public float RepositionTurnDirection { get { return _repositionTurnDirection; } }
        public float AvoidTurnDirection { get { return _avoidTurnDirection; } }

        public void Tick(float deltaTime)
        {
            if (_isDisposed || !_isEnabled)
            {
                return;
            }

            if (!IsEnemyAlive())
            {
                ChangeState(_deadState);
                SetEnabled(false);
                return;
            }

            UpdateTargetVelocity(deltaTime);
            _stateMachine.Tick(deltaTime);
        }

        public void SetEnabled(bool isEnabled)
        {
            if (!IsEnemyAlive())
            {
                _isEnabled = false;
                ChangeState(_deadState);
                StopTank();
                return;
            }

            _isEnabled = isEnabled;

            if (!_isEnabled)
            {
                StopTank();
                ResetShootReadiness();
            }
            else if (_stateMachine.CurrentState == _deadState)
            {
                ChangeState(_idleState);
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_enemy.Health != null)
            {
                _enemy.Health.Died -= OnEnemyDied;
            }

            if (_gameplayEvents != null)
            {
                _gameplayEvents.RoundStarted -= OnRoundStarted;
                _gameplayEvents.RoundFinished -= OnRoundFinished;
                _gameplayEvents.MatchFinished -= OnMatchFinished;
            }

            StopTank();
            _isDisposed = true;
        }

        public void ChangeState(IEnemyAiState nextState)
        {
            var previousState = _stateMachine.CurrentState;
            _stateMachine.ChangeState(nextState);

            if (_config.DebugLogs && previousState != _stateMachine.CurrentState && _stateMachine.CurrentState != null)
            {
                Debug.Log("[ENEMY_AI] state=" + _stateMachine.CurrentState.Name);
            }
        }

        public bool HasLiveTarget()
        {
            return _target != null && _target.Health != null && _target.Health.IsAlive;
        }

        public bool IsEnemyAlive()
        {
            return _enemy != null && _enemy.Health != null && _enemy.Health.IsAlive;
        }

        public bool IsTargetDetected()
        {
            return HasLiveTarget() && GetDistanceToTarget() <= _config.DetectionDistance;
        }

        public float GetDistanceToTarget()
        {
            if (_target == null)
            {
                return float.MaxValue;
            }

            return Vector3.Distance(GetPlanarPosition(_enemy.transform.position), GetPlanarPosition(_target.transform.position));
        }

        public void AimAtTarget()
        {
            if (!HasLiveTarget() || _enemy.Aiming == null)
            {
                return;
            }

            RefreshAimError();
            _enemy.Aiming.AimAt(GetCurrentAimPoint());
        }

        public void StopTank()
        {
            _enemy.Movement?.SetInput(0f, 0f);
        }

        public void DriveTowardTarget(float throttle)
        {
            DriveToward(GetDirectionToTarget(), throttle);
        }

        public void DriveWithTurn(float throttle, float turn)
        {
            _enemy.Movement?.SetInput(throttle, Mathf.Clamp(turn, -1f, 1f));
        }

        public void DriveToward(Vector3 desiredDirection, float throttleScale)
        {
            if (_enemy.Movement == null)
            {
                return;
            }

            desiredDirection.y = 0f;
            if (desiredDirection.sqrMagnitude <= 0.0001f)
            {
                StopTank();
                return;
            }

            var forward = Vector3.ProjectOnPlane(_enemy.transform.forward, Vector3.up);
            if (forward.sqrMagnitude <= 0.0001f)
            {
                forward = Vector3.forward;
            }

            var signedAngle = Vector3.SignedAngle(forward.normalized, desiredDirection.normalized, Vector3.up);
            var absoluteAngle = Mathf.Abs(signedAngle);
            var throttleFactor = Mathf.InverseLerp(MovementZeroThrottleAngle, MovementFullThrottleAngle, absoluteAngle);
            var throttle = Mathf.Clamp(throttleScale * throttleFactor, -1f, 1f);
            var turn = Mathf.Clamp(signedAngle / MovementFullTurnAngle, -1f, 1f);
            _enemy.Movement.SetInput(throttle, turn);
        }

        public bool ShouldReposition(float deltaTime)
        {
            _repositionTimer -= deltaTime;
            if (_repositionTimer > 0f)
            {
                return false;
            }

            ResetRepositionTimer();
            _repositionTurnDirection = _repositionTurnDirection >= 0f ? -1f : 1f;
            return true;
        }

        public void ResetRepositionTimer()
        {
            _repositionTimer = _config.RepositionInterval;
        }

        public void ChooseAvoidTurnDirection()
        {
            var probe = GetObstacleProbe();

            if (!probe.HasAnyObstacle)
            {
                _avoidTurnDirection = _repositionTurnDirection;
                return;
            }

            if (probe.LeftBlocked && !probe.RightBlocked)
            {
                _avoidTurnDirection = 1f;
                return;
            }

            if (probe.RightBlocked && !probe.LeftBlocked)
            {
                _avoidTurnDirection = -1f;
                return;
            }

            if (probe.CenterBlocked && !probe.LeftBlocked && !probe.RightBlocked)
            {
                ChooseTurnDirectionByTargetSide();
                return;
            }

            _avoidTurnDirection = _repositionTurnDirection >= 0f ? -1f : 1f;
        }

        public bool HasObstacleAhead()
        {
            var probe = GetObstacleProbe();
            return probe.HasAnyObstacle;
        }
        
        private ObstacleProbe GetObstacleProbe()
{
    var centerOrigin = _enemy.transform.position + Vector3.up * AimHeight;
    var forward = Vector3.ProjectOnPlane(_enemy.transform.forward, Vector3.up);

    if (forward.sqrMagnitude <= 0.0001f || _config.ObstacleRayDistance <= 0f)
    {
        return ObstacleProbe.Empty;
    }

    forward.Normalize();

    var right = Vector3.Cross(Vector3.up, forward).normalized;
    var sideOffset = _config.ObstacleSideRayOffset;

    var leftOrigin = centerOrigin - right * sideOffset;
    var rightOrigin = centerOrigin + right * sideOffset;

    var centerBlocked = HasObstacleFromOrigin(centerOrigin, forward);
    var leftBlocked = HasObstacleFromOrigin(leftOrigin, forward);
    var rightBlocked = HasObstacleFromOrigin(rightOrigin, forward);

    DrawObstacleRay(centerOrigin, forward, centerBlocked);
    DrawObstacleRay(leftOrigin, forward, leftBlocked);
    DrawObstacleRay(rightOrigin, forward, rightBlocked);

    return new ObstacleProbe(centerBlocked, leftBlocked, rightBlocked);
}

private bool HasObstacleFromOrigin(Vector3 origin, Vector3 direction)
{
    if (!TryFindClosestRelevantHit(origin, direction, _config.ObstacleRayDistance, _config.ObstacleMask, out var hit))
    {
        return false;
    }

    return !IsColliderOnTank(hit.collider, _target);
}

private void ChooseTurnDirectionByTargetSide()
{
    var directionToTarget = GetDirectionToTarget();
    var forward = Vector3.ProjectOnPlane(_enemy.transform.forward, Vector3.up);

    if (directionToTarget.sqrMagnitude <= 0.0001f || forward.sqrMagnitude <= 0.0001f)
    {
        _avoidTurnDirection = _repositionTurnDirection;
        return;
    }

    var targetSide = Vector3.SignedAngle(forward.normalized, directionToTarget.normalized, Vector3.up);
    _avoidTurnDirection = targetSide >= 0f ? 1f : -1f;
}

private void DrawObstacleRay(Vector3 origin, Vector3 direction, bool isBlocked)
{
    if (!_config.DebugLogs)
    {
        return;
    }

    var color = isBlocked ? Color.red : Color.green;
    Debug.DrawRay(origin, direction * _config.ObstacleRayDistance, color, Time.fixedDeltaTime);
}

        public bool TryShootTarget()
        {
            if (!CanShootTarget())
            {
                return false;
            }

            _nextShootRequestTime = Time.time + ShootRequestBaseInterval * _config.FireCooldownMultiplier;
            ResetShootReadiness();
            _enemy.Shooter?.TryShoot();

            if (_config.DebugLogs)
            {
                Debug.Log("[ENEMY_AI] shooting");
            }

            return true;
        }

        public bool CanShootTarget()
        {
            if (!HasLiveTarget() || _enemy.Shooter == null || Time.time < _nextShootRequestTime)
            {
                ResetShootReadiness();
                return false;
            }

            if (GetDistanceToTarget() > _config.FireDistance)
            {
                ResetShootReadiness();
                return false;
            }

            if (!HasLineOfSight())
            {
                ResetShootReadiness();
                return false;
            }

            if (!IsAimCloseEnough())
            {
                ResetShootReadiness();
                return false;
            }

            if (!_isShootReadinessStarted)
            {
                _isShootReadinessStarted = true;
                _shootReadyTime = Time.time + _config.ShootReactionDelay;
                return _config.ShootReactionDelay <= 0f;
            }

            return Time.time >= _shootReadyTime;
        }

        public bool HasLineOfSight()
        {
            if (!HasLiveTarget())
            {
                return false;
            }

            var origin = GetMuzzlePosition();
            var targetPoint = GetTargetAimPoint();
            var direction = targetPoint - origin;
            direction.y = 0f;

            var distance = direction.magnitude;
            if (distance <= 0.001f)
            {
                return true;
            }

            if (!TryFindClosestRelevantHit(origin, direction.normalized, distance, _config.LineOfSightMask, out var hit))
            {
                return true;
            }

            if (IsColliderOnTank(hit.collider, _target))
            {
                return true;
            }

            if (_config.DebugLogs)
            {
                Debug.Log("[ENEMY_AI] line of sight blocked by " + hit.collider.name);
            }

            return false;
        }

        private void UpdateTargetVelocity(float deltaTime)
        {
            if (!HasLiveTarget() || deltaTime <= 0f)
            {
                _hasTargetPositionSnapshot = false;
                _targetVelocity = Vector3.zero;
                return;
            }

            var currentTargetPosition = GetPlanarPosition(_target.transform.position);
            if (!_hasTargetPositionSnapshot)
            {
                _lastTargetPosition = currentTargetPosition;
                _targetVelocity = Vector3.zero;
                _hasTargetPositionSnapshot = true;
                return;
            }

            _targetVelocity = (currentTargetPosition - _lastTargetPosition) / deltaTime;
            _targetVelocity.y = 0f;
            _lastTargetPosition = currentTargetPosition;
        }

        private bool IsAimCloseEnough()
        {
            var aimForward = GetMuzzleForward();
            var direction = GetCurrentAimPoint() - GetMuzzlePosition();
            aimForward.y = 0f;
            direction.y = 0f;

            if (aimForward.sqrMagnitude <= 0.0001f || direction.sqrMagnitude <= 0.0001f)
            {
                return false;
            }

            return Vector3.Angle(aimForward.normalized, direction.normalized) <= _config.AimAngleToShoot;
        }

        private Vector3 GetMuzzlePosition()
        {
            return _enemy.Shooter != null && _enemy.Shooter.Muzzle != null
                ? _enemy.Shooter.Muzzle.position
                : _enemy.transform.position + Vector3.up * AimHeight;
        }

        private Vector3 GetMuzzleForward()
        {
            return _enemy.Shooter != null && _enemy.Shooter.Muzzle != null
                ? _enemy.Shooter.Muzzle.forward
                : _enemy.transform.forward;
        }

        private Vector3 GetCurrentAimPoint()
        {
            var muzzlePosition = GetMuzzlePosition();
            var targetPoint = GetPredictedTargetAimPoint();
            var direction = targetPoint - muzzlePosition;
            direction.y = 0f;

            if (direction.sqrMagnitude <= 0.0001f || _config.AimErrorAngle <= 0f)
            {
                return targetPoint;
            }

            var rotatedDirection = Quaternion.Euler(0f, _currentAimErrorAngle, 0f) * direction.normalized;
            var aimPoint = muzzlePosition + rotatedDirection * direction.magnitude;
            aimPoint.y = targetPoint.y;
            return aimPoint;
        }

        private Vector3 GetPredictedTargetAimPoint()
        {
            return GetTargetAimPoint() + _targetVelocity * _config.AimPredictionStrength;
        }

        private Vector3 GetTargetAimPoint()
        {
            return _target.transform.position + Vector3.up * AimHeight;
        }

        private Vector3 GetDirectionToTarget()
        {
            if (_target == null)
            {
                return Vector3.zero;
            }

            var direction = _target.transform.position - _enemy.transform.position;
            direction.y = 0f;
            return direction;
        }

        private void RefreshAimError()
        {
            if (_config.AimErrorAngle <= 0f)
            {
                _currentAimErrorAngle = 0f;
                return;
            }

            if (Time.time < _nextAimErrorChangeTime)
            {
                return;
            }

            _currentAimErrorAngle = UnityEngine.Random.Range(-_config.AimErrorAngle, _config.AimErrorAngle);
            _nextAimErrorChangeTime = Time.time + _config.AimErrorChangeInterval;
        }

        private void ResetAimError()
        {
            _currentAimErrorAngle = 0f;
            _nextAimErrorChangeTime = 0f;
        }

        private void ResetShootReadiness()
        {
            _isShootReadinessStarted = false;
            _shootReadyTime = 0f;
        }

        private bool TryFindClosestRelevantHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit closestHit)
        {
            closestHit = default;
            var hasHit = false;
            var closestDistance = float.MaxValue;
            var hitCount = Physics.RaycastNonAlloc(origin, direction, _raycastHits, distance, mask, QueryTriggerInteraction.Ignore);

            for (var index = 0; index < hitCount; index++)
            {
                var hit = _raycastHits[index];
                if (hit.collider == null || IsColliderOnTank(hit.collider, _enemy))
                {
                    continue;
                }

                if (hit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hit.distance;
                closestHit = hit;
                hasHit = true;
            }

            return hasHit;
        }

        private static bool IsColliderOnTank(Collider collider, TankFacade tank)
        {
            return collider != null && tank != null && collider.GetComponentInParent<TankFacade>() == tank;
        }

        private static Vector3 GetPlanarPosition(Vector3 position)
        {
            position.y = 0f;
            return position;
        }

        private void OnEnemyDied(TankHealth health)
        {
            ChangeState(_deadState);
            SetEnabled(false);
        }

        private void OnRoundStarted()
        {
            ResetRepositionTimer();
            ResetShootReadiness();
            ResetAimError();
            _hasTargetPositionSnapshot = false;
            _targetVelocity = Vector3.zero;
            ChangeState(_idleState);
            SetEnabled(true);
        }

        private void OnRoundFinished(RoundFinishedEvent round)
        {
            SetEnabled(false);
        }

        private void OnMatchFinished(MatchFinishedEvent match)
        {
            SetEnabled(false);
        }
        
        private struct ObstacleProbe
        {
            public static readonly ObstacleProbe Empty = new ObstacleProbe(false, false, false);

            public ObstacleProbe(bool centerBlocked, bool leftBlocked, bool rightBlocked)
            {
                CenterBlocked = centerBlocked;
                LeftBlocked = leftBlocked;
                RightBlocked = rightBlocked;
            }

            public bool CenterBlocked { get; }
            public bool LeftBlocked { get; }
            public bool RightBlocked { get; }

            public bool HasAnyObstacle
            {
                get { return CenterBlocked || LeftBlocked || RightBlocked; }
            }
        }
    }
}