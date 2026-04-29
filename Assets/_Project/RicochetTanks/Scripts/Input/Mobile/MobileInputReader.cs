using UnityEngine;

namespace RicochetTanks.Input.Mobile
{
    public sealed class MobileInputReader : MonoBehaviour, ITankInputReader, IArcadeMovementInputReader
    {
        [SerializeField] private MobileControlsView _controlsView;
        [SerializeField] private float _aimDistance = 10f;
        [SerializeField] private float _deadZone = 0.05f;

        public void Configure(MobileControlsView controlsView)
        {
            _controlsView = controlsView;
        }

        public void ReadTankInput(out float throttle, out float turn)
        {
            var movement = _controlsView != null ? _controlsView.Movement : Vector2.zero;
            throttle = ApplyDeadZone(movement.y);
            turn = ApplyDeadZone(movement.x);
        }

        public bool TryReadMovementVector(out Vector2 movement)
        {
            movement = _controlsView != null ? _controlsView.Movement : Vector2.zero;
            if (movement.sqrMagnitude <= _deadZone * _deadZone)
            {
                movement = Vector2.zero;
                return false;
            }

            movement = Vector2.ClampMagnitude(movement, 1f);
            return true;
        }

        public bool TryGetAimPoint(Camera camera, Transform aimOrigin, float planeY, out Vector3 aimPoint)
        {
            aimPoint = default;

            if (_controlsView == null || aimOrigin == null)
            {
                return false;
            }

            var aim = _controlsView.Aim;
            if (aim.sqrMagnitude <= _deadZone * _deadZone)
            {
                return false;
            }

            var direction = new Vector3(aim.x, 0f, aim.y).normalized;
            aimPoint = aimOrigin.position + direction * Mathf.Max(1f, _aimDistance);
            aimPoint.y = planeY;
            return true;
        }

        public bool IsFirePressed()
        {
            return _controlsView != null && _controlsView.ConsumeFirePressed();
        }

        public bool IsRestartPressed()
        {
            return false;
        }

        private float ApplyDeadZone(float value)
        {
            return Mathf.Abs(value) <= _deadZone ? 0f : Mathf.Clamp(value, -1f, 1f);
        }
    }
}
