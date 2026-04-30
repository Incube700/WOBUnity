namespace RicochetTanks.Gameplay.AI.States
{
    public sealed class AvoidObstacleState : IEnemyAiState
    {
        private readonly EnemyTankBrain _brain;
        private float _remainingTime;

        public AvoidObstacleState(EnemyTankBrain brain)
        {
            _brain = brain;
        }

        public string Name { get { return "AvoidObstacle"; } }

        public void Enter()
        {
            _remainingTime = _brain.Config.RepositionDuration;
            _brain.ChooseAvoidTurnDirection();
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
            _brain.DriveWithTurn(
                _brain.Config.RetreatThrottle,
                _brain.Config.ObstacleAvoidTurn * _brain.AvoidTurnDirection);

            if (_remainingTime <= 0f || !_brain.HasObstacleAhead())
            {
                _brain.ChangeState(_brain.RepositionState);
            }
        }

        public void Exit()
        {
            _brain.StopTank();
        }
    }
}
