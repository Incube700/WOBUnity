using System;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Match;
using RicochetTanks.Gameplay.Tanks;

namespace RicochetTanks.Statistics
{
    public sealed class StatisticsTracker : IDisposable
    {
        private readonly SandboxGameplayEvents _gameplayEvents;
        private readonly TankFacade _player;
        private readonly TankFacade _enemy;
        private readonly LocalMatchSessionService _sessionService;
        private readonly PlayerStatisticsRepository _repository;

        public StatisticsTracker(
            SandboxGameplayEvents gameplayEvents,
            TankFacade player,
            TankFacade enemy,
            LocalMatchSessionService sessionService,
            PlayerStatisticsRepository repository)
        {
            _gameplayEvents = gameplayEvents ?? throw new ArgumentNullException(nameof(gameplayEvents));
            _player = player;
            _enemy = enemy;
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _gameplayEvents.MatchStarted += OnMatchStarted;
            _gameplayEvents.ProjectileSpawned += OnProjectileSpawned;
            _gameplayEvents.ProjectileBounced += OnProjectileBounced;
            _gameplayEvents.HitResolved += OnHitResolved;
            _gameplayEvents.MatchFinished += OnMatchFinished;
        }

        public void Dispose()
        {
            _gameplayEvents.MatchStarted -= OnMatchStarted;
            _gameplayEvents.ProjectileSpawned -= OnProjectileSpawned;
            _gameplayEvents.ProjectileBounced -= OnProjectileBounced;
            _gameplayEvents.HitResolved -= OnHitResolved;
            _gameplayEvents.MatchFinished -= OnMatchFinished;
        }

        private void OnMatchStarted()
        {
            _sessionService.ResetStatistics();
        }

        private void OnProjectileSpawned(ProjectileSpawnedEvent projectile)
        {
            if (projectile.Owner == _player)
            {
                _sessionService.CurrentStatistics.ShotsFired++;
            }
        }

        private void OnProjectileBounced(ProjectileBouncedEvent projectileBounce)
        {
            if (projectileBounce.Owner == _player)
            {
                _sessionService.CurrentStatistics.Ricochets++;
            }
        }

        private void OnHitResolved(HitResolvedEvent hit)
        {
            var stats = _sessionService.CurrentStatistics;

            if (hit.Source == _player && hit.Target == _player)
            {
                stats.SelfHits++;
            }

            if (hit.Source == _player && hit.Target == _enemy)
            {
                stats.TankHits++;
                stats.DamageDealt += hit.Damage;
                CountHitResult(stats, hit.Result);
            }
            else if (hit.Source == _enemy && hit.Target == _player)
            {
                stats.DamageTaken += hit.Damage;
            }
        }

        private void OnMatchFinished(MatchFinishedEvent match)
        {
            if (!_sessionService.TryMarkStatisticsSaved())
            {
                return;
            }

            var data = _repository.Load();
            var stats = _sessionService.CurrentStatistics;
            stats.MatchResult = match.Result;
            data.ApplyMatch(stats, _sessionService.GetScoreLabel());
            _repository.Save(data);
        }

        private static void CountHitResult(MatchStatistics stats, HitResult result)
        {
            if (result == HitResult.Penetrated || result == HitResult.ReducedDamage)
            {
                stats.Penetrations++;
            }
            else if (result == HitResult.NoPen)
            {
                stats.NoPenetrations++;
            }
            else if (result == HitResult.Ricochet || result == HitResult.WallRicochet)
            {
                stats.Ricochets++;
            }
        }
    }
}
