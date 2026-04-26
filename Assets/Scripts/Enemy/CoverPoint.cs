using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    [SerializeField] private float _reachRadius = 0.5f;

    public float reachRadius => _reachRadius;

    private void OnValidate()
    {
        if (_reachRadius < 0.1f)
            _reachRadius = 0.1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, 0.3f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _reachRadius);
    }
}
