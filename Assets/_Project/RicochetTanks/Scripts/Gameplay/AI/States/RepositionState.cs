namespace RicochetTanks.Gameplay.AI.States
{
    public sealed class RepositionState : IEnemyAiState
    {
        private readonly EnemyTankBrain _brain;
        private float _remainingTime;

        public RepositionState(EnemyTankBrain brain)
        {
            _brain = brain;
        }

        public string Name { get { return "Reposition"; } }

        public void Enter()
        {
            _remainingTime = _brain.Config.RepositionDuration;
        }

        public void Tick(float deltaTime)
        {
            if (!_brain.IsTargetDetected())
            {
                _brain.ChangeState(_brain.IdleState);
                return;
            }

            _remainingTime -= deltaTime;
            _brain.AimAtTarget();

            if (_brain.GetDistanceToTarget() <= _brain.Config.MinDistance)
            {
                _brain.ChangeState(_brain.KeepDistanceState);
                return;
            }

            if (_brain.HasObstacleAhead())
            {
                _brain.ChangeState(_brain.AvoidObstacleState);
                return;
            }

            _brain.TryShootTarget();
            _brain.DriveWithTurn(_brain.Config.MoveThrottle, _brain.RepositionTurnDirection * _brain.Config.RepositionTurn);

            if (_remainingTime <= 0f)
            {
                _brain.ChangeState(_brain.AimAndShootState);
            }
        }

        public void Exit()
        {
            _brain.StopTank();
        }
    }
}