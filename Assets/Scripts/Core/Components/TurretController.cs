using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform turretPivot;
    [SerializeField] private float rotationSpeed = 10f; // Optional smoothing

    private void Awake()
    {
        if (turretPivot == null)
        {
            // Try to find it automatically
            turretPivot = transform.Find("TurretPivot");
            if (turretPivot == null) turretPivot = transform.Find("Turret");
            if (turretPivot == null && transform.childCount > 0) turretPivot = transform.GetChild(0);
        }
    }

    public void AimTowards(Vector3 targetPos)
    {
        if (turretPivot == null) return;

        Vector3 dir = targetPos - turretPivot.position;
        dir.y = 0f; // Keep it flat

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            turretPivot.rotation = targetRotation;
        }
    }
}
