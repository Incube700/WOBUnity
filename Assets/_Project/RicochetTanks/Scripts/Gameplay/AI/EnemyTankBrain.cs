using System;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.Gameplay.AI.States;
using RicochetTanks.Gameplay.Combat;
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

        private float _repositionTimer;
        private float _nextShootRequestTime;
        private float _repositionTurnDirection = 1f;
        private float _avoidTurnDirection = 1f;
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

            _enemy.Aiming.AimAt(GetTargetAimPoint());
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
            var directionToTarget = GetDirectionToTarget();
            var forward = Vector3.ProjectOnPlane(_enemy.transform.forward, Vector3.up);
            if (directionToTarget.sqrMagnitude <= 0.0001f || forward.sqrMagnitude <= 0.0001f)
            {
                _avoidTurnDirection = _repositionTurnDirection;
                return;
            }

            var side = Vector3.SignedAngle(forward.normalized, directionToTarget.normalized, Vector3.up);
            _avoidTurnDirection = side >= 0f ? -1f : 1f;
        }

        public bool HasObstacleAhead()
        {
            var origin = _enemy.transform.position + Vector3.up * AimHeight;
            var direction = Vector3.ProjectOnPlane(_enemy.transform.forward, Vector3.up);
            if (direction.sqrMagnitude <= 0.0001f || _config.ObstacleRayDistance <= 0f)
            {
                return false;
            }

            var hit = FindClosestRelevantHit(origin, direction.normalized, _config.ObstacleRayDistance, _config.ObstacleMask);
            if (hit == null)
            {
                return false;
            }

            return !IsColliderOnTank(hit.Collider, _target);
        }

        public bool TryShootTarget()
        {
            if (!CanShootTarget())
            {
                return false;
            }

            _nextShootRequestTime = Time.time + ShootRequestBaseInterval * _config.FireCooldownMultiplier;
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
                return false;
            }

            if (GetDistanceToTarget() > _config.FireDistance)
            {
                return false;
            }

            if (!HasLineOfSight())
            {
                return false;
            }

            return IsAimCloseEnough();
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

            var hit = FindClosestRelevantHit(origin, direction.normalized, distance, _config.LineOfSightMask);
            if (hit == null || IsColliderOnTank(hit.Collider, _target))
            {
                return true;
            }

            if (_config.DebugLogs)
            {
                Debug.Log("[ENEMY_AI] line of sight blocked by " + hit.Collider.name);
            }

            return false;
        }

        private bool IsAimCloseEnough()
        {
            var aimForward = GetMuzzleForward();
            var direction = GetTargetAimPoint() - GetMuzzlePosition();
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

        private RaycastInfo FindClosestRelevantHit(Vector3 origin, Vector3 direction, float distance, LayerMask mask)
        {
            var hits = Physics.RaycastAll(origin, direction, distance, mask, QueryTriggerInteraction.Ignore);
            RaycastInfo closestHit = null;

            for (var index = 0; index < hits.Length; index++)
            {
                var hit = hits[index];
                if (hit.collider == null || IsColliderOnTank(hit.collider, _enemy))
                {
                    continue;
                }

                if (closestHit == null || hit.distance < closestHit.Distance)
                {
                    closestHit = new RaycastInfo(hit.collider, hit.distance);
                }
            }

            return closestHit;
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

        private sealed class RaycastInfo
        {
            public RaycastInfo(Collider collider, float distance)
            {
                Collider = collider;
                Distance = distance;
            }

            public Collider Collider { get; }
            public float Distance { get; }
        }
    }
}
