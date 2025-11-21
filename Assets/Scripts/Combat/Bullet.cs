using UnityEngine;

/// <summary>
/// Простая пуля с ручным подсчётом рикошетов и угла брони.
/// </summary>
[RequireComponent(typeof(Rigidbody))] // гарантируем наличие Rigidbody
public class Bullet : MonoBehaviour
{
    [Header("Параметры полёта")]
    [SerializeField] private float speed = 12f;           // скорость пули (м/с)
    [SerializeField] private float damage = 40f;          // урон при пробитии
    [SerializeField] private int maxCollisions = 4;       // исчезает после 4-го столкновения

    private Rigidbody rb;                                 // кэш Rigidbody
    private int collisionCount = 0;                       // сколько столкновений уже было

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();                   // получаем Rigidbody
        rb.useGravity = false;                            // топ-даун — без гравитации
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // точные столкновения
        // Lock Y axis to keep it flat
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.linearVelocity = transform.forward * speed;          // стартовая скорость вдоль forward (Z)
    }

    private void FixedUpdate()
    {
        // Keep velocity constant and flat
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionCount++;                                 // считаем столкновения
        Vector3 normal = collision.GetContact(0).normal;  // нормаль поверхности
        float angle = MathAngles.ImpactAngle(rb.linearVelocity, normal); // угол между пулей и нормалью

        if (angle > 45f)                                  // угол больше 45° — наносим урон
        {
            DamageReceiver receiver = collision.collider.GetComponentInParent<DamageReceiver>(); // ищем получателя урона
            if (receiver != null) receiver.ApplyDamage(damage); // передаём урон
            
            SpawnExplosion(collision.GetContact(0).point); // создаём взрыв
            Destroy(gameObject);                          // удаляем пулю после попадания
            return;                                       // дальнейшая обработка не нужна
        }

        if (collisionCount >= maxCollisions)              // достигнут лимит столкновений
        {
            Destroy(gameObject);                          // уничтожаем пулю
            return;                                       // прекращаем обработку
        }

        Vector3 reflected = Vector3.Reflect(rb.linearVelocity.normalized, normal); // отражаем направление
        // Flatten reflection just in case
        reflected.y = 0f;
        rb.linearVelocity = reflected.normalized * speed;       // задаём новую скорость после рикошета
    }

    private void SpawnExplosion(Vector3 position)
    {
        // Create a new GameObject for the explosion
        GameObject explosionObj = new GameObject("Explosion");
        explosionObj.transform.position = position;
        explosionObj.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Lie flat on ground (XZ)
        
        // Add SpriteRenderer and ExplosionVFX
        explosionObj.AddComponent<SpriteRenderer>();
        explosionObj.AddComponent<ExplosionVFX>();
    }
}
