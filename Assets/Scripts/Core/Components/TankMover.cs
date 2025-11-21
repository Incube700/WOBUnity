using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TankMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private MovementStrategy movementStrategy;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (movementStrategy == null)
        {
            movementStrategy = ScriptableObject.CreateInstance<StandardMovementStrategy>();
        }
    }

    public void Move(Vector2 input)
    {
        if (movementStrategy != null)
        {
            movementStrategy.Move(rb, input, moveSpeed);
        }
    }

    public void Stop()
    {
        rb.linearVelocity = Vector3.zero;
    }
}
