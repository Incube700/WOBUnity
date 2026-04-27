using RicochetTanks.Configs;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public sealed class TankArmor : MonoBehaviour
    {
        [SerializeField] private int _frontArmor = 100;
        [SerializeField] private int _sideArmor = 70;
        [SerializeField] private int _rearArmor = 40;
        [SerializeField] private float _autoRicochetAngle = 70f;

        public float AutoRicochetAngle => _autoRicochetAngle;

        public void Configure(TankConfig config)
        {
            if (config == null)
            {
                return;
            }

            _frontArmor = config.FrontArmor;
            _sideArmor = config.SideArmor;
            _rearArmor = config.RearArmor;
            _autoRicochetAngle = config.AutoRicochetAngle;
        }

        public ArmorHitInfo ResolveHitInfo(Vector3 projectileDirection, Vector3 contactNormal, int penetration)
        {
            return new ArmorHitInfo(ResolveZone(contactNormal), ResolveArmor(contactNormal), CalculateHitAngle(projectileDirection, contactNormal), penetration);
        }

        public int ResolveArmor(Vector3 contactNormal)
        {
            return ResolveArmor(ResolveZone(contactNormal));
        }

        private int ResolveArmor(ArmorZone zone)
        {
            switch (zone)
            {
                case ArmorZone.Front:
                    return _frontArmor;
                case ArmorZone.Rear:
                    return _rearArmor;
                case ArmorZone.Side:
                    return _sideArmor;
                default:
                    return _sideArmor;
            }
        }

        private ArmorZone ResolveZone(Vector3 contactNormal)
        {
            contactNormal.y = 0f;

            if (contactNormal.sqrMagnitude < 0.001f)
            {
                return ArmorZone.Unknown;
            }

            var forwardDot = Vector3.Dot(contactNormal.normalized, transform.forward);

            if (forwardDot > 0.5f)
            {
                return ArmorZone.Front;
            }

            if (forwardDot < -0.5f)
            {
                return ArmorZone.Rear;
            }

            return ArmorZone.Side;
        }

        private static float CalculateHitAngle(Vector3 projectileDirection, Vector3 contactNormal)
        {
            projectileDirection.y = 0f;
            contactNormal.y = 0f;

            if (projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f)
            {
                return 0f;
            }

            return Vector3.Angle(-projectileDirection.normalized, contactNormal.normalized);
        }
    }
}
