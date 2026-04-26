using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class PlayerTankController : MonoBehaviour
    {
        [SerializeField] private TankMovement _movement;
        [SerializeField] private TurretAiming _aiming;
        [SerializeField] private TankShooter _shooter;

        private void Update()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            _movement.SetMoveDirection(new Vector2(horizontal, vertical));
            _aiming.AimAtMouse();

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                _shooter.TryShoot();
            }
        }
    }
}
