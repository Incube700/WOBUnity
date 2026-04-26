using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 16f;
        [SerializeField] private int _damage = 34;
        [SerializeField] private int _maxRicochets = 3;

        private TankFacade _owner;
        private Rigidbody _rigidbody;
        private Vector3 _direction;
        private int _ricochetCount;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Configure(TankFacade owner, int damage, int maxRicochets, float speed)
        {
            _owner = owner;
            _damage = damage;
            _maxRicochets = maxRicochets;
            _speed = speed;
        }

        public void Initialize(Vector3 direction)
        {
            _direction = direction.normalized;
            ApplyVelocity();
        }

        private void FixedUpdate()
        {
            ApplyVelocity();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (HitResolver.TryApplyDamage(other.collider, _owner, _damage))
            {
                Destroy(gameObject);
                return;
            }

            if (_ricochetCount >= _maxRicochets || other.contactCount == 0)
            {
                Destroy(gameObject);
                return;
            }

            var normal = other.GetContact(0).normal;
            _direction = RicochetCalculator.Reflect(_direction, normal);
            _ricochetCount++;
            ApplyVelocity();
        }

        private void ApplyVelocity()
        {
            if (_rigidbody == null)
            {
                return;
            }

            _rigidbody.linearVelocity = _direction * _speed;
        }
    }
}
