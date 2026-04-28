using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    public class TankMovement : MonoBehaviour
    {
        private const float FullTurnDegrees = 360f;

        [SerializeField] private float _maxForwardSpeed = 5f;
        [SerializeField] private float _maxReverseSpeed = 2.5f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _brakeDeceleration = 14f;
        [SerializeField] private float _coastDeceleration = 6f;
        [SerializeField] private float _turnSpeed = 140f;
        [SerializeField] private float _turnSpeedAtLowVelocity = 80f;
        [SerializeField] private float _inputDeadZone = 0.05f;

        private Rigidbody _rigidbody;
        private float _throttle;
        private float _turn;
        private float _currentSpeed;
        private float _currentYaw;
        private bool _isMovementEnabled = true;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            SyncYawFromRigidbody();
            ApplyRigidbodyConstraints();
        }

        public void Configure(Rigidbody body)
        {
            _rigidbody = body;
            SyncYawFromRigidbody();
            ApplyRigidbodyConstraints();
        }

        public void Configure(Rigidbody body, float moveSpeed, float turnSpeed)
        {
            _rigidbody = body;
            SyncYawFromRigidbody();
            _maxForwardSpeed = Mathf.Max(0f, moveSpeed);
            _maxReverseSpeed = _maxForwardSpeed * 0.5f;
            _turnSpeed = Mathf.Max(0f, turnSpeed);
            _turnSpeedAtLowVelocity = Mathf.Min(_turnSpeed, _turnSpeedAtLowVelocity);
            ApplyRigidbodyConstraints();
        }

        public void Configure(
            Rigidbody body,
            float maxForwardSpeed,
            float maxReverseSpeed,
            float acceleration,
            float brakeDeceleration,
            float coastDeceleration,
            float turnSpeed,
            float turnSpeedAtLowVelocity,
            float inputDeadZone)
        {
            _rigidbody = body;
            SyncYawFromRigidbody();
            _maxForwardSpeed = Mathf.Max(0f, maxForwardSpeed);
            _maxReverseSpeed = Mathf.Max(0f, maxReverseSpeed);
            _acceleration = Mathf.Max(0f, acceleration);
            _brakeDeceleration = Mathf.Max(0f, brakeDeceleration);
            _coastDeceleration = Mathf.Max(0f, coastDeceleration);
            _turnSpeed = Mathf.Max(0f, turnSpeed);
            _turnSpeedAtLowVelocity = Mathf.Max(0f, turnSpeedAtLowVelocity);
            _inputDeadZone = Mathf.Clamp01(inputDeadZone);
            ApplyRigidbodyConstraints();
        }

        public void SetInput(float throttle, float turn)
        {
            _throttle = ApplyDeadZone(Mathf.Clamp(throttle, -1f, 1f));
            _turn = ApplyDeadZone(Mathf.Clamp(turn, -1f, 1f));
        }

        public void SetMovementEnabled(bool isMovementEnabled)
        {
            _isMovementEnabled = isMovementEnabled;

            if (!_isMovementEnabled)
            {
                SetInput(0f, 0f);
                _currentSpeed = 0f;
                StopRigidbody();
            }
        }

        private void FixedUpdate()
        {
            if (_rigidbody == null)
            {
                return;
            }

            if (!_isMovementEnabled)
            {
                StopRigidbody();
                return;
            }

            ClearAngularVelocity();
            RotateBody();
            MoveBody();
        }

        private void RotateBody()
        {
            if (Mathf.Abs(_turn) > _inputDeadZone)
            {
                var turnSpeed = Mathf.Abs(_currentSpeed) <= _inputDeadZone ? _turnSpeedAtLowVelocity : _turnSpeed;
                _currentYaw = Mathf.Repeat(_currentYaw + _turn * turnSpeed * Time.fixedDeltaTime, FullTurnDegrees);
            }

            _rigidbody.MoveRotation(Quaternion.Euler(0f, _currentYaw, 0f));
        }

        private void MoveBody()
        {
            UpdateCurrentSpeed(Time.fixedDeltaTime);
            var forward = Quaternion.Euler(0f, _currentYaw, 0f) * Vector3.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= _inputDeadZone * _inputDeadZone)
            {
                return;
            }

            var planarVelocity = forward.normalized * _currentSpeed;

            if (_rigidbody.isKinematic)
            {
                var nextPosition = _rigidbody.position + planarVelocity * Time.fixedDeltaTime;
                _rigidbody.MovePosition(nextPosition);
                return;
            }

            _rigidbody.linearVelocity = new Vector3(planarVelocity.x, _rigidbody.linearVelocity.y, planarVelocity.z);
        }

        private void UpdateCurrentSpeed(float deltaTime)
        {
            if (_throttle > _inputDeadZone)
            {
                var targetSpeed = _currentSpeed < 0f ? 0f : _maxForwardSpeed * _throttle;
                var rate = _currentSpeed < 0f ? _brakeDeceleration : _acceleration;
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * deltaTime);
                return;
            }

            if (_throttle < -_inputDeadZone)
            {
                var targetSpeed = _currentSpeed > 0f ? 0f : _maxReverseSpeed * _throttle;
                var rate = _currentSpeed > 0f ? _brakeDeceleration : _acceleration;
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, targetSpeed, rate * deltaTime);
                return;
            }

            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, _coastDeceleration * deltaTime);
        }

        private void ApplyRigidbodyConstraints()
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void SyncYawFromRigidbody()
        {
            if (_rigidbody == null)
            {
                _currentYaw = transform.eulerAngles.y;
                return;
            }

            _currentYaw = _rigidbody.rotation.eulerAngles.y;
        }

        private void StopRigidbody()
        {
            if (_rigidbody == null || _rigidbody.isKinematic)
            {
                return;
            }

            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void ClearAngularVelocity()
        {
            if (_rigidbody == null || _rigidbody.isKinematic)
            {
                return;
            }

            _rigidbody.angularVelocity = Vector3.zero;
        }

        private float ApplyDeadZone(float value)
        {
            return Mathf.Abs(value) <= _inputDeadZone ? 0f : value;
        }
    }
}
