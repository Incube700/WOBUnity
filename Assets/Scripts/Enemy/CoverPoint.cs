using UnityEngine;

public class CoverPoint2D : MonoBehaviour
{
    [Tooltip("Radius around this point where the cover is considered effective.")]
    public float reachRadius = 1.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, 0.3f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, reachRadius);
    }
}
