using System;
using RicochetTanks.Features.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Views
{
    public sealed class ResultView : MonoBehaviour
    {
        [SerializeField] private Text _headlineText;
        [SerializeField] private Text _detailsText;
        [SerializeField] private Button _playAgainButton;
        [SerializeField] private Button _backToMenuButton;

        private bool _isSubscribed;

        public event Action PlayAgainClicked;
        public event Action BackToMenuClicked;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(Text headlineText, Text detailsText, Button playAgainButton, Button backToMenuButton)
        {
            Unsubscribe();
            _headlineText = headlineText;
            _detailsText = detailsText;
            _playAgainButton = playAgainButton;
            _backToMenuButton = backToMenuButton;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        public void DisplayResult(ResultSnapshot result)
        {
            if (result == null)
            {
                return;
            }

            SetText(_headlineText, result.Headline);
            SetText(_detailsText, result.Details);
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

            if (_playAgainButton != null)
            {
                _playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
            }

            if (_backToMenuButton != null)
            {
                _backToMenuButton.onClick.AddListener(OnBackToMenuButtonClicked);
            }

            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_playAgainButton != null)
            {
                _playAgainButton.onClick.RemoveListener(OnPlayAgainButtonClicked);
            }

            if (_backToMenuButton != null)
            {
                _backToMenuButton.onClick.RemoveListener(OnBackToMenuButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnPlayAgainButtonClicked()
        {
            PlayAgainClicked?.Invoke();
        }

        private void OnBackToMenuButtonClicked()
        {
            BackToMenuClicked?.Invoke();
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
