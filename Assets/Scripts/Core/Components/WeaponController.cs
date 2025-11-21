using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponStrategy weaponStrategy;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 0.5f;

    private float _nextFireTime;

    private void Awake()
    {
        if (weaponStrategy == null)
        {
            weaponStrategy = ScriptableObject.CreateInstance<SimpleCannonStrategy>();
        }
        
        if (bulletPrefab == null)
        {
             bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
             if (bulletPrefab == null) bulletPrefab = Resources.Load<GameObject>("Bullet");
        }
    }

    public bool TryShoot(LayerMask targetLayer)
    {
        if (Time.time < _nextFireTime) return false;

        if (weaponStrategy != null && muzzle != null && bulletPrefab != null)
        {
            weaponStrategy.Fire(muzzle, bulletPrefab, targetLayer);
            _nextFireTime = Time.time + fireCooldown;
            return true;
        }
        return false;
    }
}
