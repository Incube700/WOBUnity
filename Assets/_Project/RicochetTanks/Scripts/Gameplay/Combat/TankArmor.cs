using RicochetTanks.Configs;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public sealed class TankArmor : MonoBehaviour
    {
        private const float CornerNormalMinAxis = 0.6f;

        [SerializeField] private int _frontArmor = 50;
        [SerializeField] private int _sideArmor = 40;
        [SerializeField] private int _rearArmor = 10;
        [SerializeField] private float _autoRicochetAngle = 70f;

        public float AutoRicochetAngle => _autoRicochetAngle;
        public float CriticalRicochetAngle => _autoRicochetAngle;

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

        public ArmorHitInfo ResolveHitInfo(Vector3 projectileDirection, Vector3 contactNormal, float currentPenetration, float kineticFactor)
        {
            var zone = ResolveZone(contactNormal);
            var armor = ResolveArmor(zone);
            var hitAngle = CalculateHitAngle(projectileDirection, contactNormal);
            var effectiveArmor = CalculateEffectiveArmor(armor, hitAngle);
            return new ArmorHitInfo(zone, armor, effectiveArmor, hitAngle, currentPenetration, kineticFactor);
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
                case ArmorZone.Corner:
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

            if (IsCornerNormal(contactNormal))
            {
                return ArmorZone.Corner;
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

        public bool IsCornerHit(Vector3 contactNormal)
        {
            contactNormal.y = 0f;

            if (contactNormal.sqrMagnitude < 0.001f)
            {
                return false;
            }

            return IsCornerNormal(contactNormal);
        }

        private bool IsCornerNormal(Vector3 contactNormal)
        {
            var localNormal = transform.InverseTransformDirection(contactNormal.normalized);
            localNormal.y = 0f;

            if (localNormal.sqrMagnitude < 0.001f)
            {
                return false;
            }

            localNormal.Normalize();
            return Mathf.Abs(localNormal.x) >= CornerNormalMinAxis && Mathf.Abs(localNormal.z) >= CornerNormalMinAxis;
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

        private static float CalculateEffectiveArmor(float armor, float hitAngle)
        {
            var impactCos = Mathf.Cos(hitAngle * Mathf.Deg2Rad);
            return armor / Mathf.Max(0.01f, impactCos);
        }
    }
}
