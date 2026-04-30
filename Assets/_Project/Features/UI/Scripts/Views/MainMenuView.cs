using System;
using RicochetTanks.Features.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Views
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _onlineButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Text _stateText;

        private bool _isSubscribed;

        public event Action PlayClicked;
        public event Action OnlineClicked;
        public event Action QuitClicked;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(Button playButton, Button onlineButton, Button quitButton, Text stateText)
        {
            Unsubscribe();
            _playButton = playButton;
            _onlineButton = onlineButton;
            _quitButton = quitButton;
            _stateText = stateText;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        public void DisplayState(UIFlowState state)
        {
            if (_stateText != null)
            {
                _stateText.text = state.ToString();
            }
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

            if (_playButton != null)
            {
                _playButton.onClick.AddListener(OnPlayButtonClicked);
            }

            if (_onlineButton != null)
            {
                _onlineButton.onClick.AddListener(OnOnlineButtonClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitButtonClicked);
            }

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

            if (_onlineButton != null)
            {
                _onlineButton.onClick.RemoveListener(OnOnlineButtonClicked);
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

        private void OnOnlineButtonClicked()
        {
            OnlineClicked?.Invoke();
        }

        private void OnQuitButtonClicked()
        {
            QuitClicked?.Invoke();
        }
    }
}
