using RicochetTanks.Features.UI.Views;

namespace RicochetTanks.Features.UI.Infrastructure
{
    public sealed class UISandboxViews
    {
        public UISandboxViews(
            MainMenuView mainMenuView,
            LobbyView lobbyView,
            RoomView roomView,
            GameplayHudView gameplayHudView,
            ResultView resultView)
        {
            MainMenuView = mainMenuView;
            LobbyView = lobbyView;
            RoomView = roomView;
            GameplayHudView = gameplayHudView;
            ResultView = resultView;
        }

        public MainMenuView MainMenuView { get; }
        public LobbyView LobbyView { get; }
        public RoomView RoomView { get; }
        public GameplayHudView GameplayHudView { get; }
        public ResultView ResultView { get; }
    }
}
