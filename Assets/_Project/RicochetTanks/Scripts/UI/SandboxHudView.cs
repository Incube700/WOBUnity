using System;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI
{
    public class SandboxHudView : MonoBehaviour
    {
        [SerializeField] private Text _playerHpText;
        [SerializeField] private Text _enemyHpText;
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

        public void Configure(Text playerHpText, Text enemyHpText, Button restartButton)
        {
            Unsubscribe();
            _playerHpText = playerHpText;
            _enemyHpText = enemyHpText;
            _restartButton = restartButton;
            Subscribe();
        }

        public void SetPlayerHp(int currentHp, int maxHp)
        {
            if (_playerHpText != null)
            {
                _playerHpText.text = $"Player HP: {currentHp}/{maxHp}";
            }
        }

        public void SetEnemyHp(int currentHp, int maxHp)
        {
            if (_enemyHpText != null)
            {
                _enemyHpText.text = $"Enemy HP: {currentHp}/{maxHp}";
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
    }
}
