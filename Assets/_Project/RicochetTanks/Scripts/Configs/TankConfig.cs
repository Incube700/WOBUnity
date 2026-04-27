using UnityEngine;

namespace RicochetTanks.Configs
{
    [CreateAssetMenu(menuName = "Ricochet Tanks/Tank Config", fileName = "TankConfig")]
    public sealed class TankConfig : ScriptableObject
    {
        [SerializeField] private int _maxHp = 100;
        [SerializeField] private float _moveSpeed = 4.5f;
        [SerializeField] private float _turnSpeed = 150f;

        public int MaxHp => _maxHp;
        public float MoveSpeed => _moveSpeed;
        public float TurnSpeed => _turnSpeed;
    }
}
