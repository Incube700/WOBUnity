using System;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton;

        private bool _isSubscribed;

        public event Action PlayClicked;
        public event Action QuitClicked;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(Button playButton, Button quitButton)
        {
            Unsubscribe();
            _playButton = playButton;
            _quitButton = quitButton;
            Subscribe();
        }

        private void Subscribe()
        {
            if (_isSubscribed || _playButton == null || _quitButton == null)
            {
                return;
            }

            _playButton.onClick.AddListener(OnPlayButtonClicked);
            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_playButton != null)
            {
                _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnPlayButtonClicked()
        {
            PlayClicked?.Invoke();
        }

        private void OnQuitButtonClicked()
        {
            QuitClicked?.Invoke();
        }
    }
}
