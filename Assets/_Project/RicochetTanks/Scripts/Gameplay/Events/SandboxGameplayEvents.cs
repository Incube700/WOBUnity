using System;
using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public sealed class SandboxGameplayEvents
    {
        public event Action<ProjectileSpawnedEvent> ProjectileSpawned;
        public event Action<ProjectileHitEvent> ProjectileHit;
        public event Action<ProjectileBouncedEvent> ProjectileBounced;
        public event Action<HitResolvedEvent> HitResolved;
        public event Action<CombatFeedbackEvent> CombatFeedbackRequested;
        public event Action MatchStarted;
        public event Action RoundStarted;
        public event Action<RoundFinishedEvent> RoundFinished;
        public event Action<SessionScoreEvent> SessionScoreChanged;
        public event Action<string> SessionStatusChanged;
        public event Action<MatchFinishedEvent> MatchFinished;
        public event Action RestartRequested;

        private readonly DebugLogConfig _debugLogConfig;

        public SandboxGameplayEvents(DebugLogConfig debugLogConfig = null)
        {
            _debugLogConfig = debugLogConfig;
        }

        public bool ShouldLogShots => _debugLogConfig != null && _debugLogConfig.LogShots;
        public bool ShouldLogHits => _debugLogConfig != null && _debugLogConfig.LogHits;
        public bool ShouldLogBounces => _debugLogConfig != null && _debugLogConfig.LogBounces;
        public bool ShouldLogRounds => _debugLogConfig == null || _debugLogConfig.LogRounds;

        public void RaiseProjectileSpawned(Projectile projectile, TankFacade owner, Vector3 position, Vector3 direction, float speed, float damage, int bouncesLeft)
        {
            ProjectileSpawned?.Invoke(new ProjectileSpawnedEvent(projectile, owner, position, direction, speed, damage, bouncesLeft));
        }

        public void RaiseProjectileHit(Projectile projectile, Collider collider, Vector3 point, Vector3 normal, Vector3 direction)
        {
            ProjectileHit?.Invoke(new ProjectileHitEvent(projectile, collider, point, normal, direction));
        }

        public void RaiseProjectileBounced(Projectile projectile, TankFacade owner, int ricochetCount, int bouncesLeft, float speed, float damage, Vector3 normal)
        {
            ProjectileBounced?.Invoke(new ProjectileBouncedEvent(projectile, owner, ricochetCount, bouncesLeft, speed, damage, normal));
        }

        public void RaiseHitResolved(HitResolvedEvent hit)
        {
            HitResolved?.Invoke(hit);
        }

        public void RaiseCombatFeedbackRequested(
            Vector3 worldPoint,
            Vector3 worldNormal,
            TankFacade source,
            TankFacade target,
            HitResult result,
            float damage,
            float currentHp,
            float maxHp,
            ArmorHitInfo armorHit)
        {
            var feedback = new CombatFeedbackEvent(
                worldPoint,
                worldNormal,
                source,
                target,
                result,
                damage,
                currentHp,
                maxHp,
                armorHit);

            var handlers = CombatFeedbackRequested;
            if (handlers == null)
            {
                return;
            }

            foreach (Action<CombatFeedbackEvent> handler in handlers.GetInvocationList())
            {
                try
                {
                    handler(feedback);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        public void RaiseMatchStarted()
        {
            MatchStarted?.Invoke();
        }

        public void RaiseRoundStarted()
        {
            RoundStarted?.Invoke();
        }

        public void RaiseRoundFinished(RoundFinishedEvent round)
        {
            RoundFinished?.Invoke(round);
        }

        public void RaiseSessionScoreChanged(SessionScoreEvent score)
        {
            SessionScoreChanged?.Invoke(score);
        }

        public void RaiseSessionStatusChanged(string status)
        {
            SessionStatusChanged?.Invoke(status);
        }

        public void RaiseMatchFinished(MatchResult result, string label)
        {
            MatchFinished?.Invoke(new MatchFinishedEvent(result, label));
        }

        public void RaiseRestartRequested()
        {
            RestartRequested?.Invoke();
        }
    }
}
