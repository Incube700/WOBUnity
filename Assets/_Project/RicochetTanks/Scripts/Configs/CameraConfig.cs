using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Camera Config", fileName = "CameraConfig")]
    public sealed class CameraConfig : ScriptableObject
    {
        [SerializeField] private bool _orthographic = true;
        [SerializeField] private Vector3 _localPosition = new Vector3(0f, 14f, 0f);
        [SerializeField] private Vector3 _localEulerAngles = new Vector3(90f, 0f, 0f);
        [SerializeField] private float _orthographicSize = 9f;
        [SerializeField] private float _nearClipPlane = 0.1f;
        [SerializeField] private float _farClipPlane = 50f;
        [SerializeField] private Color _backgroundColor = new Color(0.04f, 0.05f, 0.055f);

        public bool Orthographic => _orthographic;
        public Vector3 LocalPosition => _localPosition;
        public Vector3 LocalEulerAngles => _localEulerAngles;
        public float OrthographicSize => _orthographicSize;
        public float NearClipPlane => _nearClipPlane;
        public float FarClipPlane => _farClipPlane;
        public Color BackgroundColor => _backgroundColor;
    }
}
