using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 16f;
        [SerializeField] private int _damage = 34;
        [SerializeField] private int _maxRicochets = 3;

        private Vector3 _direction;
        private int _ricochetCount;

        public void Initialize(Vector3 direction)
        {
            _direction = direction.normalized;
        }

        private void Update()
        {
            transform.position += _direction * (_speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.TryGetComponent<TankHealth>(out var health))
            {
                health.ApplyDamage(_damage);
                Destroy(gameObject);
                return;
            }

            var normal = other.contacts[0].normal;
            _direction = Vector3.Reflect(_direction, normal).normalized;
            _ricochetCount++;

            if (_ricochetCount > _maxRicochets)
            {
                Destroy(gameObject);
            }
        }
    }
}
