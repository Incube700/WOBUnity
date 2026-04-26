using System;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public class TankHealth : MonoBehaviour
    {
        [SerializeField] private int _maxHp = 100;

        public event Action<TankHealth> Died;
        public event Action<int, int> HealthChanged;

        public int CurrentHp { get; private set; }
        public int MaxHp => _maxHp;
        public bool IsAlive => CurrentHp > 0;

        private void Awake()
        {
            ResetHealth();
        }

        public void Configure(int maxHp)
        {
            _maxHp = Mathf.Max(1, maxHp);
            ResetHealth();
        }

        public void ResetHealth()
        {
            CurrentHp = _maxHp;
            HealthChanged?.Invoke(CurrentHp, _maxHp);
        }

        public void ApplyDamage(int damage)
        {
            if (CurrentHp <= 0)
            {
                return;
            }

            CurrentHp = Mathf.Max(0, CurrentHp - damage);
            HealthChanged?.Invoke(CurrentHp, _maxHp);

            if (CurrentHp == 0)
            {
                Died?.Invoke(this);
                gameObject.SetActive(false);
            }
        }
    }
}
