using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;
using RicochetTanks.Features.UI.Views;

namespace RicochetTanks.Features.UI.Presenters
{
    public sealed class RoomPresenter : IDisposable
    {
        private readonly RoomView _view;
        private readonly IScreenService _screenService;
        private readonly IRoomService _roomService;

        private bool _isReady;

        public RoomPresenter(RoomView view, IScreenService screenService, IRoomService roomService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));

            _view.ReadyClicked += OnReadyClicked;
            _view.StartClicked += OnStartClicked;
            _view.LeaveClicked += OnLeaveClicked;
            _roomService.RoomChanged += OnRoomChanged;

            OnRoomChanged(_roomService.CurrentRoom);
        }

        public void Dispose()
        {
            _view.ReadyClicked -= OnReadyClicked;
            _view.StartClicked -= OnStartClicked;
            _view.LeaveClicked -= OnLeaveClicked;
            _roomService.RoomChanged -= OnRoomChanged;
        }

        private void OnReadyClicked()
        {
            _isReady = !_isReady;
            _roomService.SetReady(_isReady);
        }

        private void OnStartClicked()
        {
            _screenService.SetState(UIFlowState.LoadingMatch);
            _roomService.StartMatch();
            _screenService.ShowGameplayHud();
        }

        private void OnLeaveClicked()
        {
            _roomService.LeaveRoom();
            _screenService.ShowLobby();
        }

        private void OnRoomChanged(RoomSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            _isReady = snapshot.IsLocalReady;
            _view.DisplayRoom(snapshot);
        }
    }
}
