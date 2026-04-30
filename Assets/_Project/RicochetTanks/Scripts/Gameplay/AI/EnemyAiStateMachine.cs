namespace RicochetTanks.Gameplay.AI
{
    public sealed class EnemyAiStateMachine
    {
        public IEnemyAiState CurrentState { get; private set; }

        public void ChangeState(IEnemyAiState nextState)
        {
            if (nextState == null || CurrentState == nextState)
            {
                return;
            }

            CurrentState?.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }

        public void Tick(float deltaTime)
        {
            CurrentState?.Tick(deltaTime);
        }
    }
}
