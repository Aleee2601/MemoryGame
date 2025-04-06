using MemoryGame.Models;
using MemoryGame.Services;
using System;

namespace MemoryGame.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private readonly UserService _userService;
        private readonly GameService _gameService;
        
        private ViewModelBase _currentViewModel;
        private LoginViewModel _loginViewModel;
        private GameViewModel _gameViewModel;
        private StatisticsViewModel _statisticsViewModel;
        private AboutViewModel _aboutViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel()
        {
            // Initialize services
            _fileService = new FileService();
            _userService = new UserService(_fileService);
            _gameService = new GameService(_fileService, _userService);
            
            // Initialize view models
            _loginViewModel = new LoginViewModel(_userService, _fileService);
            _loginViewModel.PlayRequested += OnPlayRequested;
            
            _gameViewModel = new GameViewModel(_gameService, _userService);
            _gameViewModel.ExitRequested += OnExitRequested;
            _gameViewModel.StatisticsRequested += OnStatisticsRequested;
            _gameViewModel.AboutRequested += OnAboutRequested;
            
            _statisticsViewModel = new StatisticsViewModel(_userService);
            _statisticsViewModel.CloseRequested += OnStatisticsCloseRequested;
            
            _aboutViewModel = new AboutViewModel();
            _aboutViewModel.CloseRequested += OnAboutCloseRequested;
            
            // Set initial view model
            CurrentViewModel = _loginViewModel;
        }

        private void OnPlayRequested(User user)
        {
            _gameViewModel.Initialize(user);
            CurrentViewModel = _gameViewModel;
        }

        private void OnExitRequested()
        {
            CurrentViewModel = _loginViewModel;
        }
        
        private void OnStatisticsRequested()
        {
            CurrentViewModel = _statisticsViewModel;
        }
        
        private void OnAboutRequested()
        {
            CurrentViewModel = _aboutViewModel;
        }

        public void ShowStatistics()
        {
            if (CurrentViewModel == _gameViewModel)
            {
                CurrentViewModel = _statisticsViewModel;
            }
        }

        private void OnStatisticsCloseRequested()
        {
            CurrentViewModel = _gameViewModel;
        }

        public void ShowAbout()
        {
            if (CurrentViewModel == _gameViewModel)
            {
                CurrentViewModel = _aboutViewModel;
            }
        }

        private void OnAboutCloseRequested()
        {
            CurrentViewModel = _gameViewModel;
        }
    }
}
