using RicochetTanks.Gameplay.Combat;
using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    [DisallowMultipleComponent]
    public sealed class TankDeathHandler : MonoBehaviour
    {
        private TankFacade _tank;
        private TankHealth _health;
        private bool _isSubscribed;

        public void Configure(TankFacade tank, TankHealth health)
        {
            Unsubscribe();

            _tank = tank;
            _health = health;

            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed || _health == null)
            {
                return;
            }

            _health.Died += OnDied;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _health == null)
            {
                return;
            }

            _health.Died -= OnDied;
            _isSubscribed = false;
        }

        private void OnDied(TankHealth health)
        {
            _tank?.SetGameplayEnabled(false);
        }
    }
}