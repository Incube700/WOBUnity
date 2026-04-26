using UnityEngine;

namespace RicochetTanks.Gameplay
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

        public void AimAtMouse()
        {
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
