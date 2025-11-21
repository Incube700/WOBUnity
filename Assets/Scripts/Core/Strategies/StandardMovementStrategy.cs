using UnityEngine;

[CreateAssetMenu(menuName = "Strategies/Movement/Standard")]
public class StandardMovementStrategy : MovementStrategy
{
    public override void Move(Rigidbody rb, Vector2 input, float speed)
    {
        // Map Y input to Z axis for 3D movement on XZ plane
        Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;
        rb.linearVelocity = moveDir * speed;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, Time.deltaTime * 10f);
        }
    }
}
