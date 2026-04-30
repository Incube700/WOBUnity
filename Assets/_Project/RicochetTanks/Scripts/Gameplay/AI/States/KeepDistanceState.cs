namespace RicochetTanks.Gameplay.AI.States
{
    public sealed class KeepDistanceState : IEnemyAiState
    {
        private readonly EnemyTankBrain _brain;

        public KeepDistanceState(EnemyTankBrain brain)
        {
            _brain = brain;
        }

        public string Name { get { return "KeepDistance"; } }

        public void Enter()
        {
        }

        public void Tick(float deltaTime)
        {
            if (!_brain.IsTargetDetected())
            {
                _brain.ChangeState(_brain.IdleState);
                return;
            }

            _brain.AimAtTarget();
            _brain.TryShootTarget();

            if (_brain.HasObstacleAhead())
            {
                _brain.ChangeState(_brain.AvoidObstacleState);
                return;
            }

            if (_brain.GetDistanceToTarget() >= _brain.Config.PreferredDistance)
            {
                _brain.ChangeState(_brain.AimAndShootState);
                return;
            }

            _brain.DriveTowardTarget(_brain.Config.RetreatThrottle);
        }

        public void Exit()
        {
            _brain.StopTank();
        }
    }
}
