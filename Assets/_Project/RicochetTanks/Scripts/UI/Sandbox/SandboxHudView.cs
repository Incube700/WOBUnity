using System;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.Sandbox
{
    public class SandboxHudView : MonoBehaviour
    {
        [SerializeField] private Text _playerHpText;
        [SerializeField] private Text _enemyHpText;
        [SerializeField] private Text _lastHitText;
        [SerializeField] private Text _roundResultText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _controlsHintText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitToMenuButton;

        private bool _isSubscribed;

        public event Action RestartClicked;
        public event Action ExitToMenuClicked;

        private void Awake()
        {
            EnsureRuntimeControls();
        }

        private void OnEnable()
        {
            EnsureRuntimeControls();
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(
            Text playerHpText,
            Text enemyHpText,
            Text lastHitText,
            Text roundResultText,
            Text controlsHintText,
            Button restartButton)
        {
            Configure(playerHpText, enemyHpText, lastHitText, roundResultText, null, controlsHintText, restartButton, null);
        }

        public void Configure(
            Text playerHpText,
            Text enemyHpText,
            Text lastHitText,
            Text roundResultText,
            Text scoreText,
            Text controlsHintText,
            Button restartButton,
            Button exitToMenuButton)
        {
            Unsubscribe();
            _playerHpText = playerHpText;
            _enemyHpText = enemyHpText;
            _lastHitText = lastHitText;
            _roundResultText = roundResultText;
            _scoreText = scoreText;
            _controlsHintText = controlsHintText;
            _restartButton = restartButton;
            _exitToMenuButton = exitToMenuButton;
            EnsureRuntimeControls();
            Subscribe();
        }

        public void SetPlayerHp(float currentHp, float maxHp)
        {
            if (_playerHpText != null)
            {
                _playerHpText.text = $"Player HP: {Format(currentHp)}/{Format(maxHp)}";
            }
        }

        public void SetEnemyHp(float currentHp, float maxHp)
        {
            if (_enemyHpText != null)
            {
                _enemyHpText.text = $"Enemy HP: {Format(currentHp)}/{Format(maxHp)}";
            }
        }

        public void SetLastHitResult(string text)
        {
            if (_lastHitText != null)
            {
                _lastHitText.text = text;
            }
        }

        public void SetRoundResult(string text)
        {
            if (_roundResultText != null)
            {
                _roundResultText.text = text;
            }
        }

        public void SetScore(string text)
        {
            if (_scoreText != null)
            {
                _scoreText.text = text;
            }
        }

        public void SetControlsHint(string text)
        {
            if (_controlsHintText != null)
            {
                _controlsHintText.text = text;
            }
        }

        public void SetResultControlsVisible(bool isVisible)
        {
            if (_restartButton != null)
            {
                _restartButton.gameObject.SetActive(isVisible);
            }

            if (_exitToMenuButton != null)
            {
                _exitToMenuButton.gameObject.SetActive(isVisible);
            }
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartButtonClicked);
            }

            if (_exitToMenuButton != null)
            {
                _exitToMenuButton.onClick.AddListener(OnExitToMenuButtonClicked);
            }

            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            }

            if (_exitToMenuButton != null)
            {
                _exitToMenuButton.onClick.RemoveListener(OnExitToMenuButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnRestartButtonClicked()
        {
            RestartClicked?.Invoke();
        }

        private void OnExitToMenuButtonClicked()
        {
            ExitToMenuClicked?.Invoke();
        }

        private void EnsureRuntimeControls()
        {
            var parent = transform;

            if (_scoreText == null)
            {
                _scoreText = CreateText(parent, "ScoreText", new Vector2(0f, -52f), new Vector2(420f, 28f), TextAnchor.MiddleCenter);
                AnchorTopCenter(_scoreText.rectTransform);
            }

            if (_exitToMenuButton == null)
            {
                _exitToMenuButton = CreateButton(parent, "Menu", new Vector2(-20f, -68f), new Vector2(160f, 42f));
                AnchorTopRight((RectTransform)_exitToMenuButton.transform);
            }
        }

        private static Text CreateText(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, TextAnchor alignment)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var rectTransform = textObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = anchoredPosition;
            var label = textObject.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.alignment = alignment;
            label.color = Color.white;
            return label;
        }

        private static Button CreateButton(Transform parent, string text, Vector2 anchoredPosition, Vector2 size)
        {
            var buttonObject = new GameObject(text + "Button");
            buttonObject.transform.SetParent(parent, false);

            var rectTransform = buttonObject.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = anchoredPosition;

            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            var button = buttonObject.AddComponent<Button>();

            var textObject = new GameObject("Label");
            textObject.transform.SetParent(buttonObject.transform, false);
            var textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            var label = textObject.AddComponent<Text>();
            label.text = text;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 16;

            return button;
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

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
