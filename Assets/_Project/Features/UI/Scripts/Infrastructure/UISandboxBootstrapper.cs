using System;
using System.Collections.Generic;
using RicochetTanks.Features.UI.Configs;
using RicochetTanks.Features.UI.Core;
using RicochetTanks.Features.UI.Presenters;
using RicochetTanks.Features.UI.Services.Interfaces;
using RicochetTanks.Features.UI.Services.Mocks;
using RicochetTanks.Features.UI.Views;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RicochetTanks.Features.UI.Infrastructure
{
    public sealed class UISandboxBootstrapper : MonoBehaviour
    {
        [SerializeField] private UIThemeConfig _theme;
        [SerializeField] private MainMenuView _mainMenuView;
        [SerializeField] private LobbyView _lobbyView;
        [SerializeField] private RoomView _roomView;
        [SerializeField] private GameplayHudView _gameplayHudView;
        [SerializeField] private ResultView _resultView;

        private readonly List<IDisposable> _presenters = new List<IDisposable>();

        private IScreenService _screenService;
        private ILobbyService _lobbyService;
        private IRoomService _roomService;
        private IConnectionStatusService _connectionStatusService;

        private void Awake()
        {
            Compose();
        }

        private void OnDestroy()
        {
            if (_screenService != null)
            {
                _screenService.StateChanged -= OnScreenStateChanged;
            }

            DisposePresenters();
        }

        public void Configure(
            UIThemeConfig theme,
            MainMenuView mainMenuView,
            LobbyView lobbyView,
            RoomView roomView,
            GameplayHudView gameplayHudView,
            ResultView resultView)
        {
            _theme = theme;
            _mainMenuView = mainMenuView;
            _lobbyView = lobbyView;
            _roomView = roomView;
            _gameplayHudView = gameplayHudView;
            _resultView = resultView;
        }

        private void Compose()
        {
            ResolveTheme();
            EnsureEventSystem();
            EnsureViews();

            _screenService = new MockScreenService();
            _lobbyService = new MockLobbyService();
            _roomService = new MockRoomService();
            _connectionStatusService = new MockConnectionStatusService();

            _screenService.StateChanged += OnScreenStateChanged;
            BindPresenters();
            _screenService.ShowMainMenu();
        }

        private void ResolveTheme()
        {
            if (_theme == null)
            {
                _theme = ScriptableObject.CreateInstance<UIThemeConfig>();
            }
        }

        private void EnsureViews()
        {
            if (_mainMenuView != null
                && _lobbyView != null
                && _roomView != null
                && _gameplayHudView != null
                && _resultView != null)
            {
                return;
            }

            var roots = UIRuntimeFactory.CreateAndroidCanvas("UISandboxCanvas", _theme);
            var views = UISandboxBuilder.Create(roots, _theme);
            _mainMenuView = views.MainMenuView;
            _lobbyView = views.LobbyView;
            _roomView = views.RoomView;
            _gameplayHudView = views.GameplayHudView;
            _resultView = views.ResultView;
        }

        private void BindPresenters()
        {
            DisposePresenters();
            _presenters.Add(new MainMenuPresenter(_mainMenuView, _screenService, _connectionStatusService));
            _presenters.Add(new LobbyPresenter(_lobbyView, _screenService, _lobbyService, _roomService, _connectionStatusService));
            _presenters.Add(new RoomPresenter(_roomView, _screenService, _roomService));
            _presenters.Add(new GameplayHudPresenter(_gameplayHudView, _screenService, _roomService, _connectionStatusService));
            _presenters.Add(new ResultPresenter(_resultView, _screenService, _roomService, _connectionStatusService));
        }

        private void DisposePresenters()
        {
            for (var i = 0; i < _presenters.Count; i++)
            {
                _presenters[i].Dispose();
            }

            _presenters.Clear();
        }

        private void OnScreenStateChanged(UIFlowState state)
        {
            var isMainMenu = state == UIFlowState.Disconnected
                || state == UIFlowState.Connecting
                || state == UIFlowState.ConnectionLost;

            _mainMenuView.SetVisible(isMainMenu);
            _lobbyView.SetVisible(state == UIFlowState.InLobby);
            _roomView.SetVisible(state == UIFlowState.InRoom || state == UIFlowState.LoadingMatch);
            _gameplayHudView.SetVisible(state == UIFlowState.InMatch);
            _resultView.SetVisible(state == UIFlowState.MatchFinished);
        }

        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("UI EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }
}
