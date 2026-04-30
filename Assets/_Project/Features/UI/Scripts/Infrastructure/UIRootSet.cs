using UnityEngine;

namespace RicochetTanks.Features.UI.Infrastructure
{
    public sealed class UIRootSet
    {
        public UIRootSet(
            Canvas canvas,
            RectTransform safeAreaRoot,
            RectTransform screensRoot,
            RectTransform popupsRoot,
            RectTransform overlayRoot)
        {
            Canvas = canvas;
            SafeAreaRoot = safeAreaRoot;
            ScreensRoot = screensRoot;
            PopupsRoot = popupsRoot;
            OverlayRoot = overlayRoot;
        }

        public Canvas Canvas { get; }
        public RectTransform SafeAreaRoot { get; }
        public RectTransform ScreensRoot { get; }
        public RectTransform PopupsRoot { get; }
        public RectTransform OverlayRoot { get; }
    }
}
