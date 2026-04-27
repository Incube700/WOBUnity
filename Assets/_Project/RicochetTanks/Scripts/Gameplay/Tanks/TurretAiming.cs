using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    public class TurretAiming : MonoBehaviour
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Camera _camera;
        [SerializeField] private float _rotationSpeed = 220f;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        public void Configure(Transform turret, Camera targetCamera)
        {
            Configure(turret, targetCamera, _rotationSpeed);
        }

        public void Configure(Transform turret, Camera targetCamera, float rotationSpeed)
        {
            _turret = turret;
            _camera = targetCamera;
            _rotationSpeed = rotationSpeed;
        }

        public void AimAtMouse()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
            {
                return;
            }

            var ray = _camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            var ground = new Plane(Vector3.up, Vector3.zero);

            if (!ground.Raycast(ray, out var enter))
            {
                return;
            }

            AimAt(ray.GetPoint(enter));
        }

        public void AimAt(Vector3 worldPoint)
        {
            if (_turret == null)
            {
                return;
            }

            var direction = worldPoint - _turret.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                var targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                _turret.rotation = Quaternion.RotateTowards(_turret.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
    }
}
