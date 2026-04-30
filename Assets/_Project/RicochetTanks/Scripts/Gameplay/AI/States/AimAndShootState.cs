namespace RicochetTanks.Gameplay.AI.States
{
    public sealed class AimAndShootState : IEnemyAiState
    {
        private readonly EnemyTankBrain _brain;

        public AimAndShootState(EnemyTankBrain brain)
        {
            _brain = brain;
        }

        public string Name { get { return "AimAndShoot"; } }

        public void Enter()
        {
            _brain.StopTank();
        }

        public void Tick(float deltaTime)
        {
            if (!_brain.IsTargetDetected())
            {
                _brain.ChangeState(_brain.IdleState);
                return;
            }

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

            if (!_brain.HasLineOfSight())
            {
                _brain.ChangeState(_brain.RepositionState);
                return;
            }

            _brain.TryShootTarget();

            if (_brain.GetDistanceToTarget() > _brain.Config.MaxDistance || _brain.ShouldReposition(deltaTime))
            {
                _brain.ChangeState(_brain.RepositionState);
            }
            else
            {
                _brain.StopTank();
            }
        }

        public void Exit()
        {
        }
    }
}
