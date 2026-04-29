using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuViewFactory
    {
        public MainMenuView CreateFallbackView()
        {
            var canvas = UiFactory.CreateCanvas("MainMenuCanvas");
            var title = UiFactory.CreateText(canvas.transform, "Title", new Vector2(0f, 120f), new Vector2(420f, 55f), TextAnchor.MiddleCenter);
            title.text = "Ricochet Tanks / World of Balance";
            title.fontSize = 32;

            var playButton = UiFactory.CreateButton(canvas.transform, "Play Demo", new Vector2(0f, 30f), null);
            var quitButton = UiFactory.CreateButton(canvas.transform, "Quit", new Vector2(0f, -30f), null);

            var view = canvas.gameObject.AddComponent<MainMenuView>();
            view.Configure(playButton, quitButton);
            return view;
        }
    }
}
