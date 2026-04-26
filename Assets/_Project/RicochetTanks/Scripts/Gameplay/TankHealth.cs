using System;
using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class TankHealth : MonoBehaviour
    {
        [SerializeField] private int _maxHp = 100;

        public event Action<TankHealth> Died;
        public event Action<int, int> HealthChanged;

        public int CurrentHp { get; private set; }

        private void Awake()
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
