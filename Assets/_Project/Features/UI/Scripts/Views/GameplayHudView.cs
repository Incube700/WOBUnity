using System;
using RicochetTanks.Features.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Views
{
    public sealed class GameplayHudView : MonoBehaviour
    {
        [SerializeField] private Slider _playerHealthBar;
        [SerializeField] private Text _playerHealthText;
        [SerializeField] private Slider _enemyHealthBar;
        [SerializeField] private Text _enemyHealthText;
        [SerializeField] private Slider _reloadBar;
        [SerializeField] private Text _reloadText;
        [SerializeField] private Text _lastHitResultText;
        [SerializeField] private Text _ricochetCountText;
        [SerializeField] private Text _connectionText;
        [SerializeField] private Button _finishButton;
        [SerializeField] private Button _leaveButton;

        private bool _isSubscribed;

        public event Action FinishClicked;
        public event Action LeaveClicked;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(
            Slider playerHealthBar,
            Text playerHealthText,
            Slider enemyHealthBar,
            Text enemyHealthText,
            Slider reloadBar,
            Text reloadText,
            Text lastHitResultText,
            Text ricochetCountText,
            Text connectionText,
            Button finishButton,
            Button leaveButton)
        {
            Unsubscribe();
            _playerHealthBar = playerHealthBar;
            _playerHealthText = playerHealthText;
            _enemyHealthBar = enemyHealthBar;
            _enemyHealthText = enemyHealthText;
            _reloadBar = reloadBar;
            _reloadText = reloadText;
            _lastHitResultText = lastHitResultText;
            _ricochetCountText = ricochetCountText;
            _connectionText = connectionText;
            _finishButton = finishButton;
            _leaveButton = leaveButton;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        public void DisplayHud(GameplayHudSnapshot hud)
        {
            if (hud == null)
            {
                return;
            }

            SetNormalizedValue(_playerHealthBar, hud.PlayerHp, hud.PlayerMaxHp);
            SetText(_playerHealthText, "Player HP: " + Mathf.CeilToInt(hud.PlayerHp) + "/" + Mathf.CeilToInt(hud.PlayerMaxHp));
            SetNormalizedValue(_enemyHealthBar, hud.EnemyHp, hud.EnemyMaxHp);
            SetText(_enemyHealthText, "Enemy HP: " + Mathf.CeilToInt(hud.EnemyHp) + "/" + Mathf.CeilToInt(hud.EnemyMaxHp));
            SetSliderValue(_reloadBar, Mathf.Clamp01(hud.ReloadProgress));
            SetText(_reloadText, "Reload: " + Mathf.RoundToInt(Mathf.Clamp01(hud.ReloadProgress) * 100f) + "%");
            SetText(_lastHitResultText, "Last hit: " + hud.LastHitResult);
            SetText(_ricochetCountText, "Ricochets: " + hud.RicochetCount);
        }

        public void DisplayConnection(ConnectionStatusSnapshot connection)
        {
            if (connection == null)
            {
                return;
            }

            SetText(_connectionText, connection.StatusText + " | Ping: " + connection.PingText);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (_finishButton != null)
            {
                _finishButton.onClick.AddListener(OnFinishButtonClicked);
            }

            if (_leaveButton != null)
            {
                _leaveButton.onClick.AddListener(OnLeaveButtonClicked);
            }

            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_finishButton != null)
            {
                _finishButton.onClick.RemoveListener(OnFinishButtonClicked);
            }

            if (_leaveButton != null)
            {
                _leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnFinishButtonClicked()
        {
            FinishClicked?.Invoke();
        }

        private void OnLeaveButtonClicked()
        {
            LeaveClicked?.Invoke();
        }

        private static void SetNormalizedValue(Slider slider, float current, float max)
        {
            if (slider == null)
            {
                return;
            }

            var value = max > 0f ? current / max : 0f;
            SetSliderValue(slider, Mathf.Clamp01(value));
        }

        private static void SetSliderValue(Slider slider, float value)
        {
            if (slider == null)
            {
                return;
            }

            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = value;
        }

        private static void SetText(Text target, string value)
        {
            if (target != null)
            {
                target.text = value;
            }
        }
    }
}
