using System;
using System.Collections.Generic;
using RicochetTanks.Features.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Views
{
    public sealed class LobbyView : MonoBehaviour
    {
        [SerializeField] private Button _quickMatchButton;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _joinByCodeButton;
        [SerializeField] private Button _refreshButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private InputField _roomNameInput;
        [SerializeField] private InputField _roomCodeInput;
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _placeholderText;
        [SerializeField] private Transform _roomListRoot;
        [SerializeField] private RoomCardView _roomCardPrefab;

        private readonly List<RoomCardView> _roomCards = new List<RoomCardView>();
        private bool _isSubscribed;

        public event Action QuickMatchClicked;
        public event Action<string> CreateRoomRequested;
        public event Action<string> JoinByCodeRequested;
        public event Action RefreshClicked;
        public event Action BackClicked;
        public event Action<string> RoomSelected;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(
            Button quickMatchButton,
            Button createRoomButton,
            Button joinByCodeButton,
            Button refreshButton,
            Button backButton,
            InputField roomNameInput,
            InputField roomCodeInput,
            Text statusText,
            Text placeholderText,
            Transform roomListRoot,
            RoomCardView roomCardPrefab)
        {
            Unsubscribe();
            _quickMatchButton = quickMatchButton;
            _createRoomButton = createRoomButton;
            _joinByCodeButton = joinByCodeButton;
            _refreshButton = refreshButton;
            _backButton = backButton;
            _roomNameInput = roomNameInput;
            _roomCodeInput = roomCodeInput;
            _statusText = statusText;
            _placeholderText = placeholderText;
            _roomListRoot = roomListRoot;
            _roomCardPrefab = roomCardPrefab;
            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        public void DisplayLobby(LobbySnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            SetStatus(snapshot.StatusText);
            DisplayRooms(snapshot.Rooms);
        }

        public void SetStatus(string status)
        {
            if (_statusText != null)
            {
                _statusText.text = status;
            }
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        private void DisplayRooms(IReadOnlyList<RoomSummary> rooms)
        {
            ClearRooms();

            if (_placeholderText != null)
            {
                _placeholderText.gameObject.SetActive(rooms == null || rooms.Count == 0);
            }

            if (rooms == null || _roomListRoot == null || _roomCardPrefab == null)
            {
                return;
            }

            for (var i = 0; i < rooms.Count; i++)
            {
                var card = Instantiate(_roomCardPrefab, _roomListRoot);
                card.gameObject.SetActive(true);
                card.Display(rooms[i]);
                card.Selected += OnRoomCardSelected;
                _roomCards.Add(card);
            }
        }

        private void ClearRooms()
        {
            for (var i = 0; i < _roomCards.Count; i++)
            {
                var card = _roomCards[i];
                if (card == null)
                {
                    continue;
                }

                card.Selected -= OnRoomCardSelected;
                Destroy(card.gameObject);
            }

            _roomCards.Clear();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (_quickMatchButton != null)
            {
                _quickMatchButton.onClick.AddListener(OnQuickMatchButtonClicked);
            }

            if (_createRoomButton != null)
            {
                _createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
            }

            if (_joinByCodeButton != null)
            {
                _joinByCodeButton.onClick.AddListener(OnJoinByCodeButtonClicked);
            }

            if (_refreshButton != null)
            {
                _refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            }

            if (_backButton != null)
            {
                _backButton.onClick.AddListener(OnBackButtonClicked);
            }

            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_quickMatchButton != null)
            {
                _quickMatchButton.onClick.RemoveListener(OnQuickMatchButtonClicked);
            }

            if (_createRoomButton != null)
            {
                _createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClicked);
            }

            if (_joinByCodeButton != null)
            {
                _joinByCodeButton.onClick.RemoveListener(OnJoinByCodeButtonClicked);
            }

            if (_refreshButton != null)
            {
                _refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
            }

            if (_backButton != null)
            {
                _backButton.onClick.RemoveListener(OnBackButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnQuickMatchButtonClicked()
        {
            QuickMatchClicked?.Invoke();
        }

        private void OnCreateRoomButtonClicked()
        {
            var roomName = _roomNameInput != null ? _roomNameInput.text : string.Empty;
            CreateRoomRequested?.Invoke(roomName);
        }

        private void OnJoinByCodeButtonClicked()
        {
            var roomCode = _roomCodeInput != null ? _roomCodeInput.text : string.Empty;
            JoinByCodeRequested?.Invoke(roomCode);
        }

        private void OnRefreshButtonClicked()
        {
            RefreshClicked?.Invoke();
        }

        private void OnBackButtonClicked()
        {
            BackClicked?.Invoke();
        }

        private void OnRoomCardSelected(string roomCode)
        {
            RoomSelected?.Invoke(roomCode);
        }
    }
}
