using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Projectiles;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    public sealed class CombatVfxFactory
    {
        private const int IgnoreRaycastLayer = 2;

        private readonly CombatVfxConfig _config;
        private readonly Transform _root;

        public CombatVfxFactory(CombatVfxConfig config, Transform root)
        {
            _config = config;
            _root = root;
        }

        public void ConfigureProjectileTrail(Projectile projectile)
        {
            if (projectile == null || _config == null)
            {
                return;
            }

            if (!projectile.TryGetComponent<TrailRenderer>(out var trail))
            {
                trail = projectile.gameObject.AddComponent<TrailRenderer>();
            }

            trail.time = _config.ProjectileTrailLifetime;
            trail.startWidth = _config.ProjectileTrailWidth;
            trail.endWidth = 0f;
            trail.startColor = _config.ProjectileTrailStartColor;
            trail.endColor = _config.ProjectileTrailEndColor;
            trail.emitting = true;

            if (trail.material == null)
            {
                var trailMaterial = CreateMaterial(_config.ProjectileTrailStartColor);
                if (trailMaterial != null)
                {
                    trail.material = trailMaterial;
                }
            }
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

        public void CreateTankDeath(Vector3 position, Quaternion rotation)
        {
            if (_config == null)
            {
                return;
            }

            CreateVfxOrFallback(
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
                var wreck = InstantiateConfiguredPrefab(
                    _config.WreckMarkerPrefab,
                    "Tank Wreck Marker",
                    position,
                    Quaternion.Euler(0f, rotation.eulerAngles.y, 0f),
                    _root,
                    _config.WreckScale,
                    _config.WreckLifetime);

                return wreck;
            }

            var root = new GameObject("Tank Wreck Marker");
            root.layer = IgnoreRaycastLayer;
            root.transform.SetParent(_root, true);
            root.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, rotation.eulerAngles.y, 0f));

            CreateChildPrimitive(
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
                var smoke = InstantiateConfiguredPrefab(
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

            CreateVfxOrFallback(
                null,
                "Tank Death Smoke VFX",
                smokePosition,
                Vector3.up,
                _config.SmokeColor,
                _config.VfxScale * 1.4f,
                _config.SmokeLifetime);
        }

        public void PlayShotRecoil(TankFacade owner)
        {
            if (_config == null || owner == null)
            {
                return;
            }

            var target = FindDescendant(owner.transform, "Turret") ?? owner.transform;
            if (!target.TryGetComponent<ShotRecoilView>(out var recoil))
            {
                recoil = target.gameObject.AddComponent<ShotRecoilView>();
            }

            recoil.Play(_config.ShotRecoilDistance, _config.ShotRecoilDuration);
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
            if (prefab != null)
            {
                InstantiateConfiguredPrefab(
                    prefab,
                    objectName,
                    point + ResolveNormal(normal) * _config.EffectNormalOffset,
                    ResolveRotation(normal),
                    _root,
                    scale,
                    lifetime);
                return;
            }

            CreateBurst(objectName, point, normal, color, scale, lifetime);
        }

        private GameObject InstantiateConfiguredPrefab(
            GameObject prefab,
            string objectName,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            float scale,
            float lifetime)
        {
            var instance = Object.Instantiate(prefab, position, rotation, parent);
            instance.name = objectName;
            SetLayerRecursively(instance, IgnoreRaycastLayer);
            DisableColliders(instance);
            instance.transform.localScale = instance.transform.localScale * Mathf.Max(0.01f, scale);

            var lifetimeView = instance.GetComponent<CombatVfxLifetimeView>();
            if (lifetimeView == null)
            {
                lifetimeView = instance.AddComponent<CombatVfxLifetimeView>();
            }

            lifetimeView.Play(lifetime);
            return instance;
        }

        private void CreateBurst(string objectName, Vector3 point, Vector3 normal, Color color, float scale, float lifetime)
        {
            var instance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            instance.name = objectName;
            instance.layer = IgnoreRaycastLayer;
            instance.transform.SetParent(_root, true);
            instance.transform.position = point + ResolveNormal(normal) * _config.EffectNormalOffset;
            instance.transform.localScale = Vector3.one * Mathf.Max(0.01f, scale);

            if (instance.TryGetComponent<Collider>(out var collider))
            {
                DisableCollider(collider);
            }

            if (instance.TryGetComponent<Renderer>(out var renderer))
            {
                var material = CreateMaterial(color);
                if (material != null)
                {
                    renderer.material = material;
                }
            }

            var view = instance.AddComponent<CombatVfxBurstView>();
            view.Play(color, scale, lifetime);
        }

        private static GameObject CreateChildPrimitive(
            Transform parent,
            string objectName,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale,
            Color color)
        {
            var instance = GameObject.CreatePrimitive(primitiveType);
            instance.name = objectName;
            instance.layer = IgnoreRaycastLayer;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = localScale;

            if (instance.TryGetComponent<Collider>(out var collider))
            {
                DisableCollider(collider);
            }

            if (instance.TryGetComponent<Renderer>(out var renderer))
            {
                var material = CreateMaterial(color);
                if (material != null)
                {
                    renderer.material = material;
                }
            }

            return instance;
        }

        private static void DisableCollider(Collider collider)
        {
            collider.enabled = false;
            Object.Destroy(collider);
        }

        private static void DisableColliders(GameObject root)
        {
            var colliders = root.GetComponentsInChildren<Collider>(true);
            for (var index = 0; index < colliders.Length; index++)
            {
                DisableCollider(colliders[index]);
            }
        }

        private static void SetLayerRecursively(GameObject root, int layer)
        {
            root.layer = layer;
            var rootTransform = root.transform;

            for (var index = 0; index < rootTransform.childCount; index++)
            {
                SetLayerRecursively(rootTransform.GetChild(index).gameObject, layer);
            }
        }

        private static Vector3 ResolveNormal(Vector3 normal)
        {
            return normal.sqrMagnitude < 0.001f ? Vector3.up : normal.normalized;
        }

        private static Quaternion ResolveRotation(Vector3 normal)
        {
            var resolvedNormal = ResolveNormal(normal);
            return Quaternion.FromToRotation(Vector3.forward, resolvedNormal);
        }

        private static Transform FindDescendant(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == objectName)
            {
                return root;
            }

            for (var index = 0; index < root.childCount; index++)
            {
                var result = FindDescendant(root.GetChild(index), objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Legacy Shaders/Transparent/Diffuse")
                ?? Shader.Find("Unlit/Transparent")
                ?? Shader.Find("Universal Render Pipeline/Unlit")
                ?? Shader.Find("Unlit/Color")
                ?? Shader.Find("Standard");

            if (shader == null)
            {
                return null;
            }

            var material = new Material(shader);

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }

            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            return material;
        }
    }
}
