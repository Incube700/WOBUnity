using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Weapon/Simple Cannon")]
public class SimpleCannonStrategy : WeaponStrategy
{
    public override void Fire(Transform firePoint, GameObject bulletPrefab, LayerMask targetLayer)
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
