using UnityEngine;

namespace RicochetTanks.Input
{
    public interface ITankInputReader
    {
        void ReadTankInput(out float throttle, out float turn);
        bool TryGetAimPoint(Camera camera, Transform aimOrigin, float planeY, out Vector3 aimPoint);
        bool IsFirePressed();
        bool IsRestartPressed();
    }
}
