using UnityEngine;

namespace RicochetTanks.Gameplay.Tanks
{
    public class TurretAiming : MonoBehaviour
    {
        [SerializeField] private Transform _turret;
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        public void Configure(Transform turret, Camera targetCamera)
        {
            _turret = turret;
            _camera = targetCamera;
        }

        public void AimAtMouse()
        {
            if (_turret == null)
            {
                return;
            }

            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
            {
                return;
            }

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var ground = new Plane(Vector3.up, Vector3.zero);

            if (!ground.Raycast(ray, out var enter))
            {
                return;
            }

            var hitPoint = ray.GetPoint(enter);
            var direction = hitPoint - _turret.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                _turret.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }
    }
}
