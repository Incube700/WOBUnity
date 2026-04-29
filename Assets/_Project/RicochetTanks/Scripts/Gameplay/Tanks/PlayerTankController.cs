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
        [SerializeField] private float _arcadeFullTurnAngle = 90f;
        [SerializeField] private float _arcadeFullThrottleAngle = 25f;
        [SerializeField] private float _arcadeZeroThrottleAngle = 130f;

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
            if (inputReader is IArcadeMovementInputReader arcadeInputReader
                && arcadeInputReader.TryReadMovementVector(out var movement))
            {
                ConvertArcadeMovement(movement, out throttle, out turn);
                return;
            }

            inputReader.ReadTankInput(out throttle, out turn);
        }

        private void ConvertArcadeMovement(Vector2 movement, out float throttle, out float turn)
        {
            var desiredDirection = new Vector3(movement.x, 0f, movement.y);
            var movementMagnitude = Mathf.Clamp01(movement.magnitude);

            if (desiredDirection.sqrMagnitude <= 0.0001f)
            {
                throttle = 0f;
                turn = 0f;
                return;
            }

            var forward = Vector3.ProjectOnPlane(_tank.transform.forward, Vector3.up);
            if (forward.sqrMagnitude <= 0.0001f)
            {
                forward = Vector3.forward;
            }

            var signedAngle = Vector3.SignedAngle(forward.normalized, desiredDirection.normalized, Vector3.up);
            var absoluteAngle = Mathf.Abs(signedAngle);
            var fullTurnAngle = Mathf.Max(1f, _arcadeFullTurnAngle);

            turn = Mathf.Clamp(signedAngle / fullTurnAngle, -1f, 1f);
            throttle = movementMagnitude * Mathf.InverseLerp(_arcadeZeroThrottleAngle, _arcadeFullThrottleAngle, absoluteAngle);
        }
    }
}
