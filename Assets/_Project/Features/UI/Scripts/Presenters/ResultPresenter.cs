using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;
using RicochetTanks.Features.UI.Views;

namespace RicochetTanks.Features.UI.Presenters
{
    public sealed class ResultPresenter : IDisposable
    {
        private readonly ResultView _view;
        private readonly IScreenService _screenService;
        private readonly IRoomService _roomService;
        private readonly IConnectionStatusService _connectionStatusService;

        public ResultPresenter(
            ResultView view,
            IScreenService screenService,
            IRoomService roomService,
            IConnectionStatusService connectionStatusService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));

            _view.PlayAgainClicked += OnPlayAgainClicked;
            _view.BackToMenuClicked += OnBackToMenuClicked;
            _roomService.ResultChanged += OnResultChanged;

            _view.DisplayResult(_roomService.CurrentResult);
        }

        public void Dispose()
        {
            _view.PlayAgainClicked -= OnPlayAgainClicked;
            _view.BackToMenuClicked -= OnBackToMenuClicked;
            _roomService.ResultChanged -= OnResultChanged;
        }

        private void OnPlayAgainClicked()
        {
            if (_roomService.CurrentRoom != null)
            {
                _roomService.StartMatch();
                _screenService.ShowGameplayHud();
                return;
            }

            _screenService.ShowLobby();
        }

        private void OnBackToMenuClicked()
        {
            _roomService.LeaveRoom();
            _connectionStatusService.Disconnect();
            _screenService.ShowMainMenu();
        }

        private void OnResultChanged(ResultSnapshot result)
        {
            _view.DisplayResult(result);
        }
    }
}
