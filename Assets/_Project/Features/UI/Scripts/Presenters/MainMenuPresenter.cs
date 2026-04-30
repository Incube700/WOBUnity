using System;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Services.Interfaces;
using RicochetTanks.Features.UI.Views;

namespace RicochetTanks.Features.UI.Presenters
{
    public sealed class MainMenuPresenter : IDisposable
    {
        private readonly MainMenuView _view;
        private readonly IScreenService _screenService;
        private readonly IConnectionStatusService _connectionStatusService;

        public MainMenuPresenter(
            MainMenuView view,
            IScreenService screenService,
            IConnectionStatusService connectionStatusService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _screenService = screenService ?? throw new ArgumentNullException(nameof(screenService));
            _connectionStatusService = connectionStatusService ?? throw new ArgumentNullException(nameof(connectionStatusService));

            _view.PlayClicked += OnPlayClicked;
            _view.OnlineClicked += OnOnlineClicked;
            _view.QuitClicked += OnQuitClicked;
            _screenService.StateChanged += OnScreenStateChanged;

            _view.DisplayState(_screenService.CurrentState);
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
            _view.OnlineClicked -= OnOnlineClicked;
            _view.QuitClicked -= OnQuitClicked;
            _screenService.StateChanged -= OnScreenStateChanged;
        }

        private void OnPlayClicked()
        {
            _screenService.ShowGameplayHud();
        }

        private void OnOnlineClicked()
        {
            _screenService.SetState(UIFlowState.Connecting);
            _connectionStatusService.Connect();
            _screenService.ShowLobby();
        }

        private void OnQuitClicked()
        {
            _connectionStatusService.Disconnect();
            _screenService.ShowMainMenu();
        }

        private void OnScreenStateChanged(UIFlowState state)
        {
            _view.DisplayState(state);
        }
    }
}
