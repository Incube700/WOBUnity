using RicochetTanks.Input;
using UnityEngine;

namespace RicochetTanks.Input.Desktop
{
    public sealed class DesktopInputReader : MonoBehaviour, ITankInputReader
    {
        public void ReadTankInput(out float throttle, out float turn)
        {
            throttle = 0f;
            turn = 0f;

            if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
            {
                throttle += 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
            {
                throttle -= 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
            {
                turn += 1f;
            }

            if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
            {
                turn -= 1f;
            }
        }

        public bool IsFirePressed()
        {
            return UnityEngine.Input.GetMouseButtonDown(0) || UnityEngine.Input.GetKeyDown(KeyCode.Space);
        }

        public bool IsRestartPressed()
        {
            return UnityEngine.Input.GetKeyDown(KeyCode.R);
        }

        public bool TryGetAimPoint(Camera camera, Transform aimOrigin, float planeY, out Vector3 aimPoint)
        {
            return TryGetAimPoint(camera, planeY, out aimPoint);
        }

        public bool TryGetAimPoint(Camera camera, float planeY, out Vector3 aimPoint)
        {
            aimPoint = default;

            if (camera == null)
            {
                return false;
            }

            var ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            var ground = new Plane(Vector3.up, new Vector3(0f, planeY, 0f));

            if (!ground.Raycast(ray, out var enter))
            {
                return false;
            }

            aimPoint = ray.GetPoint(enter);
            return true;
        }
    }
}
