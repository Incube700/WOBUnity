using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;
using RicochetTanks.Features.UI.Views;

namespace RicochetTanks.Features.UI.Presenters
{
    public sealed class LobbyPresenter : IDisposable
    {
        private readonly LobbyView _view;
        private readonly IScreenService _screenService;
        private readonly ILobbyService _lobbyService;
        private readonly IRoomService _roomService;
        private readonly IConnectionStatusService _connectionStatusService;

        public LobbyPresenter(
            LobbyView view,
            IScreenService screenService,
            ILobbyService lobbyService,
            IRoomService roomService,
            IConnectionStatusService connectionStatusService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            _lobbyService = lobbyService ?? throw new ArgumentNullException(nameof(lobbyService));
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));

            _view.QuickMatchClicked += OnQuickMatchClicked;
            _view.CreateRoomRequested += OnCreateRoomRequested;
            _view.JoinByCodeRequested += OnJoinByCodeRequested;
            _view.RefreshClicked += OnRefreshClicked;
            _view.BackClicked += OnBackClicked;
            _view.RoomSelected += OnRoomSelected;
            _lobbyService.LobbyChanged += OnLobbyChanged;
            _connectionStatusService.StatusChanged += OnConnectionStatusChanged;

            _view.DisplayLobby(_lobbyService.CurrentLobby);
            _view.SetStatus(_connectionStatusService.CurrentStatus.StatusText);
        }

        public void Dispose()
        {
            _view.QuickMatchClicked -= OnQuickMatchClicked;
            _view.CreateRoomRequested -= OnCreateRoomRequested;
            _view.JoinByCodeRequested -= OnJoinByCodeRequested;
            _view.RefreshClicked -= OnRefreshClicked;
            _view.BackClicked -= OnBackClicked;
            _view.RoomSelected -= OnRoomSelected;
            _lobbyService.LobbyChanged -= OnLobbyChanged;
            _connectionStatusService.StatusChanged -= OnConnectionStatusChanged;
        }

        private void OnQuickMatchClicked()
        {
            OpenRoom(_lobbyService.QuickMatch());
        }

        private void OnCreateRoomRequested(string roomName)
        {
            OpenRoom(_lobbyService.CreateRoom(roomName));
        }

        private void OnJoinByCodeRequested(string roomCode)
        {
            OpenRoom(_lobbyService.JoinRoom(roomCode));
        }

        private void OnRefreshClicked()
        {
            _lobbyService.RefreshRooms();
        }

        private void OnBackClicked()
        {
            _lobbyService.LeaveLobby();
            _connectionStatusService.Disconnect();
            _screenService.ShowMainMenu();
        }

        private void OnRoomSelected(string roomCode)
        {
            OpenRoom(_lobbyService.JoinRoom(roomCode));
        }

        private void OnLobbyChanged(LobbySnapshot snapshot)
        {
            _view.DisplayLobby(snapshot);
        }

        private void OnConnectionStatusChanged(ConnectionStatusSnapshot status)
        {
            _view.SetStatus(status.StatusText);
        }

        private void OpenRoom(RoomSnapshot room)
        {
            if (room == null)
            {
                return;
            }

            _roomService.OpenRoom(room);
            _screenService.ShowRoom();
        }
    }
}
