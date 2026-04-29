using UnityEngine;

namespace RicochetTanks.Input
{
    public interface IArcadeMovementInputReader
    {
        bool TryReadMovementVector(out Vector2 movement);
    }
}
