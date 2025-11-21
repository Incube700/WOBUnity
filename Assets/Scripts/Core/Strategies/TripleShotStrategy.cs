using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Weapon/Triple Shot")]
public class TripleShotStrategy : WeaponStrategy
{
    [SerializeField] private float spreadAngle = 15f;

    public override void Fire(Transform firePoint, GameObject bulletPrefab, LayerMask targetLayer)
    {
        // Center shot
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Left shot
        Quaternion leftRot = firePoint.rotation * Quaternion.Euler(0, -spreadAngle, 0);
        Instantiate(bulletPrefab, firePoint.position, leftRot);

        // Right shot
        Quaternion rightRot = firePoint.rotation * Quaternion.Euler(0, spreadAngle, 0);
        Instantiate(bulletPrefab, firePoint.position, rightRot);
    }
}
