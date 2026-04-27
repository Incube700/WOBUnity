using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Tank Config", fileName = "TankConfig")]
    public sealed class TankConfig : ScriptableObject
    {
        [SerializeField] private int _maxHp = 100;
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _turnSpeed = 150f;
        [SerializeField] private float _turretRotationSpeed = 220f;
        [SerializeField] private int _frontArmor = 100;
        [SerializeField] private int _sideArmor = 70;
        [SerializeField] private int _rearArmor = 40;
        [SerializeField] private float _autoRicochetAngle = 70f;

        public int MaxHp => _maxHp;
        public float MoveSpeed => _moveSpeed;
        public float TurnSpeed => _turnSpeed;
        public float TurretRotationSpeed => _turretRotationSpeed;
        public int FrontArmor => _frontArmor;
        public int SideArmor => _sideArmor;
        public int RearArmor => _rearArmor;
        public float AutoRicochetAngle => _autoRicochetAngle;
    }
}
