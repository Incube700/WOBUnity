using UnityEngine;

namespace RicochetTanks.Features.UI.Configs
{
    [CreateAssetMenu(fileName = "UIThemeConfig", menuName = "World of Balance/UI Theme")]
    public sealed class UIThemeConfig : ScriptableObject
    {
        [Header("Sprites")]
        [SerializeField] private Sprite _buttonSprite;
        [SerializeField] private Sprite _panelSprite;
        [SerializeField] private Sprite _popupSprite;
        [SerializeField] private Sprite _barFillSprite;

        [Header("Typography")]
        [SerializeField] private Font _font;
        [SerializeField] private int _titleFontSize = 42;
        [SerializeField] private int _headerFontSize = 28;
        [SerializeField] private int _bodyFontSize = 18;
        [SerializeField] private int _buttonFontSize = 18;

        [Header("Colors")]
        [SerializeField] private Color _screenBackgroundColor = new Color(0.05f, 0.06f, 0.07f, 1f);
        [SerializeField] private Color _panelColor = new Color(0.12f, 0.13f, 0.14f, 0.96f);
        [SerializeField] private Color _popupColor = new Color(0.08f, 0.09f, 0.1f, 0.98f);
        [SerializeField] private Color _buttonColor = new Color(0.22f, 0.26f, 0.3f, 1f);
        [SerializeField] private Color _buttonTextColor = Color.white;
        [SerializeField] private Color _primaryTextColor = Color.white;
        [SerializeField] private Color _secondaryTextColor = new Color(0.78f, 0.82f, 0.86f, 1f);
        [SerializeField] private Color _healthFillColor = new Color(0.8f, 0.18f, 0.18f, 1f);
        [SerializeField] private Color _reloadFillColor = new Color(0.22f, 0.58f, 0.9f, 1f);

        [Header("Layout")]
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1920f, 1080f);
        [SerializeField] private Vector2 _defaultButtonSize = new Vector2(280f, 56f);
        [SerializeField] private Vector2 _roomCardSize = new Vector2(560f, 88f);
        [SerializeField] private float _spacing = 18f;

        public Sprite ButtonSprite { get { return _buttonSprite; } }
        public Sprite PanelSprite { get { return _panelSprite; } }
        public Sprite PopupSprite { get { return _popupSprite; } }
        public Sprite BarFillSprite { get { return _barFillSprite; } }
        public Font Font { get { return _font; } }
        public int TitleFontSize { get { return _titleFontSize; } }
        public int HeaderFontSize { get { return _headerFontSize; } }
        public int BodyFontSize { get { return _bodyFontSize; } }
        public int ButtonFontSize { get { return _buttonFontSize; } }
        public Color ScreenBackgroundColor { get { return _screenBackgroundColor; } }
        public Color PanelColor { get { return _panelColor; } }
        public Color PopupColor { get { return _popupColor; } }
        public Color ButtonColor { get { return _buttonColor; } }
        public Color ButtonTextColor { get { return _buttonTextColor; } }
        public Color PrimaryTextColor { get { return _primaryTextColor; } }
        public Color SecondaryTextColor { get { return _secondaryTextColor; } }
        public Color HealthFillColor { get { return _healthFillColor; } }
        public Color ReloadFillColor { get { return _reloadFillColor; } }
        public Vector2 ReferenceResolution { get { return _referenceResolution; } }
        public Vector2 DefaultButtonSize { get { return _defaultButtonSize; } }
        public Vector2 RoomCardSize { get { return _roomCardSize; } }
        public float Spacing { get { return _spacing; } }

        public Font ResolveFont()
        {
            return _font != null ? _font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
    }
}
