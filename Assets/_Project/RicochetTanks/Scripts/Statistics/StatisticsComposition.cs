using System;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Match;
using RicochetTanks.Gameplay.Tanks;

namespace RicochetTanks.Statistics
{
    public sealed class StatisticsComposition : IDisposable
    {
        private readonly StatisticsTracker _tracker;

        public StatisticsComposition(
            SandboxGameplayEvents gameplayEvents,
            TankFacade player,
            TankFacade enemy,
            LocalMatchSessionService sessionService)
        {
            Repository = new PlayerStatisticsRepository();
            Data = Repository.Load();
            _tracker = new StatisticsTracker(gameplayEvents, player, enemy, sessionService, Repository);
        }

        public PlayerStatisticsRepository Repository { get; }
        public PlayerStatisticsData Data { get; }

        public void Dispose()
        {
            _tracker.Dispose();
        }
    }
}
