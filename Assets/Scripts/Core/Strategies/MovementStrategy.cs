using UnityEngine;

public abstract class MovementStrategy : ScriptableObject
{
    public abstract void Move(Rigidbody rb, Vector2 input, float speed);
}
