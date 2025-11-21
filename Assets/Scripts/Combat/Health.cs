using UnityEngine;

public class Health : DamageReceiver
{
    [SerializeField] private float maxHealth = 5f; // начальное здоровье
    private float _current; // текущее здоровье

    private void Awake()
    {
        _current = maxHealth; // устанавливаем здоровье при запуске
    }

    public override void ApplyDamage(float amount)
    {
        _current -= amount; // уменьшаем здоровье на величину урона
        if (_current <= 0f) // проверяем, не умер ли объект
        {
            Destroy(gameObject); // уничтожаем объект при смерти
        }
    }
}

