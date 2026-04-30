using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuViewFactory
    {
        public MainMenuView CreateFallbackView()
        {
            var canvas = UiFactory.CreateCanvas("MainMenuCanvas");

            var menuRoot = new GameObject("MainMenuRoot");
            menuRoot.transform.SetParent(canvas.transform, false);
            var menuRootRect = menuRoot.AddComponent<RectTransform>();
            Stretch(menuRootRect);

            var title = UiFactory.CreateText(menuRoot.transform, "Title", new Vector2(0f, 135f), new Vector2(520f, 55f), TextAnchor.MiddleCenter);
            title.text = "Ricochet Tanks / World of Balance";
            title.fontSize = 32;

            var playButton = UiFactory.CreateButton(menuRoot.transform, "Start Game", new Vector2(0f, 45f), null);
            var statisticsButton = UiFactory.CreateButton(menuRoot.transform, "Statistics", new Vector2(0f, -15f), null);
            var quitButton = UiFactory.CreateButton(menuRoot.transform, "Exit", new Vector2(0f, -75f), null);

            var statisticsPanel = CreateStatisticsPanel(canvas.transform, out var summaryText, out var recentText, out var backButton, out var resetButton);
            statisticsPanel.SetActive(false);

            var view = canvas.gameObject.AddComponent<MainMenuView>();
            view.Configure(
                playButton,
                statisticsButton,
                quitButton,
                menuRoot,
                statisticsPanel,
                summaryText,
                recentText,
                backButton,
                resetButton);
            return view;
        }

        private static GameObject CreateStatisticsPanel(
            Transform parent,
            out Text summaryText,
            out Text recentText,
            out Button backButton,
            out Button resetButton)
        {
            var panel = new GameObject("StatisticsPanel");
            panel.transform.SetParent(parent, false);
            var rectTransform = panel.AddComponent<RectTransform>();
            Stretch(rectTransform);

            var background = panel.AddComponent<Image>();
            background.color = new Color(0.05f, 0.06f, 0.07f, 0.96f);

            var title = UiFactory.CreateText(panel.transform, "StatisticsTitle", new Vector2(0f, -28f), new Vector2(520f, 42f), TextAnchor.MiddleCenter);
            AnchorTopCenter(title.rectTransform);
            title.text = "Statistics";
            title.fontSize = 28;

            summaryText = UiFactory.CreateText(panel.transform, "StatisticsSummary", new Vector2(32f, -92f), new Vector2(430f, 360f), TextAnchor.UpperLeft);
            AnchorTopLeft(summaryText.rectTransform);
            summaryText.fontSize = 16;

            recentText = UiFactory.CreateText(panel.transform, "RecentMatches", new Vector2(490f, -92f), new Vector2(740f, 390f), TextAnchor.UpperLeft);
            AnchorTopLeft(recentText.rectTransform);
            recentText.fontSize = 14;

            backButton = UiFactory.CreateButton(panel.transform, "Back", new Vector2(-20f, 22f), new Vector2(160f, 42f), null);
            AnchorBottomRight((RectTransform)backButton.transform);

            resetButton = UiFactory.CreateButton(panel.transform, "Reset Stats", new Vector2(-200f, 22f), new Vector2(180f, 42f), null);
            AnchorBottomRight((RectTransform)resetButton.transform);

            return panel;
        }

        private static void Stretch(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static void AnchorTopLeft(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
        }

        private static void AnchorTopCenter(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
        }

        private static void AnchorBottomRight(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 0f);
            rectTransform.pivot = new Vector2(1f, 0f);
        }
    }
}
