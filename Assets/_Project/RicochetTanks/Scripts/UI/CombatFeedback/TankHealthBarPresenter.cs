using System;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.UI.Core;

namespace RicochetTanks.UI.CombatFeedback
{
    public sealed class TankHealthBarPresenter : IPresenter
    {
        private readonly TankHealth _health;
        private readonly TankHealthBarView _view;
        private bool _isDisposed;

        public TankHealthBarPresenter(TankHealthBarView view, TankHealth health)
        {
            _view = view;
            _health = health;

            if (_health == null || _view == null)
            {
                return;
            }

            _health.HealthChanged += OnHealthChanged;
            OnHealthChanged(_health.CurrentHp, _health.MaxHp);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_health != null)
            {
                _health.HealthChanged -= OnHealthChanged;
            }

            _isDisposed = true;
        }

        private void OnHealthChanged(float currentHp, float maxHp)
        {
            _view.SetHealth(currentHp, maxHp);
        }
    }
}
