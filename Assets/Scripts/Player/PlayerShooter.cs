using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Простой скрипт стрельбы игрока.
/// </summary>
public class PlayerShooter : MonoBehaviour
{
    [FormerlySerializedAs("bullet2DPrefab")]
    [SerializeField] private GameObject bulletPrefab;   // префаб пули
    [SerializeField] private Transform muzzle;          // точка вылета
    [SerializeField] private float fireCooldown = 0.25f;// задержка между выстрелами
    [SerializeField] private bool useMuzzleUp = true; // ориентировать пулю по оси Up (иначе Right)

    private float cooldown;                             // таймер перезарядки

    private void Awake()
    {
        if (bulletPrefab == null)                       // если префаб не назначен в инспекторе
            bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet"); // пробуем найти в Resources
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;                     // уменьшаем таймер

        bool wantShoot = false;                         // флаг желания стрелять
        if (GameInput.Instance != null)                 // если есть централизованный ввод
            wantShoot = GameInput.Instance.Fire || GameInput.Instance.FireHeld; // читаем действие Fire
        else
            wantShoot = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space); // резервный ввод

        if (wantShoot && cooldown <= 0f)                // можно стрелять
        {
            Shoot();                                    // создаём пулю
            cooldown = fireCooldown;                    // сбрасываем таймер
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || muzzle == null)     // проверяем необходимые ссылки
        {
            Debug.LogError("PlayerShooter: назначьте bulletPrefab и muzzle", this); // выводим ошибку
            return;                                     // прекращаем выполнение
        }

        // Пуля должна лететь вдоль дула.
        // Если спрайт/трансформ дула ориентирован «вправо» — берём muzzle.right,
        // иначе берём muzzle.up. Поворот подбираем так, чтобы локальная up пули
        // совпала с выбранным направлением (Bullet летит вдоль transform.up).
        Vector3 dir = useMuzzleUp ? muzzle.up : muzzle.right;
        Vector3 spawnPos = muzzle.position + dir * 0.2f; // небольшой вынос от дула
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, dir);
        Instantiate(bulletPrefab, spawnPos, rot);
    }
}
