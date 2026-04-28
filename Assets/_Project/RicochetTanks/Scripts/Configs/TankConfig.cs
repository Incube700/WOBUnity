using UnityEngine;
using UnityEngine.Serialization;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Tank Config", fileName = "TankConfig")]
    public sealed class TankConfig : ScriptableObject
    {
        [SerializeField] private int _maxHp = 100;
        [FormerlySerializedAs("_moveSpeed")]
        [SerializeField] private float _maxForwardSpeed = 5f;
        [SerializeField] private float _maxReverseSpeed = 2.5f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _brakeDeceleration = 14f;
        [SerializeField] private float _coastDeceleration = 6f;
        [SerializeField] private float _turnSpeed = 140f;
        [SerializeField] private float _turnSpeedAtLowVelocity = 80f;
        [SerializeField] private float _turretRotationSpeed = 360f;
        [SerializeField] private float _inputDeadZone = 0.05f;
        [SerializeField] private int _frontArmor = 100;
        [SerializeField] private int _sideArmor = 70;
        [SerializeField] private int _rearArmor = 40;
        [SerializeField] private float _autoRicochetAngle = 70f;

        public int MaxHp => _maxHp;
        public float MaxForwardSpeed => Mathf.Max(0f, _maxForwardSpeed);
        public float MaxReverseSpeed => Mathf.Max(0f, _maxReverseSpeed);
        public float Acceleration => Mathf.Max(0f, _acceleration);
        public float BrakeDeceleration => Mathf.Max(0f, _brakeDeceleration);
        public float CoastDeceleration => Mathf.Max(0f, _coastDeceleration);
        public float TurnSpeed => Mathf.Max(0f, _turnSpeed);
        public float TurnSpeedAtLowVelocity => Mathf.Max(0f, _turnSpeedAtLowVelocity);
        public float TurretRotationSpeed => Mathf.Max(0f, _turretRotationSpeed);
        public float InputDeadZone => Mathf.Clamp01(_inputDeadZone);
        public float MoveSpeed => MaxForwardSpeed;
        public int FrontArmor => _frontArmor;
        public int SideArmor => _sideArmor;
        public int RearArmor => _rearArmor;
        public float AutoRicochetAngle => _autoRicochetAngle;
    }
}
