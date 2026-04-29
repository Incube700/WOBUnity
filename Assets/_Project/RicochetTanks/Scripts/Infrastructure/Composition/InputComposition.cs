using RicochetTanks.Input;
using RicochetTanks.Input.Desktop;
using RicochetTanks.Input.Mobile;
using UnityEngine;

namespace RicochetTanks.Infrastructure.Composition
{
    public sealed class InputComposition
    {
        private readonly MonoBehaviour _owner;
        private readonly TankInputMode _inputMode;
        private readonly GameObject _mobileControlsPrefab;
        private readonly DesktopInputReader _desktopInputReader;

        private MobileInputReader _mobileInputReader;
        private MobileControlsView _mobileControlsView;

        public InputComposition(
            MonoBehaviour owner,
            TankInputMode inputMode,
            GameObject mobileControlsPrefab,
            DesktopInputReader desktopInputReader)
        {
            _owner = owner;
            _inputMode = inputMode;
            _mobileControlsPrefab = mobileControlsPrefab;
            _desktopInputReader = desktopInputReader;
        }

        public ITankInputReader CreateActiveInputReader()
        {
            var desktopInputReader = EnsureDesktopInputReader();
            _mobileControlsView = _mobileControlsView != null ? _mobileControlsView : ResolveExistingMobileControlsView();

            if (ResolveInputMode() == TankInputMode.Mobile)
            {
                return EnsureMobileInputReader();
            }

            SetMobileControlsVisible(false);
            return desktopInputReader;
        }

        private TankInputMode ResolveInputMode()
        {
            if (_inputMode != TankInputMode.Auto)
            {
                return _inputMode;
            }

            return Application.isMobilePlatform ? TankInputMode.Mobile : TankInputMode.Desktop;
        }

        private DesktopInputReader EnsureDesktopInputReader()
        {
            if (_desktopInputReader != null)
            {
                return _desktopInputReader;
            }

            if (_owner.TryGetComponent<DesktopInputReader>(out var existingInputReader))
            {
                return existingInputReader;
            }

            return _owner.gameObject.AddComponent<DesktopInputReader>();
        }

        private ITankInputReader EnsureMobileInputReader()
        {
            if (_mobileInputReader == null)
            {
                _mobileInputReader = _owner.GetComponent<MobileInputReader>();
            }

            if (_mobileInputReader == null)
            {
                _mobileInputReader = _owner.gameObject.AddComponent<MobileInputReader>();
            }

            if (_mobileControlsView == null)
            {
                _mobileControlsView = ResolveExistingMobileControlsView() ?? CreateMobileControlsView();
            }

            SetMobileControlsVisible(true);
            _mobileInputReader.Configure(_mobileControlsView);
            return _mobileInputReader;
        }

        private MobileControlsView ResolveExistingMobileControlsView()
        {
            return _owner.GetComponentInChildren<MobileControlsView>(true);
        }

        private MobileControlsView CreateMobileControlsView()
        {
            if (_mobileControlsPrefab != null)
            {
                var controlsObject = Object.Instantiate(_mobileControlsPrefab, _owner.transform);
                controlsObject.name = _mobileControlsPrefab.name;

                if (controlsObject.TryGetComponent<MobileControlsView>(out var prefabView))
                {
                    return prefabView;
                }

                var childView = controlsObject.GetComponentInChildren<MobileControlsView>(true);
                if (childView != null)
                {
                    return childView;
                }

                Debug.LogWarning("[MOBILE_INPUT] Mobile controls prefab has no MobileControlsView. Runtime controls will be created.");
                Object.Destroy(controlsObject);
            }

            return MobileControlsView.CreateDefault("MobileControlsCanvas", _owner.transform);
        }

        private void SetMobileControlsVisible(bool isVisible)
        {
            if (_mobileControlsView != null)
            {
                _mobileControlsView.gameObject.SetActive(isVisible);
            }
        }
    }
}
