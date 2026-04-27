using System;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.Sandbox
{
    public class SandboxHudView : MonoBehaviour
    {
        [SerializeField] private Text _playerHpText;
        [SerializeField] private Text _enemyHpText;
        [SerializeField] private Text _lastHitText;
        [SerializeField] private Text _roundResultText;
        [SerializeField] private Text _controlsHintText;
        [SerializeField] private Button _restartButton;

        private bool _isSubscribed;

        public event Action RestartClicked;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(
            Text playerHpText,
            Text enemyHpText,
            Text lastHitText,
            Text roundResultText,
            Text controlsHintText,
            Button restartButton)
        {
            Unsubscribe();
            _playerHpText = playerHpText;
            _enemyHpText = enemyHpText;
            _lastHitText = lastHitText;
            _roundResultText = roundResultText;
            _controlsHintText = controlsHintText;
            _restartButton = restartButton;
            Subscribe();
        }

        public void SetPlayerHp(float currentHp, float maxHp)
        {
            if (_playerHpText != null)
            {
                _playerHpText.text = $"Player HP: {Format(currentHp)}/{Format(maxHp)}";
            }
        }

        public void SetEnemyHp(float currentHp, float maxHp)
        {
            if (_enemyHpText != null)
            {
                _enemyHpText.text = $"Enemy HP: {Format(currentHp)}/{Format(maxHp)}";
            }
        }

        public void SetLastHitResult(string text)
        {
            if (_lastHitText != null)
            {
                _lastHitText.text = text;
            }
        }

        public void SetRoundResult(string text)
        {
            if (_roundResultText != null)
            {
                _roundResultText.text = text;
            }
        }

        public void SetControlsHint(string text)
        {
            if (_controlsHintText != null)
            {
                _controlsHintText.text = text;
            }
        }

        private void Subscribe()
        {
            if (_isSubscribed || _restartButton == null)
            {
                return;
            }

            _restartButton.onClick.AddListener(OnRestartButtonClicked);
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || _restartButton == null)
            {
                return;
            }

            _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            _isSubscribed = false;
        }

        private void OnRestartButtonClicked()
        {
            RestartClicked?.Invoke();
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
