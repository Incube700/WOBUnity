using UnityEngine;

namespace RicochetTanks.Gameplay
{
    public class TankShooter : MonoBehaviour
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _cooldown = 0.35f;

        private float _nextShotTime;

        public void TryShoot()
        {
            if (Time.time < _nextShotTime)
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

            var projectile = projectileObject.AddComponent<Projectile>();
            projectile.Initialize(_muzzle.forward);
            Destroy(projectileObject, 8f);
        }
    }
}
