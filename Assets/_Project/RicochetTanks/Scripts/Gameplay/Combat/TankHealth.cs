using System;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public class TankHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHp = 100f;

        public event Action<TankHealth> Died;
        public event Action<float, float> HealthChanged;

        public float CurrentHp { get; private set; }
        public float MaxHp => _maxHp;
        public bool IsAlive => CurrentHp > 0;

        private void Awake()
        {
            ResetHealth();
        }

        public void Configure(float maxHp)
        {
            _maxHp = Mathf.Max(1, maxHp);
            ResetHealth();
        }

        public void ResetHealth()
        {
            CurrentHp = _maxHp;
            HealthChanged?.Invoke(CurrentHp, _maxHp);
        }

        public void ApplyDamage(float damage)
        {
            if (CurrentHp <= 0)
            {
                return;
            }

            CurrentHp = Mathf.Max(0f, CurrentHp - Mathf.Max(0f, damage));
            HealthChanged?.Invoke(CurrentHp, _maxHp);

            if (CurrentHp == 0)
            {
                Died?.Invoke(this);
                gameObject.SetActive(false);
            }
        }
    }
}
