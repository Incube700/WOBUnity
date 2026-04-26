using UnityEngine;

/// <summary>
/// Abstract base class for any component that wants to receive damage from bullets.
/// Legacy compatibility base for old prototype scripts.
/// </summary>
public abstract class DamageReceiver : MonoBehaviour
{
    public abstract void ApplyDamage(float amount);
}
