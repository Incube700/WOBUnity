namespace RicochetTanks.Gameplay.AI.States
{
    public sealed class DeadState : IEnemyAiState
    {
        private readonly EnemyTankBrain _brain;

        public DeadState(EnemyTankBrain brain)
        {
            _brain = brain;
        }

        public string Name { get { return "Dead"; } }

        public void Enter()
        {
            _brain.StopTank();
        }

        public void Tick(float deltaTime)
        {
            _brain.StopTank();
        }

        public void Exit()
        {
        }
    }
}
