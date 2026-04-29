using RicochetTanks.Configs;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    internal static class CombatVfxUtility
    {
        public const int IgnoreRaycastLayer = 2;

        public static GameObject InstantiateConfiguredPrefab(
            CombatVfxConfig config,
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

        public static void CreateVfxOrFallback(
            CombatVfxConfig config,
            Transform root,
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
                    config,
                    prefab,
                    objectName,
                    point + ResolveNormal(normal) * config.EffectNormalOffset,
                    ResolveRotation(normal),
                    root,
                    scale,
                    lifetime);
                return;
            }

            CreateBurst(config, root, objectName, point, normal, color, scale, lifetime);
        }

        public static GameObject CreateChildPrimitive(
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

        public static Material CreateMaterial(Color color)
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

        public static Transform FindDescendant(Transform root, string objectName)
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

        private static void CreateBurst(
            CombatVfxConfig config,
            Transform root,
            string objectName,
            Vector3 point,
            Vector3 normal,
            Color color,
            float scale,
            float lifetime)
        {
            var instance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            instance.name = objectName;
            instance.layer = IgnoreRaycastLayer;
            instance.transform.SetParent(root, true);
            instance.transform.position = point + ResolveNormal(normal) * config.EffectNormalOffset;
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
    }
}
