using RicochetTanks.UI;
using RicochetTanks.UI.Sandbox;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.Infrastructure.Composition
{
    public sealed class SandboxHudViewFactory
    {
        public SandboxHudView Create(Canvas gameplayCanvas)
        {
            var canvas = gameplayCanvas != null ? gameplayCanvas : UiFactory.CreateCanvas("GameplayCanvas");

            var playerHpText = UiFactory.CreateText(canvas.transform, "PlayerHpText", new Vector2(20f, -20f), new Vector2(260f, 28f), TextAnchor.MiddleLeft);
            AnchorTopLeft(playerHpText.rectTransform);
            var enemyHpText = UiFactory.CreateText(canvas.transform, "EnemyHpText", new Vector2(20f, -52f), new Vector2(260f, 28f), TextAnchor.MiddleLeft);
            AnchorTopLeft(enemyHpText.rectTransform);
            var lastHitText = UiFactory.CreateText(canvas.transform, "LastHitText", new Vector2(20f, -84f), new Vector2(520f, 28f), TextAnchor.MiddleLeft);
            AnchorTopLeft(lastHitText.rectTransform);
            var roundResultText = UiFactory.CreateText(canvas.transform, "RoundResultText", new Vector2(0f, -18f), new Vector2(360f, 30f), TextAnchor.MiddleCenter);
            AnchorTopCenter(roundResultText.rectTransform);
            var controlsHintText = UiFactory.CreateText(canvas.transform, "ControlsHintText", new Vector2(0f, 22f), new Vector2(900f, 30f), TextAnchor.MiddleCenter);
            AnchorBottomCenter(controlsHintText.rectTransform);
            var restartButton = UiFactory.CreateButton(canvas.transform, "Restart", new Vector2(-20f, -20f), new Vector2(160f, 42f), null);
            AnchorTopRight((RectTransform)restartButton.transform);

            var hudView = canvas.gameObject.AddComponent<SandboxHudView>();
            hudView.Configure(playerHpText, enemyHpText, lastHitText, roundResultText, controlsHintText, restartButton);
            return hudView;
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

        private static void AnchorTopRight(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(1f, 1f);
        }

        private static void AnchorBottomCenter(RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0f);
        }
    }
}
