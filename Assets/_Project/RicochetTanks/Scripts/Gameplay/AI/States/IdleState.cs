namespace RicochetTanks.Gameplay.AI.States
{
    public sealed class IdleState : IEnemyAiState
    {
        private readonly EnemyTankBrain _brain;

        public IdleState(EnemyTankBrain brain)
        {
            _brain = brain;
        }

        public string Name { get { return "Idle"; } }

        public void Enter()
        {
            _brain.StopTank();
        }

        public void Tick(float deltaTime)
        {
            if (!_brain.IsEnemyAlive())
            {
                _brain.ChangeState(_brain.DeadState);
                return;
            }

            if (!_brain.IsTargetDetected())
            {
                _brain.StopTank();
                return;
            }

            _brain.ChangeState(
                _brain.GetDistanceToTarget() <= _brain.Config.MinDistance
                    ? _brain.KeepDistanceState
                    : _brain.AimAndShootState);
        }

        public void Exit()
        {
        }
    }
}
