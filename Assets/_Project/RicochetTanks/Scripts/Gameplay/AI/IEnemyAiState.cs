namespace RicochetTanks.Gameplay.AI
{
    public interface IEnemyAiState
    {
        string Name { get; }
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
