using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class TankMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        private Rigidbody _rigidbody;
        private Vector3 _moveDirection;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void SetMoveDirection(Vector2 input)
        {
            _moveDirection = new Vector3(input.x, 0f, input.y).normalized;
        }

        private void FixedUpdate()
        {
            _rigidbody.linearVelocity = _moveDirection * _speed;

            if (_moveDirection.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            }
        }
    }
}
