using System;
using RicochetTanks.Statistics;
using UnityEngine;
using UnityEngine.UI;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _statisticsButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private GameObject _menuRoot;
        [SerializeField] private GameObject _statisticsPanel;
        [SerializeField] private Text _statisticsSummaryText;
        [SerializeField] private Text _recentMatchesText;
        [SerializeField] private Button _backFromStatisticsButton;
        [SerializeField] private Button _resetStatisticsButton;

        private bool _isSubscribed;

        public event Action PlayClicked;
        public event Action StatisticsClicked;
        public event Action BackFromStatisticsClicked;
        public event Action ResetStatisticsClicked;
        public event Action QuitClicked;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Configure(Button playButton, Button quitButton)
        {
            Configure(playButton, null, quitButton, null, null, null, null, null, null);
        }

        public void Configure(
            Button playButton,
            Button statisticsButton,
            Button quitButton,
            GameObject menuRoot,
            GameObject statisticsPanel,
            Text statisticsSummaryText,
            Text recentMatchesText,
            Button backFromStatisticsButton,
            Button resetStatisticsButton)
        {
            Unsubscribe();
            _playButton = playButton;
            _statisticsButton = statisticsButton;
            _quitButton = quitButton;
            _menuRoot = menuRoot;
            _statisticsPanel = statisticsPanel;
            _statisticsSummaryText = statisticsSummaryText;
            _recentMatchesText = recentMatchesText;
            _backFromStatisticsButton = backFromStatisticsButton;
            _resetStatisticsButton = resetStatisticsButton;
            Subscribe();
        }

        public void ShowMainMenu()
        {
            SetActive(_menuRoot, true);
            SetActive(_statisticsPanel, false);
        }

        public void ShowStatistics(PlayerStatisticsData data)
        {
            SetActive(_menuRoot, false);
            SetActive(_statisticsPanel, true);
            DisplayStatistics(data);
        }

        public void DisplayStatistics(PlayerStatisticsData data)
        {
            if (data == null)
            {
                data = new PlayerStatisticsData();
            }

            if (_statisticsSummaryText != null)
            {
                _statisticsSummaryText.text =
                    "Total matches: " + data.TotalMatches + "\n"
                    + "Wins: " + data.Wins + "  Losses: " + data.Losses + "  Draws: " + data.Draws + "\n"
                    + "Win rate: " + Format(data.WinRatePercent) + "%\n"
                    + "Shots fired: " + data.ShotsFired + "  Hits: " + data.TankHits + "\n"
                    + "Accuracy: " + Format(data.AccuracyPercent) + "%\n"
                    + "Ricochets: " + data.Ricochets + "\n"
                    + "Penetrations: " + data.Penetrations + "  No penetrations: " + data.NoPenetrations + "\n"
                    + "Self hits: " + data.SelfHits + "\n"
                    + "Damage dealt: " + Format(data.DamageDealt) + "\n"
                    + "Damage taken: " + Format(data.DamageTaken) + "\n"
                    + "Efficiency: " + Format(data.EfficiencyDamagePerShot);
            }

            if (_recentMatchesText != null)
            {
                _recentMatchesText.text = BuildRecentMatchesText(data);
            }
        }

        private void Subscribe()
        {
            if (_isSubscribed || _playButton == null || _quitButton == null)
            {
                return;
            }

            _playButton.onClick.AddListener(OnPlayButtonClicked);
            if (_statisticsButton != null)
            {
                _statisticsButton.onClick.AddListener(OnStatisticsButtonClicked);
            }

            if (_backFromStatisticsButton != null)
            {
                _backFromStatisticsButton.onClick.AddListener(OnBackFromStatisticsButtonClicked);
            }

            if (_resetStatisticsButton != null)
            {
                _resetStatisticsButton.onClick.AddListener(OnResetStatisticsButtonClicked);
            }

            _quitButton.onClick.AddListener(OnQuitButtonClicked);
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_playButton != null)
            {
                _playButton.onClick.RemoveListener(OnPlayButtonClicked);
            }

            if (_statisticsButton != null)
            {
                _statisticsButton.onClick.RemoveListener(OnStatisticsButtonClicked);
            }

            if (_backFromStatisticsButton != null)
            {
                _backFromStatisticsButton.onClick.RemoveListener(OnBackFromStatisticsButtonClicked);
            }

            if (_resetStatisticsButton != null)
            {
                _resetStatisticsButton.onClick.RemoveListener(OnResetStatisticsButtonClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
            }

            _isSubscribed = false;
        }

        private void OnPlayButtonClicked()
        {
            PlayClicked?.Invoke();
        }

        private void OnStatisticsButtonClicked()
        {
            StatisticsClicked?.Invoke();
        }

        private void OnBackFromStatisticsButtonClicked()
        {
            BackFromStatisticsClicked?.Invoke();
        }

        private void OnResetStatisticsButtonClicked()
        {
            ResetStatisticsClicked?.Invoke();
        }

        private void OnQuitButtonClicked()
        {
            QuitClicked?.Invoke();
        }

        private static void SetActive(GameObject target, bool isActive)
        {
            if (target != null)
            {
                target.SetActive(isActive);
            }
        }

        private static string BuildRecentMatchesText(PlayerStatisticsData data)
        {
            if (data.RecentMatches == null || data.RecentMatches.Count == 0)
            {
                return "Recent games:\nNo matches yet.";
            }

            var text = "Recent games:\n";
            for (var i = 0; i < data.RecentMatches.Count; i++)
            {
                var match = data.RecentMatches[i];
                text += match.DateTime + "  " + match.Result
                    + "  Score " + match.RoundsScore
                    + "  Shots " + match.Shots
                    + "  Hits " + match.Hits
                    + "  Acc " + Format(match.AccuracyPercent) + "%"
                    + "  DD " + Format(match.DamageDealt)
                    + "  DT " + Format(match.DamageTaken);

                if (i < data.RecentMatches.Count - 1)
                {
                    text += "\n";
                }
            }

            return text;
        }

        private static string Format(float value)
        {
            return value.ToString("0.##");
        }
    }
}
