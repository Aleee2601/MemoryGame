using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly FileService _fileService;
        private readonly UserService _userService;
        private readonly GameService _gameService;

        private LoginViewModel _loginViewModel;
        private GameViewModel _gameViewModel;
        private StatisticsViewModel _statisticsViewModel;
        private AboutViewModel _aboutViewModel;

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public MainViewModel()
        {
            // Inițializează serviciile
            _fileService = new FileService();
            _userService = new UserService(_fileService);
            _gameService = new GameService(_fileService, _userService);

            // Inițializează ViewModel-urile
            _loginViewModel = new LoginViewModel(_userService, _fileService);
            _gameViewModel = new GameViewModel(_gameService, _userService);
            _statisticsViewModel = new StatisticsViewModel(_userService);
            _aboutViewModel = new AboutViewModel();

            // Setează handler pentru evenimentul Play (pornire joc)
            _loginViewModel.PlayRequested += OnPlayRequested;

            // Handler pentru ieșire din joc (revine la login)
            _gameViewModel.ExitRequested += OnExitRequested;

            // Navigare către statistici / about
            _gameViewModel.StatisticsRequested += OnStatisticsRequested;
            _gameViewModel.AboutRequested += OnAboutRequested;

            // Închidere ferestre statistici / about
            _statisticsViewModel.CloseRequested += OnStatisticsCloseRequested;
            _aboutViewModel.CloseRequested += OnAboutCloseRequested;

            // View inițial
            CurrentViewModel = _loginViewModel;
        }

        // === Evenimente ===

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

        private void OnStatisticsCloseRequested()
        {
            CurrentViewModel = _gameViewModel;
        }

        private void OnAboutCloseRequested()
        {
            CurrentViewModel = _gameViewModel;
        }

        // Funcții publice (dacă vrei să le apelezi din MainWindow.xaml.cs, de exemplu)

        public void ShowStatistics()
        {
            CurrentViewModel = _statisticsViewModel;
        }

        public void ShowAbout()
        {
            CurrentViewModel = _aboutViewModel;
        }
    }
}
