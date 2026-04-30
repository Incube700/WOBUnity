using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;
using RicochetTanks.Features.UI.Views;

namespace RicochetTanks.Features.UI.Presenters
{
    public sealed class GameplayHudPresenter : IDisposable
    {
        private readonly GameplayHudView _view;
        private readonly IScreenService _screenService;
        private readonly IRoomService _roomService;
        private readonly IConnectionStatusService _connectionStatusService;

        public GameplayHudPresenter(
            GameplayHudView view,
            IScreenService screenService,
            IRoomService roomService,
            IConnectionStatusService connectionStatusService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));

            _view.FinishClicked += OnFinishClicked;
            _view.LeaveClicked += OnLeaveClicked;
            _roomService.GameplayHudChanged += OnGameplayHudChanged;
            _connectionStatusService.StatusChanged += OnConnectionStatusChanged;

            _view.DisplayHud(_roomService.CurrentHud);
            _view.DisplayConnection(_connectionStatusService.CurrentStatus);
        }

        public void Dispose()
        {
            _view.FinishClicked -= OnFinishClicked;
            _view.LeaveClicked -= OnLeaveClicked;
            _roomService.GameplayHudChanged -= OnGameplayHudChanged;
            _connectionStatusService.StatusChanged -= OnConnectionStatusChanged;
        }

        private void OnFinishClicked()
        {
            _roomService.FinishMatch(MatchResultType.Victory);
            _screenService.ShowResult();
        }

        private void OnLeaveClicked()
        {
            _roomService.LeaveRoom();
            _screenService.ShowLobby();
        }

        private void OnGameplayHudChanged(GameplayHudSnapshot hud)
        {
            _view.DisplayHud(hud);
        }

        private void OnConnectionStatusChanged(ConnectionStatusSnapshot status)
        {
            _view.DisplayConnection(status);
        }
    }
}
