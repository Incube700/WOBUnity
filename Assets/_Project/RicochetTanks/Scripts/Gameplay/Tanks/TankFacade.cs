using RicochetTanks.Gameplay.Combat;
using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    [DisallowMultipleComponent]
    public class TankFacade : MonoBehaviour
    {
        [SerializeField] private TankMovement _movement;
        [SerializeField] private TurretAiming _aiming;
        [SerializeField] private TankShooter _shooter;
        [SerializeField] private TankHealth _health;
        [SerializeField] private PlayerTankController _controller;

        public TankMovement Movement => _movement;
        public TurretAiming Aiming => _aiming;
        public TankShooter Shooter => _shooter;
        public TankHealth Health => _health;

        public void Configure(
            TankMovement movement,
            TurretAiming aiming,
            TankShooter shooter,
            TankHealth health,
            PlayerTankController controller)
        {
            _movement = movement;
            _aiming = aiming;
            _shooter = shooter;
            _health = health;
            _controller = controller;

            if (_controller != null)
            {
                _controller.Configure(this);
            }
        }

        public void SetPlayerControlled(bool isPlayerControlled)
        {
            if (_controller != null)
            {
                _controller.enabled = isPlayerControlled;
            }
        }
    }
}
