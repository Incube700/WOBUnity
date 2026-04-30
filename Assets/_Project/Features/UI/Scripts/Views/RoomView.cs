using System;
using System.Text;
using RicochetTanks.Features.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Views
{
    public sealed class RoomView : MonoBehaviour
    {
        [SerializeField] private Text _roomNameText;
        [SerializeField] private Text _roomCodeText;
        [SerializeField] private Text _regionText;
        [SerializeField] private Text _stateText;
        [SerializeField] private Text _playerSlotsText;
        [SerializeField] private Button _readyButton;
        [SerializeField] private Text _readyButtonText;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _leaveButton;

        private bool _isSubscribed;

        public event Action ReadyClicked;
        public event Action StartClicked;
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
            Text roomNameText,
            Text roomCodeText,
            Text regionText,
            Text stateText,
            Text playerSlotsText,
            Button readyButton,
            Text readyButtonText,
            Button startButton,
            Button leaveButton)
        {
            Unsubscribe();
            _roomNameText = roomNameText;
            _roomCodeText = roomCodeText;
            _regionText = regionText;
            _stateText = stateText;
            _playerSlotsText = playerSlotsText;
            _readyButton = readyButton;
            _readyButtonText = readyButtonText;
            _startButton = startButton;
            _leaveButton = leaveButton;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        public void DisplayRoom(RoomSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            SetText(_roomNameText, snapshot.RoomName);
            SetText(_roomCodeText, "Code: " + snapshot.RoomCode);
            SetText(_regionText, "Region: " + snapshot.Region);
            SetText(_stateText, snapshot.StateText);
            SetText(_playerSlotsText, BuildPlayerSlots(snapshot));

            if (_readyButtonText != null)
            {
                _readyButtonText.text = snapshot.IsLocalReady ? "Ready: On" : "Ready";
            }

            if (_startButton != null)
            {
                _startButton.gameObject.SetActive(snapshot.IsHost);
                _startButton.interactable = snapshot.CanStart;
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

            if (_readyButton != null)
            {
                _readyButton.onClick.AddListener(OnReadyButtonClicked);
            }

            if (_startButton != null)
            {
                _startButton.onClick.AddListener(OnStartButtonClicked);
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

            if (_readyButton != null)
            {
                _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
            }

            if (_startButton != null)
            {
                _startButton.onClick.RemoveListener(OnStartButtonClicked);
            }

            if (_leaveButton != null)
            {
                _leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnReadyButtonClicked()
        {
            ReadyClicked?.Invoke();
        }

        private void OnStartButtonClicked()
        {
            StartClicked?.Invoke();
        }

        private void OnLeaveButtonClicked()
        {
            LeaveClicked?.Invoke();
        }

        private static string BuildPlayerSlots(RoomSnapshot snapshot)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < snapshot.MaxPlayers; i++)
            {
                if (i < snapshot.PlayerCount)
                {
                    var player = snapshot.Players[i];
                    builder.Append(i + 1);
                    builder.Append(". ");
                    builder.Append(player.DisplayName);
                    builder.Append(player.IsHost ? " (Host)" : string.Empty);
                    builder.Append(player.IsReady ? " - Ready" : " - Waiting");
                }
                else
                {
                    builder.Append(i + 1);
                    builder.Append(". Empty");
                }

                if (i < snapshot.MaxPlayers - 1)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
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
