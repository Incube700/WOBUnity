using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    public class PlayerTankController : MonoBehaviour
    {
        [SerializeField] private TankFacade _tank;

        public void Configure(TankFacade tank)
        {
            _tank = tank;
        }

        private void Awake()
        {
            if (_tank == null)
            {
                _tank = GetComponent<TankFacade>();
            }
        }

        private void Update()
        {
            if (_tank == null || _tank.Movement == null || _tank.Aiming == null || _tank.Shooter == null)
            {
                return;
            }

            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            _tank.Movement.SetMoveDirection(new Vector2(horizontal, vertical));
            _tank.Aiming.AimAtMouse();

            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                _tank.Shooter.TryShoot();
            }
        }
    }
}
