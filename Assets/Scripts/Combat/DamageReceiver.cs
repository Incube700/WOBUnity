using UnityEngine;

/// <summary>
/// Abstract base class for any component that wants to receive damage from bullets.
/// </summary>
public abstract class DamageReceiver2D : MonoBehaviour
{
    public abstract void ApplyDamage(float amount);
}
