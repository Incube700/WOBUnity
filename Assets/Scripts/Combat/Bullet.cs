using UnityEngine;

/// <summary>
/// Простая пуля с ручным подсчётом рикошетов и угла брони.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // гарантируем наличие Rigidbody2D
public class Bullet : MonoBehaviour
{
    [Header("Параметры полёта")]
    [SerializeField] private float speed = 12f;           // скорость пули (м/с)
    [SerializeField] private float damage = 40f;          // урон при пробитии
    [SerializeField] private int maxCollisions = 4;       // исчезает после 4-го столкновения

    private Rigidbody2D rb;                               // кэш Rigidbody2D
    private int collisionCount = 0;                       // сколько столкновений уже было

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();                 // получаем Rigidbody2D
        rb.gravityScale = 0f;                             // топ-даун — без гравитации
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // точные столкновения
        rb.linearVelocity = transform.up.normalized * speed;    // стартовая скорость вдоль up
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = rb.linearVelocity.normalized * speed;     // поддерживаем постоянную скорость
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionCount++;                                 // считаем столкновения
        Vector2 normal = collision.GetContact(0).normal;  // нормаль поверхности
        float angle = MathAngles.ImpactAngle(rb.linearVelocity, normal); // угол между пулей и нормалью

        if (angle > 45f)                                  // угол больше 45° — наносим урон
        {
            DamageReceiver2D receiver = collision.collider.GetComponentInParent<DamageReceiver2D>(); // ищем получателя урона
            if (receiver != null) receiver.ApplyDamage(damage); // передаём урон
            Destroy(gameObject);                          // удаляем пулю после попадания
            return;                                       // дальнейшая обработка не нужна
        }

        if (collisionCount >= maxCollisions)              // достигнут лимит столкновений
        {
            Destroy(gameObject);                          // уничтожаем пулю
            return;                                       // прекращаем обработку
        }

        Vector2 reflected = Vector2.Reflect(rb.linearVelocity.normalized, normal); // отражаем направление
        rb.linearVelocity = reflected * speed;                  // задаём новую скорость после рикошета
    }
}
