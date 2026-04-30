using RicochetTanks.Features.UI.Configs;
using RicochetTanks.Features.UI.Views;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Features.UI.Infrastructure
{
    public static class UISandboxBuilder
    {
        public static UISandboxViews Create(UIRootSet roots, UIThemeConfig theme)
        {
            var mainMenu = CreateMainMenu(roots.ScreensRoot, theme);
            var lobby = CreateLobby(roots.ScreensRoot, theme);
            var room = CreateRoom(roots.ScreensRoot, theme);
            var gameplayHud = CreateGameplayHud(roots.ScreensRoot, theme);
            var result = CreateResult(roots.PopupsRoot, theme);
            return new UISandboxViews(mainMenu, lobby, room, gameplayHud, result);
        }

        public static MainMenuView CreateMainMenu(Transform parent, UIThemeConfig theme)
        {
            var screen = CreateScreen(parent, "MainMenuView", theme);

            var title = UIRuntimeFactory.CreateText(screen, "Title", "World of Balance", theme.TitleFontSize, TextAnchor.MiddleCenter, theme);
            UIRuntimeFactory.SetAnchored(title.rectTransform, new Vector2(0f, 210f), new Vector2(720f, 72f), new Vector2(0.5f, 0.5f));

            var subtitle = UIRuntimeFactory.CreateText(screen, "Subtitle", "Ricochet Tanks UI Sandbox", theme.HeaderFontSize, TextAnchor.MiddleCenter, theme);
            subtitle.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(subtitle.rectTransform, new Vector2(0f, 150f), new Vector2(720f, 50f), new Vector2(0.5f, 0.5f));

            var stateText = UIRuntimeFactory.CreateText(screen, "StateText", "Disconnected", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            stateText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(stateText.rectTransform, new Vector2(0f, 94f), new Vector2(420f, 34f), new Vector2(0.5f, 0.5f));

            var column = CreateColumn(screen, "MainMenuButtons", new Vector2(0f, -40f), new Vector2(320f, 230f), 18f);
            var playButton = UIRuntimeFactory.CreateButton(column, "PlayButton", "Play UI Match", theme, out _);
            var onlineButton = UIRuntimeFactory.CreateButton(column, "OnlineButton", "Online", theme, out _);
            var quitButton = UIRuntimeFactory.CreateButton(column, "QuitButton", "Disconnect", theme, out _);

            var view = screen.gameObject.AddComponent<MainMenuView>();
            view.Configure(playButton, onlineButton, quitButton, stateText);
            return view;
        }

        public static LobbyView CreateLobby(Transform parent, UIThemeConfig theme)
        {
            var screen = CreateScreen(parent, "LobbyView", theme);
            var panel = UIRuntimeFactory.CreateFixedPanel(screen, "LobbyPanel", new Vector2(1180f, 760f), theme);

            var title = UIRuntimeFactory.CreateText(panel, "Title", "Online Lobby", theme.HeaderFontSize, TextAnchor.MiddleLeft, theme);
            UIRuntimeFactory.SetAnchored(title.rectTransform, new Vector2(42f, 330f), new Vector2(360f, 46f), new Vector2(0f, 0.5f));

            var statusText = UIRuntimeFactory.CreateText(panel, "StatusText", "Room List placeholder", theme.BodyFontSize, TextAnchor.MiddleRight, theme);
            statusText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(statusText.rectTransform, new Vector2(-42f, 330f), new Vector2(520f, 40f), new Vector2(1f, 0.5f));

            var quickMatchButton = UIRuntimeFactory.CreateButton(panel, "QuickMatchButton", "Quick Match", theme, out _);
            UIRuntimeFactory.SetAnchored(quickMatchButton.GetComponent<RectTransform>(), new Vector2(42f, 250f), theme.DefaultButtonSize, new Vector2(0f, 0.5f));

            var refreshButton = UIRuntimeFactory.CreateButton(panel, "RefreshRoomsButton", "Refresh Rooms", theme, out _);
            UIRuntimeFactory.SetAnchored(refreshButton.GetComponent<RectTransform>(), new Vector2(350f, 250f), theme.DefaultButtonSize, new Vector2(0f, 0.5f));

            var roomNameInput = UIRuntimeFactory.CreateInputField(panel, "RoomNameInput", "Room name", theme);
            UIRuntimeFactory.SetAnchored(roomNameInput.GetComponent<RectTransform>(), new Vector2(42f, 150f), new Vector2(300f, 48f), new Vector2(0f, 0.5f));

            var createRoomButton = UIRuntimeFactory.CreateButton(panel, "CreateRoomButton", "Create Room", theme, out _);
            UIRuntimeFactory.SetAnchored(createRoomButton.GetComponent<RectTransform>(), new Vector2(370f, 150f), theme.DefaultButtonSize, new Vector2(0f, 0.5f));

            var roomCodeInput = UIRuntimeFactory.CreateInputField(panel, "RoomCodeInput", "Room code", theme);
            UIRuntimeFactory.SetAnchored(roomCodeInput.GetComponent<RectTransform>(), new Vector2(42f, 70f), new Vector2(300f, 48f), new Vector2(0f, 0.5f));

            var joinButton = UIRuntimeFactory.CreateButton(panel, "JoinByCodeButton", "Join By Code", theme, out _);
            UIRuntimeFactory.SetAnchored(joinButton.GetComponent<RectTransform>(), new Vector2(370f, 70f), theme.DefaultButtonSize, new Vector2(0f, 0.5f));

            var backButton = UIRuntimeFactory.CreateButton(panel, "BackButton", "Back", theme, out _);
            UIRuntimeFactory.SetAnchored(backButton.GetComponent<RectTransform>(), new Vector2(42f, -302f), theme.DefaultButtonSize, new Vector2(0f, 0.5f));

            var listPanel = UIRuntimeFactory.CreateFixedPanel(panel, "RoomListPanel", new Vector2(560f, 516f), theme);
            UIRuntimeFactory.SetAnchored(listPanel, new Vector2(-42f, -40f), new Vector2(560f, 516f), new Vector2(1f, 0.5f));

            var roomListRoot = UIRuntimeFactory.CreateStretchRect("RoomListRoot", listPanel);
            roomListRoot.offsetMin = new Vector2(0f, 20f);
            roomListRoot.offsetMax = new Vector2(0f, -20f);
            var layout = roomListRoot.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 12f;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var placeholderText = UIRuntimeFactory.CreateText(listPanel, "PlaceholderText", "Room List placeholder", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            placeholderText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.Stretch(placeholderText.rectTransform, Vector2.zero);

            var roomCardTemplate = UIRuntimeFactory.CreateRoomCard(roomListRoot, theme);
            roomCardTemplate.gameObject.SetActive(false);

            var view = screen.gameObject.AddComponent<LobbyView>();
            view.Configure(
                quickMatchButton,
                createRoomButton,
                joinButton,
                refreshButton,
                backButton,
                roomNameInput,
                roomCodeInput,
                statusText,
                placeholderText,
                roomListRoot,
                roomCardTemplate);
            return view;
        }

        public static RoomView CreateRoom(Transform parent, UIThemeConfig theme)
        {
            var screen = CreateScreen(parent, "RoomView", theme);
            var panel = UIRuntimeFactory.CreateFixedPanel(screen, "RoomPanel", new Vector2(760f, 540f), theme);

            var roomNameText = UIRuntimeFactory.CreateText(panel, "RoomNameText", "Room", theme.HeaderFontSize, TextAnchor.MiddleCenter, theme);
            UIRuntimeFactory.SetAnchored(roomNameText.rectTransform, new Vector2(0f, 205f), new Vector2(620f, 52f), new Vector2(0.5f, 0.5f));

            var roomCodeText = UIRuntimeFactory.CreateText(panel, "RoomCodeText", "Code: ----", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            UIRuntimeFactory.SetAnchored(roomCodeText.rectTransform, new Vector2(0f, 158f), new Vector2(620f, 32f), new Vector2(0.5f, 0.5f));

            var regionText = UIRuntimeFactory.CreateText(panel, "RegionText", "Region: Auto/Asia", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            regionText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(regionText.rectTransform, new Vector2(0f, 118f), new Vector2(620f, 32f), new Vector2(0.5f, 0.5f));

            var stateText = UIRuntimeFactory.CreateText(panel, "StateText", "Waiting", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            UIRuntimeFactory.SetAnchored(stateText.rectTransform, new Vector2(0f, 76f), new Vector2(620f, 32f), new Vector2(0.5f, 0.5f));

            var playerSlotsText = UIRuntimeFactory.CreateText(panel, "PlayerSlotsText", "1. Empty\n2. Empty", theme.BodyFontSize, TextAnchor.UpperLeft, theme);
            UIRuntimeFactory.SetAnchored(playerSlotsText.rectTransform, new Vector2(0f, -20f), new Vector2(520f, 120f), new Vector2(0.5f, 0.5f));

            var readyButton = UIRuntimeFactory.CreateButton(panel, "ReadyButton", "Ready", theme, out var readyButtonText);
            UIRuntimeFactory.SetAnchored(readyButton.GetComponent<RectTransform>(), new Vector2(-230f, -194f), theme.DefaultButtonSize, new Vector2(0.5f, 0.5f));

            var startButton = UIRuntimeFactory.CreateButton(panel, "StartButton", "Start", theme, out _);
            UIRuntimeFactory.SetAnchored(startButton.GetComponent<RectTransform>(), new Vector2(0f, -194f), theme.DefaultButtonSize, new Vector2(0.5f, 0.5f));

            var leaveButton = UIRuntimeFactory.CreateButton(panel, "LeaveButton", "Leave", theme, out _);
            UIRuntimeFactory.SetAnchored(leaveButton.GetComponent<RectTransform>(), new Vector2(230f, -194f), theme.DefaultButtonSize, new Vector2(0.5f, 0.5f));

            var view = screen.gameObject.AddComponent<RoomView>();
            view.Configure(roomNameText, roomCodeText, regionText, stateText, playerSlotsText, readyButton, readyButtonText, startButton, leaveButton);
            return view;
        }

        public static GameplayHudView CreateGameplayHud(Transform parent, UIThemeConfig theme)
        {
            var screen = UIRuntimeFactory.CreateStretchRect("GameplayHudView", parent);

            var playerHealth = UIRuntimeFactory.CreateProgressBar(screen, "WOB_HealthBar_Player", theme.HealthFillColor);
            UIRuntimeFactory.SetAnchored(playerHealth.GetComponent<RectTransform>(), new Vector2(42f, -42f), new Vector2(360f, 22f), new Vector2(0f, 1f));

            var playerHealthText = UIRuntimeFactory.CreateText(screen, "PlayerHealthText", "Player HP", theme.BodyFontSize, TextAnchor.MiddleLeft, theme);
            UIRuntimeFactory.SetAnchored(playerHealthText.rectTransform, new Vector2(42f, -74f), new Vector2(360f, 28f), new Vector2(0f, 1f));

            var enemyHealth = UIRuntimeFactory.CreateProgressBar(screen, "WOB_HealthBar_Enemy", theme.HealthFillColor);
            UIRuntimeFactory.SetAnchored(enemyHealth.GetComponent<RectTransform>(), new Vector2(-42f, -42f), new Vector2(360f, 22f), new Vector2(1f, 1f));

            var enemyHealthText = UIRuntimeFactory.CreateText(screen, "EnemyHealthText", "Enemy HP", theme.BodyFontSize, TextAnchor.MiddleRight, theme);
            UIRuntimeFactory.SetAnchored(enemyHealthText.rectTransform, new Vector2(-42f, -74f), new Vector2(360f, 28f), new Vector2(1f, 1f));

            var reload = UIRuntimeFactory.CreateProgressBar(screen, "WOB_ReloadBar", theme.ReloadFillColor);
            UIRuntimeFactory.SetAnchored(reload.GetComponent<RectTransform>(), new Vector2(0f, -42f), new Vector2(360f, 22f), new Vector2(0.5f, 1f));

            var reloadText = UIRuntimeFactory.CreateText(screen, "ReloadText", "Reload", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            UIRuntimeFactory.SetAnchored(reloadText.rectTransform, new Vector2(0f, -74f), new Vector2(360f, 28f), new Vector2(0.5f, 1f));

            var connectionText = UIRuntimeFactory.CreateText(screen, "ConnectionText", "Ping: -- ms", theme.BodyFontSize, TextAnchor.MiddleRight, theme);
            connectionText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(connectionText.rectTransform, new Vector2(-42f, -112f), new Vector2(420f, 28f), new Vector2(1f, 1f));

            var lastHitText = UIRuntimeFactory.CreateText(screen, "LastHitText", "Last hit: none", theme.BodyFontSize, TextAnchor.MiddleLeft, theme);
            UIRuntimeFactory.SetAnchored(lastHitText.rectTransform, new Vector2(42f, 112f), new Vector2(520f, 30f), new Vector2(0f, 0f));

            var ricochetText = UIRuntimeFactory.CreateText(screen, "RicochetText", "Ricochets: 0", theme.BodyFontSize, TextAnchor.MiddleLeft, theme);
            ricochetText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(ricochetText.rectTransform, new Vector2(42f, 76f), new Vector2(520f, 30f), new Vector2(0f, 0f));

            var finishButton = UIRuntimeFactory.CreateButton(screen, "FinishMockButton", "Finish Mock", theme, out _);
            UIRuntimeFactory.SetAnchored(finishButton.GetComponent<RectTransform>(), new Vector2(-342f, 42f), theme.DefaultButtonSize, new Vector2(1f, 0f));

            var leaveButton = UIRuntimeFactory.CreateButton(screen, "LeaveMatchButton", "Leave", theme, out _);
            UIRuntimeFactory.SetAnchored(leaveButton.GetComponent<RectTransform>(), new Vector2(-42f, 42f), theme.DefaultButtonSize, new Vector2(1f, 0f));

            var view = screen.gameObject.AddComponent<GameplayHudView>();
            view.Configure(
                playerHealth,
                playerHealthText,
                enemyHealth,
                enemyHealthText,
                reload,
                reloadText,
                lastHitText,
                ricochetText,
                connectionText,
                finishButton,
                leaveButton);
            return view;
        }

        public static ResultView CreateResult(Transform parent, UIThemeConfig theme)
        {
            var screen = CreateScreen(parent, "ResultView", theme);
            var popup = UIRuntimeFactory.CreateFixedPanel(screen, "WOB_Popup", new Vector2(700f, 420f), theme);
            var image = popup.GetComponent<Image>();
            if (image != null)
            {
                image.color = theme.PopupColor;
                image.sprite = theme.PopupSprite;
                image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            }

            var headlineText = UIRuntimeFactory.CreateText(popup, "HeadlineText", "Result", theme.HeaderFontSize, TextAnchor.MiddleCenter, theme);
            UIRuntimeFactory.SetAnchored(headlineText.rectTransform, new Vector2(0f, 116f), new Vector2(560f, 58f), new Vector2(0.5f, 0.5f));

            var detailsText = UIRuntimeFactory.CreateText(popup, "DetailsText", "Match summary placeholder", theme.BodyFontSize, TextAnchor.MiddleCenter, theme);
            detailsText.color = theme.SecondaryTextColor;
            UIRuntimeFactory.SetAnchored(detailsText.rectTransform, new Vector2(0f, 42f), new Vector2(560f, 72f), new Vector2(0.5f, 0.5f));

            var playAgainButton = UIRuntimeFactory.CreateButton(popup, "PlayAgainButton", "Play Again", theme, out _);
            UIRuntimeFactory.SetAnchored(playAgainButton.GetComponent<RectTransform>(), new Vector2(-160f, -118f), theme.DefaultButtonSize, new Vector2(0.5f, 0.5f));

            var backButton = UIRuntimeFactory.CreateButton(popup, "BackToMenuButton", "Main Menu", theme, out _);
            UIRuntimeFactory.SetAnchored(backButton.GetComponent<RectTransform>(), new Vector2(160f, -118f), theme.DefaultButtonSize, new Vector2(0.5f, 0.5f));

            var view = screen.gameObject.AddComponent<ResultView>();
            view.Configure(headlineText, detailsText, playAgainButton, backButton);
            return view;
        }

        private static RectTransform CreateScreen(Transform parent, string name, UIThemeConfig theme)
        {
            var screen = UIRuntimeFactory.CreateStretchRect(name, parent);
            var image = screen.gameObject.AddComponent<Image>();
            image.color = theme != null ? theme.ScreenBackgroundColor : new Color(0.05f, 0.06f, 0.07f, 1f);
            return screen;
        }

        private static RectTransform CreateColumn(Transform parent, string name, Vector2 position, Vector2 size, float spacing)
        {
            var column = new GameObject(name);
            column.transform.SetParent(parent, false);
            var rectTransform = column.AddComponent<RectTransform>();
            UIRuntimeFactory.SetAnchored(rectTransform, position, size, new Vector2(0.5f, 0.5f));

            var layout = column.AddComponent<VerticalLayoutGroup>();
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            return rectTransform;
        }
    }
}
