using RicochetTanks.Input;
using RicochetTanks.Input.Desktop;
using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    public class PlayerTankController : MonoBehaviour
    {
        [SerializeField] private TankFacade _tank;
        [SerializeField] private DesktopInputReader _inputReader;
        [SerializeField] private Camera _camera;

        private ITankInputReader _activeInputReader;
        private bool _isControlEnabled = true;

        public void Configure(TankFacade tank)
        {
            _tank = tank;
        }

        public void Configure(TankFacade tank, DesktopInputReader inputReader, Camera camera)
        {
            _inputReader = inputReader;
            Configure(tank, (ITankInputReader)inputReader, camera);
        }

        public void Configure(TankFacade tank, ITankInputReader inputReader, Camera camera)
        {
            _tank = tank;
            _activeInputReader = inputReader;
            _camera = camera;
        }

        public void SetControlEnabled(bool isControlEnabled)
        {
            _isControlEnabled = isControlEnabled;

            if (!_isControlEnabled && _tank != null && _tank.Movement != null)
            {
                _tank.Movement.SetInput(0f, 0f);
            }
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
            if (!_isControlEnabled || _tank == null || _tank.Movement == null || _tank.Aiming == null || _tank.Shooter == null)
            {
                return;
            }

            var inputReader = _activeInputReader ?? _inputReader;

            if (inputReader == null)
            {
                return;
            }

            ReadMovementInput(inputReader, out var throttle, out var turn);
            _tank.Movement.SetInput(throttle, turn);

            if (inputReader.TryGetAimPoint(_camera, _tank.transform, 0f, out var aimPoint))
            {
                _tank.Aiming.AimAt(aimPoint);
            }

            if (inputReader.IsFirePressed())
            {
                _tank.Shooter.TryShoot();
            }
        }

        private void ReadMovementInput(ITankInputReader inputReader, out float throttle, out float turn)
        {
            inputReader.ReadTankInput(out throttle, out turn);
        }
    }
}