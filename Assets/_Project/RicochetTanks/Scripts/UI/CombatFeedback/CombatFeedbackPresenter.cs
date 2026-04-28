using System;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Events;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    public sealed class CombatFeedbackPresenter : IDisposable
    {
        private const float NormalOffset = 0.18f;
        private const float VerticalOffset = 0.22f;
        private const float Lifetime = 1f;

        private readonly SandboxGameplayEvents _gameplayEvents;
        private readonly CombatFeedbackFactory _factory;
        private bool _isDisposed;

        public CombatFeedbackPresenter(SandboxGameplayEvents gameplayEvents, CombatFeedbackFactory factory)
        {
            _gameplayEvents = gameplayEvents;
            _factory = factory;

            if (_gameplayEvents != null)
            {
                _gameplayEvents.CombatFeedbackRequested += OnCombatFeedbackRequested;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_gameplayEvents != null)
            {
                _gameplayEvents.CombatFeedbackRequested -= OnCombatFeedbackRequested;
            }

            _isDisposed = true;
        }

        private void OnCombatFeedbackRequested(CombatFeedbackEvent feedback)
        {
            if (_factory == null)
            {
                return;
            }

            var spawnPosition = feedback.WorldPoint
                + ResolveNormal(feedback.WorldNormal) * NormalOffset
                + Vector3.up * VerticalOffset;

            var view = _factory.CreateFloatingHitText(spawnPosition);
            if (view == null)
            {
                return;
            }

            view.Play(ResolveText(feedback), ResolveColor(feedback), Lifetime);
        }

        private static Vector3 ResolveNormal(Vector3 normal)
        {
            return normal.sqrMagnitude < 0.001f ? Vector3.zero : normal.normalized;
        }

        private static string ResolveText(CombatFeedbackEvent feedback)
        {
            if (feedback.Result == HitResult.Ricochet)
            {
                return "RICOCHET";
            }

            if (feedback.Result == HitResult.NoPen)
            {
                return "NO PEN";
            }

            if (feedback.Damage > 0f)
            {
                return $"-{Format(feedback.Damage)}";
            }

            return feedback.Result.ToString().ToUpperInvariant();
        }

        private static Color ResolveColor(CombatFeedbackEvent feedback)
        {
            if (feedback.Result == HitResult.Ricochet)
            {
                return new Color(0.25f, 0.85f, 1f);
            }

            if (feedback.Result == HitResult.NoPen)
            {
                return new Color(0.82f, 0.86f, 0.88f);
            }

            return new Color(1f, 0.34f, 0.18f);
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
