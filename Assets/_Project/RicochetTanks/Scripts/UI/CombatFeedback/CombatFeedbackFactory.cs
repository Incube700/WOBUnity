using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    public sealed class CombatFeedbackFactory
    {
        private readonly GameObject _healthBarPrefab;
        private readonly GameObject _floatingHitTextPrefab;
        private readonly Transform _root;
        private readonly Camera _camera;

        private bool _didWarnMissingHealthBarPrefab;
        private bool _didWarnMissingFloatingTextPrefab;
        private bool _didWarnInvalidHealthBarPrefab;
        private bool _didWarnInvalidFloatingTextPrefab;

        public CombatFeedbackFactory(
            GameObject healthBarPrefab,
            GameObject floatingHitTextPrefab,
            Transform root,
            Camera camera)
        {
            _healthBarPrefab = healthBarPrefab;
            _floatingHitTextPrefab = floatingHitTextPrefab;
            _root = root;
            _camera = camera;
        }

        public TankHealthBarView CreateHealthBar(TankFacade tank)
        {
            if (_healthBarPrefab == null)
            {
                WarnMissingHealthBarPrefab();
                return null;
            }

            if (tank == null)
            {
                return null;
            }

            var instance = Object.Instantiate(_healthBarPrefab, _root);
            instance.name = $"{tank.name} Health Bar";

            var view = instance.GetComponent<TankHealthBarView>();
            if (view == null)
            {
                WarnInvalidHealthBarPrefab();
                Object.Destroy(instance);
                return null;
            }

            view.SetCamera(_camera);
            view.Follow(tank.transform);
            return view;
        }

        public FloatingHitTextView CreateFloatingHitText(Vector3 worldPosition)
        {
            if (_floatingHitTextPrefab == null)
            {
                WarnMissingFloatingTextPrefab();
                return null;
            }

            var instance = Object.Instantiate(_floatingHitTextPrefab, worldPosition, Quaternion.identity, _root);
            instance.name = "Floating Hit Text";

            var view = instance.GetComponent<FloatingHitTextView>();
            if (view == null)
            {
                WarnInvalidFloatingTextPrefab();
                Object.Destroy(instance);
                return null;
            }

            view.SetCamera(_camera);
            return view;
        }

        private void WarnMissingHealthBarPrefab()
        {
            if (_didWarnMissingHealthBarPrefab)
            {
                return;
            }

            Debug.LogWarning("Combat feedback health bar prefab is missing. Assign WorldHealthBarPrefab on GameplayEntryPoint or create it from Tools/Ricochet Tanks/Create Combat Feedback Prefabs.");
            _didWarnMissingHealthBarPrefab = true;
        }

        private void WarnMissingFloatingTextPrefab()
        {
            if (_didWarnMissingFloatingTextPrefab)
            {
                return;
            }

            Debug.LogWarning("Combat feedback floating hit text prefab is missing. Assign FloatingHitTextPrefab on GameplayEntryPoint or create it from Tools/Ricochet Tanks/Create Combat Feedback Prefabs.");
            _didWarnMissingFloatingTextPrefab = true;
        }

        private void WarnInvalidHealthBarPrefab()
        {
            if (_didWarnInvalidHealthBarPrefab)
            {
                return;
            }

            Debug.LogWarning("Combat feedback health bar prefab is missing a TankHealthBarView component.");
            _didWarnInvalidHealthBarPrefab = true;
        }

        private void WarnInvalidFloatingTextPrefab()
        {
            if (_didWarnInvalidFloatingTextPrefab)
            {
                return;
            }

            Debug.LogWarning("Combat feedback floating hit text prefab is missing a FloatingHitTextView component.");
            _didWarnInvalidFloatingTextPrefab = true;
        }
    }
}
