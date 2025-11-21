using UnityEngine;

public abstract class WeaponStrategy : ScriptableObject
{
    public abstract void Fire(Transform firePoint, GameObject bulletPrefab, LayerMask targetLayer);
}
