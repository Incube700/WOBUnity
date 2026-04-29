using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    internal sealed class ImpactVfxFactory
    {
        private readonly CombatVfxConfig _config;
        private readonly Transform _root;

        public ImpactVfxFactory(CombatVfxConfig config, Transform root)
        {
            _config = config;
            _root = root;
        }

        public void CreateWorldImpact(Vector3 point, Vector3 normal)
        {
            if (_config == null)
            {
                return;
            }

            CreateVfxOrFallback(
                _config.WorldImpactVfxPrefab,
                "World Impact VFX",
                point,
                normal,
                _config.ImpactColor,
                _config.VfxScale,
                _config.ImpactLifetime);
        }

        public void CreateTankHit(Vector3 point, Vector3 normal, HitResult result)
        {
            if (_config == null)
            {
                return;
            }

            if (result == HitResult.Ricochet)
            {
                CreateRicochet(point, normal);
                return;
            }

            if (result == HitResult.NoPen)
            {
                CreateVfxOrFallback(
                    _config.NoPenetrationVfxPrefab,
                    "No Penetration VFX",
                    point,
                    normal,
                    _config.NoPenetrationColor,
                    _config.VfxScale,
                    _config.ImpactLifetime);
                return;
            }

            CreateVfxOrFallback(
                _config.TankPenetrationVfxPrefab,
                "Tank Penetration VFX",
                point,
                normal,
                _config.ImpactColor,
                _config.VfxScale,
                _config.ImpactLifetime);
        }

        public void CreateRicochet(Vector3 point, Vector3 normal)
        {
            if (_config == null)
            {
                return;
            }

            CreateVfxOrFallback(
                _config.RicochetSparkVfxPrefab,
                "Ricochet Spark VFX",
                point,
                normal,
                _config.RicochetColor,
                _config.VfxScale,
                _config.RicochetSparkLifetime);
        }

        private void CreateVfxOrFallback(
            GameObject prefab,
            string objectName,
            Vector3 point,
            Vector3 normal,
            Color color,
            float scale,
            float lifetime)
        {
            CombatVfxUtility.CreateVfxOrFallback(_config, _root, prefab, objectName, point, normal, color, scale, lifetime);
        }
    }
}
