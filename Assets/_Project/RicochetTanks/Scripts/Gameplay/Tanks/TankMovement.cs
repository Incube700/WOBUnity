using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    public class TankMovement : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _turnSpeed = 150f;

        private Rigidbody _rigidbody;
        private float _throttle;
        private float _turn;
        private bool _isMovementEnabled = true;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Configure(Rigidbody body)
        {
            _rigidbody = body;
        }

        public void Configure(Rigidbody body, float moveSpeed, float turnSpeed)
        {
            _rigidbody = body;
            _moveSpeed = moveSpeed;
            _turnSpeed = turnSpeed;
        }

        public void SetInput(float throttle, float turn)
        {
            _throttle = Mathf.Clamp(throttle, -1f, 1f);
            _turn = Mathf.Clamp(turn, -1f, 1f);
        }

        public void SetMovementEnabled(bool isMovementEnabled)
        {
            _isMovementEnabled = isMovementEnabled;

            if (!_isMovementEnabled)
            {
                SetInput(0f, 0f);
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

            var rotationDelta = Quaternion.Euler(0f, _turn * _turnSpeed * Time.fixedDeltaTime, 0f);
            _rigidbody.MoveRotation(_rigidbody.rotation * rotationDelta);
            _rigidbody.linearVelocity = transform.forward * (_throttle * _moveSpeed);
        }

        private void StopRigidbody()
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
