using UnityEngine;

public static class MathAngles
{
    /// <summary>
    /// Calculates the angle between the velocity vector and the surface (90 - angle of incidence).
    /// 90 degrees means perpendicular impact (direct hit).
    /// 0 degrees means parallel to surface (graze).
    /// </summary>
    public static float ImpactAngle(Vector3 velocity, Vector3 normal)
    {
        // Angle between -velocity and normal is the angle of incidence (0 if perpendicular).
        float angleWithNormal = Vector3.Angle(-velocity, normal);
        // We want the angle with the surface, so 90 - angleWithNormal.
        return 90f - angleWithNormal;
    }
}
