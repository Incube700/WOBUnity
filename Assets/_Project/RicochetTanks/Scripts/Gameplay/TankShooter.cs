using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class TankShooter : MonoBehaviour
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _cooldown = 0.35f;
        [SerializeField] private float _projectileSpeed = 22f;
        [SerializeField] private int _projectileDamage = 34;
        [SerializeField] private int _maxRicochets = 3;

        private TankFacade _owner;
        private float _nextShotTime;

        public void Configure(Transform muzzle, TankFacade owner)
        {
            _muzzle = muzzle;
            _owner = owner;
        }

        public void TryShoot()
        {
            if (_muzzle == null || Time.time < _nextShotTime)
            {
                return;
            }

            _nextShotTime = Time.time + _cooldown;
            var projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.transform.position = _muzzle.position;
            projectileObject.transform.localScale = Vector3.one * 0.25f;

            var rigidbody = projectileObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.Configure(_owner, _projectileDamage, _maxRicochets, _projectileSpeed);
            projectile.Initialize(_muzzle.forward);
            TintProjectile(projectileObject);
            Destroy(projectileObject, 8f);
        }

        private static void TintProjectile(GameObject projectileObject)
        {
            if (!projectileObject.TryGetComponent<Renderer>(out var renderer))
            {
                return;
            }

            renderer.material.color = new Color(1f, 0.9f, 0.2f);
        }
    }
}
