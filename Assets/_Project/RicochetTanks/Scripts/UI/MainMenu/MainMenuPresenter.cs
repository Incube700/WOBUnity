using System;
using RicochetTanks.Gameplay.Match;
using RicochetTanks.Infrastructure.SceneLoading;
using RicochetTanks.Statistics;
using RicochetTanks.UI.Core;

namespace RicochetTanks.UI.MainMenu
{
    public sealed class MainMenuPresenter : IPresenter
    {
        private readonly MainMenuView _view;
        private readonly SceneLoaderService _sceneLoaderService;
        private readonly PlayerStatisticsRepository _statisticsRepository;
        private readonly LocalMatchSessionService _sessionService;

        public MainMenuPresenter(MainMenuView view, SceneLoaderService sceneLoaderService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _sceneLoaderService = sceneLoaderService ?? throw new ArgumentNullException(nameof(sceneLoaderService));
            _statisticsRepository = new PlayerStatisticsRepository();
            _sessionService = new LocalMatchSessionService();

            _view.PlayClicked += OnPlayClicked;
            _view.StatisticsClicked += OnStatisticsClicked;
            _view.BackFromStatisticsClicked += OnBackFromStatisticsClicked;
            _view.ResetStatisticsClicked += OnResetStatisticsClicked;
            _view.QuitClicked += OnQuitClicked;
            _view.ShowMainMenu();
        }

        public void Dispose()
        {
            _view.PlayClicked -= OnPlayClicked;
            _view.StatisticsClicked -= OnStatisticsClicked;
            _view.BackFromStatisticsClicked -= OnBackFromStatisticsClicked;
            _view.ResetStatisticsClicked -= OnResetStatisticsClicked;
            _view.QuitClicked -= OnQuitClicked;
        }

        private void OnPlayClicked()
        {
            _sessionService.StartNewMatch(3);
            _sceneLoaderService.Load(SceneLoaderService.DemoSceneName);
        }

        private void OnStatisticsClicked()
        {
            _view.ShowStatistics(_statisticsRepository.Load());
        }

        private void OnBackFromStatisticsClicked()
        {
            _view.ShowMainMenu();
        }

        private void OnResetStatisticsClicked()
        {
            _statisticsRepository.Reset();
            _view.DisplayStatistics(_statisticsRepository.Load());
        }

        private static void OnQuitClicked()
        {
            SceneLoaderService.QuitGame();
        }
    }
}
