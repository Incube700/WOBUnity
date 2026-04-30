using System;
using RicochetTanks.Features.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Views
{
    public sealed class RoomCardView : MonoBehaviour
    {
        [SerializeField] private Button _selectButton;
        [SerializeField] private Text _roomNameText;
        [SerializeField] private Text _roomCodeText;
        [SerializeField] private Text _playersText;
        [SerializeField] private Text _regionText;

        private string _roomCode;
        private bool _isSubscribed;

        public event Action<string> Selected;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(Button selectButton, Text roomNameText, Text roomCodeText, Text playersText, Text regionText)
        {
            Unsubscribe();
            _selectButton = selectButton;
            _roomNameText = roomNameText;
            _roomCodeText = roomCodeText;
            _playersText = playersText;
            _regionText = regionText;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        public void Display(RoomSummary room)
        {
            if (room == null)
            {
                return;
            }

            _roomCode = room.RoomCode;
            SetText(_roomNameText, room.RoomName);
            SetText(_roomCodeText, room.RoomCode);
            SetText(_playersText, room.PlayerCount + "/" + room.MaxPlayers);
            SetText(_regionText, room.Region);
        }

        private void Subscribe()
        {
            if (_isSubscribed || _selectButton == null)
            {
                return;
            }

            _selectButton.onClick.AddListener(OnSelectButtonClicked);
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_selectButton != null)
            {
                _selectButton.onClick.RemoveListener(OnSelectButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnSelectButtonClicked()
        {
            Selected?.Invoke(_roomCode);
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
