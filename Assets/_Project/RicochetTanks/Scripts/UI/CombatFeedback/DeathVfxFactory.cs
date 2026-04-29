using RicochetTanks.Configs;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    internal sealed class DeathVfxFactory
    {
        private readonly CombatVfxConfig _config;
        private readonly Transform _root;

        public DeathVfxFactory(CombatVfxConfig config, Transform root)
        {
            _config = config;
            _root = root;
        }

        public void CreateTankDeath(Vector3 position, Quaternion rotation)
        {
            if (_config == null)
            {
                return;
            }

            CombatVfxUtility.CreateVfxOrFallback(
                _config,
                _root,
                _config.TankDeathExplosionVfxPrefab,
                "Tank Death Explosion VFX",
                position,
                Vector3.up,
                _config.DeathExplosionColor,
                _config.VfxScale * 1.6f,
                _config.DeathExplosionLifetime);

            var wreck = CreateWreckMarker(position, rotation);
            CreateSmokeMarker(wreck, position, rotation);
        }

        private GameObject CreateWreckMarker(Vector3 position, Quaternion rotation)
        {
            if (_config.WreckMarkerPrefab != null)
            {
                return CombatVfxUtility.InstantiateConfiguredPrefab(
                    _config,
                    _config.WreckMarkerPrefab,
                    "Tank Wreck Marker",
                    position,
                    Quaternion.Euler(0f, rotation.eulerAngles.y, 0f),
                    _root,
                    _config.WreckScale,
                    _config.WreckLifetime);
            }

            var root = new GameObject("Tank Wreck Marker");
            root.layer = CombatVfxUtility.IgnoreRaycastLayer;
            root.transform.SetParent(_root, true);
            root.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, rotation.eulerAngles.y, 0f));

            CombatVfxUtility.CreateChildPrimitive(
                root.transform,
                "Wreck Hull",
                PrimitiveType.Cube,
                new Vector3(0f, 0.08f, 0f),
                new Vector3(1.05f, 0.12f, 1.25f) * _config.WreckScale,
                _config.WreckColor);

            var view = root.AddComponent<WreckMarkerView>();
            view.Play(_config.WreckLifetime);
            return root;
        }

        private void CreateSmokeMarker(GameObject wreck, Vector3 position, Quaternion rotation)
        {
            var smokePosition = position + _config.WreckSmokeOffset;
            if (_config.SmokeVfxPrefab != null)
            {
                var parent = wreck != null ? wreck.transform : _root;
                var smoke = CombatVfxUtility.InstantiateConfiguredPrefab(
                    _config,
                    _config.SmokeVfxPrefab,
                    "Tank Death Smoke VFX",
                    smokePosition,
                    Quaternion.Euler(0f, rotation.eulerAngles.y, 0f),
                    parent,
                    _config.VfxScale,
                    _config.SmokeLifetime);

                if (wreck != null)
                {
                    smoke.transform.localPosition = _config.WreckSmokeOffset;
                }

                return;
            }

            CombatVfxUtility.CreateVfxOrFallback(
                _config,
                _root,
                null,
                "Tank Death Smoke VFX",
                smokePosition,
                Vector3.up,
                _config.SmokeColor,
                _config.VfxScale * 1.4f,
                _config.SmokeLifetime);
        }
    }
}
