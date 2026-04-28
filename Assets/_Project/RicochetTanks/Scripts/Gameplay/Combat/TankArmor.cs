using RicochetTanks.Configs;
using UnityEngine;

namespace RicochetTanks.Gameplay.Combat
{
    public sealed class TankArmor : MonoBehaviour
    {
        private const float CornerNormalMinAxis = 0.6f;
        private const float SafeMinCos = 0.01f;

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

        public ArmorHitInfo ResolveHitInfo(
            Vector3 projectileDirection,
            Vector3 contactNormal,
            float currentPenetration,
            float kineticFactor)
        {
            var zone = ResolveZone(contactNormal);
            var armor = ResolveArmor(zone);
            var impactDot = CalculateImpactDot(projectileDirection, contactNormal);
            var hitAngle = CalculateHitAngle(impactDot);
            var effectiveArmor = CalculateEffectiveArmor(armor, impactDot);
            return new ArmorHitInfo(zone, armor, effectiveArmor, hitAngle, impactDot, currentPenetration, kineticFactor);
        }

        public int ResolveArmor(Vector3 contactNormal)
        {
            return ResolveArmor(ResolveZone(contactNormal));
        }

        public int ResolveArmor(ArmorZone zone)
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

        public ArmorZone ResolveZone(Vector3 contactNormal)
        {
            var localNormal = transform.InverseTransformDirection(contactNormal);
            localNormal.y = 0f;

            if (localNormal.sqrMagnitude < 0.001f)
            {
                return ArmorZone.Unknown;
            }

            localNormal.Normalize();
            var absoluteX = Mathf.Abs(localNormal.x);
            var absoluteZ = Mathf.Abs(localNormal.z);

            if (absoluteZ >= absoluteX && localNormal.z > 0f)
            {
                return ArmorZone.Front;
            }

            if (absoluteZ >= absoluteX && localNormal.z < 0f)
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

            var localNormal = transform.InverseTransformDirection(contactNormal.normalized);
            localNormal.y = 0f;

            if (localNormal.sqrMagnitude < 0.001f)
            {
                return false;
            }

            localNormal.Normalize();
            return Mathf.Abs(localNormal.x) >= CornerNormalMinAxis && Mathf.Abs(localNormal.z) >= CornerNormalMinAxis;
        }

        private static float CalculateImpactDot(Vector3 projectileDirection, Vector3 contactNormal)
        {
            return projectileDirection.sqrMagnitude < 0.001f || contactNormal.sqrMagnitude < 0.001f
                ? 1f
                : Vector3.Dot(-projectileDirection.normalized, contactNormal.normalized);
        }

        private static float CalculateHitAngle(float impactDot)
        {
            return Mathf.Acos(Mathf.Clamp(impactDot, -1f, 1f)) * Mathf.Rad2Deg;
        }

        private static float CalculateEffectiveArmor(float armor, float impactDot)
        {
            return armor / Mathf.Max(Mathf.Clamp01(impactDot), SafeMinCos);
        }
    }
}
