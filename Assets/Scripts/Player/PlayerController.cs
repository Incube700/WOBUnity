using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : DamageReceiver
{
    [SerializeField] private LayerMask enemyLayer; // Слой врагов

    private TankMover _mover;
    private TurretController _turret;
    private WeaponController _weapon;
    private float _nextContactDamageTime; // таймер урона от контакта
    private Plane _groundPlane; // плоскость для рейкаста мыши

    private void Awake()
    {
        _mover = GetComponent<TankMover>();
        _turret = GetComponent<TurretController>();
        _weapon = GetComponent<WeaponController>();
        
        // Ensure components exist (optional, or use RequireComponent)
        if (_mover == null) _mover = gameObject.AddComponent<TankMover>();
        if (_turret == null) _turret = gameObject.AddComponent<TurretController>();
        if (_weapon == null) _weapon = gameObject.AddComponent<WeaponController>();

        _groundPlane = new Plane(Vector3.up, Vector3.zero); // Плоскость на высоте 0
    }

    private void Update()
    {
        HandleAiming();
        HandleShooting();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (_mover != null)
        {
            _mover.Move(GameInput.Instance.Move);
        }
    }

    private void HandleAiming()
    {
        Ray ray = Camera.main.ScreenPointToRay(GameInput.Instance.PointerScreen);
        if (_groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            if (_turret != null)
            {
                _turret.AimTowards(hitPoint);
            }
        }
    }

    private void HandleShooting()
    {
        if (GameInput.Instance.FirePressed)
        {
            if (_weapon != null)
            {
                _weapon.TryShoot(enemyLayer);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // В 3D используем OnCollisionStay вместо OnCollisionStay2D
        if (collision.collider.GetComponent<EnemyController>() == null) return; // урон только врагам
        if (Time.time < _nextContactDamageTime) return; // применяем урон не чаще раза в секунду
        _nextContactDamageTime = Time.time + 1f; // запоминаем время следующего тика урона
        
        // Используем DamageReceiver (базовый класс) или Health
        collision.collider.GetComponent<DamageReceiver>()?.ApplyDamage(1f); // наносим 1 единицу урона
    }
}
