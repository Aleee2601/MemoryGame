using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private readonly GameService _gameService;
        private readonly UserService _userService;
        private readonly DispatcherTimer _timer;

        private User? _currentUser;
        private Game? _currentGame;
        private ObservableCollection<Card> _cards;
        private ObservableCollection<Category> _categories;
        private Category? _selectedCategory;
        private int _rows;
        private int _columns;
        private bool _isCustomSize;
        private int _minutes;
        private int _seconds;
        private string _timeDisplay;
        private Card? _firstSelectedCard;
        private Card? _secondSelectedCard;
        private bool _isProcessingCards;
        private bool _isGameOver;
        private bool _isGameWon;
        private ObservableCollection<Guid> _savedGames;
        private Guid _selectedSavedGame;

        public User CurrentUser { get => _currentUser!; set => SetProperty(ref _currentUser, value); }
        public Game CurrentGame { get => _currentGame!; set { SetProperty(ref _currentGame, value); OnPropertyChanged(nameof(IsGameStarted)); } }
        public ObservableCollection<Card> Cards { get => _cards; set => SetProperty(ref _cards, value); }
        public ObservableCollection<Category> Categories { get => _categories; set => SetProperty(ref _categories, value); }
        public Category SelectedCategory { get => _selectedCategory!; set { if (SetProperty(ref _selectedCategory, value)) ((RelayCommand)NewGameCommand).RaiseCanExecuteChanged(); } }
        public int Rows { get => _rows; set { if (SetProperty(ref _rows, value)) ((RelayCommand)NewGameCommand).RaiseCanExecuteChanged(); } }
        public int Columns { get => _columns; set { if (SetProperty(ref _columns, value)) ((RelayCommand)NewGameCommand).RaiseCanExecuteChanged(); } }
        public bool IsCustomSize { get => _isCustomSize; set { if (SetProperty(ref _isCustomSize, value)) { if (!value) { Rows = 4; Columns = 4; } OnPropertyChanged(nameof(IsStandardSize)); } } }
        public bool IsStandardSize { get => !_isCustomSize; set => IsCustomSize = !value; }
        public int Minutes { get => _minutes; set => SetProperty(ref _minutes, value); }
        public int Seconds { get => _seconds; set => SetProperty(ref _seconds, value); }
        public string TimeDisplay { get => _timeDisplay; set => SetProperty(ref _timeDisplay, value); }
        public bool IsGameOver { get => _isGameOver; set => SetProperty(ref _isGameOver, value); }
        public bool IsGameWon { get => _isGameWon; set => SetProperty(ref _isGameWon, value); }
        public bool IsGameStarted => CurrentGame != null;
        public ObservableCollection<Guid> SavedGames { get => _savedGames; set => SetProperty(ref _savedGames, value); }
        public Guid SelectedSavedGame { get => _selectedSavedGame; set { if (SetProperty(ref _selectedSavedGame, value)) ((RelayCommand)OpenGameCommand).RaiseCanExecuteChanged(); } }

        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand OpenGameCommand { get; }
        public ICommand CardClickCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SelectCategoryCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand AboutCommand { get; }

        public event Action? ExitRequested;
        public event Action? StatisticsRequested;
        public event Action? AboutRequested;

        public GameViewModel(GameService gameService, UserService userService)
        {
            _gameService = gameService;
            _userService = userService;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Timer_Tick;

            _cards = new ObservableCollection<Card>();
            _categories = new ObservableCollection<Category>(_gameService.LoadCategories());
            _savedGames = new ObservableCollection<Guid>();
            _timeDisplay = "00:00";

            NewGameCommand = new RelayCommand(StartNewGame, CanStartNewGame);
            SaveGameCommand = new RelayCommand(SaveGame, CanSaveGame);
            OpenGameCommand = new RelayCommand(OpenGame, CanOpenGame);
            CardClickCommand = new RelayCommand(CardClick, CanCardClick);
            ExitCommand = new RelayCommand(Exit);
            SelectCategoryCommand = new RelayCommand(SelectCategory);
            StatisticsCommand = new RelayCommand(ShowStatistics);
            AboutCommand = new RelayCommand(ShowAbout);

            Rows = 4;
            Columns = 4;
            Minutes = 2;
            Seconds = 0;
            UpdateTimeDisplay();
        }

        public void Initialize(User user)
        {
            CurrentUser = user;
            SavedGames = new ObservableCollection<Guid>(_userService.GetSavedGames(user.Username));
        }

        private bool CanStartNewGame(object parameter) => SelectedCategory != null && (!IsCustomSize || (Rows >= 2 && Rows <= 6 && Columns >= 2 && Columns <= 6 && (Rows * Columns) % 2 == 0));

        private void StartNewGame(object parameter)
        {
            try
            {
                _timer.Stop();
                TimeSpan totalTime = new TimeSpan(0, Minutes, Seconds);
                CurrentGame = _gameService.CreateNewGame(CurrentUser.Username, SelectedCategory.Name, Rows, Columns, totalTime);
                Cards = new ObservableCollection<Card>(CurrentGame.Cards);
                _firstSelectedCard = _secondSelectedCard = null;
                _isProcessingCards = false;
                IsGameOver = IsGameWon = false;
                CurrentGame.ElapsedTime = TimeSpan.Zero;
                UpdateTimeDisplay();
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting new game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveGame(object parameter) => CurrentGame != null && !IsGameOver;

        private void SaveGame(object? parameter)
        {
            if (CurrentGame == null) return;
            try
            {
                _timer.Stop();
                CurrentGame.ElapsedTime = DateTime.Now - CurrentGame.StartTime;
                _gameService.SaveGame(CurrentGame);
                if (!SavedGames.Contains(CurrentGame.Id)) SavedGames.Add(CurrentGame.Id);
                MessageBox.Show("Game saved successfully.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanOpenGame(object parameter) => SelectedSavedGame != Guid.Empty;

        private void OpenGame(object parameter)
        {
            try
            {
                _timer.Stop();
                CurrentGame = _gameService.LoadGame(SelectedSavedGame);
                if (CurrentGame == null) return;

                Cards = new ObservableCollection<Card>(CurrentGame.Cards);
                Rows = CurrentGame.Rows;
                Columns = CurrentGame.Columns;
                IsCustomSize = !(Rows == 4 && Columns == 4);
                SelectedCategory = Categories.FirstOrDefault(c => c.Name == CurrentGame.Category) ?? new Category { Name = "Default" };

                _firstSelectedCard = _secondSelectedCard = null;
                _isProcessingCards = false;
                IsGameOver = IsGameWon = false;

                TimeSpan remainingTime = CurrentGame.TotalTime - CurrentGame.ElapsedTime;
                if (remainingTime.TotalSeconds <= 0)
                {
                    remainingTime = TimeSpan.Zero;
                    IsGameOver = true;
                }
                Minutes = remainingTime.Minutes;
                Seconds = remainingTime.Seconds;
                UpdateTimeDisplay();

                if (!IsGameOver)
                {
                    CurrentGame.StartTime = DateTime.Now - CurrentGame.ElapsedTime;
                    _timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanCardClick(object parameter) => !IsGameOver && !_isProcessingCards && parameter is Card card && !card.IsFlipped && !card.IsMatched;

        private void CardClick(object parameter)
        {
            if (parameter is not Card card) return;

            card.IsFlipped = true;
            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
            }
            else if (_secondSelectedCard == null)
            {
                _secondSelectedCard = card;
                _isProcessingCards = true;
                Application.Current.Dispatcher.BeginInvoke(new Action(ProcessCardMatch), DispatcherPriority.Background);
            }
        }

        private void ProcessCardMatch()
        {
            if (_firstSelectedCard == null || _secondSelectedCard == null) return;

            if (_firstSelectedCard.PairId == _secondSelectedCard.PairId)
            {
                _firstSelectedCard.IsMatched = _secondSelectedCard.IsMatched = true;
                if (Cards.All(c => c.IsMatched)) GameWon();
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _firstSelectedCard!.IsFlipped = false;
                    _secondSelectedCard!.IsFlipped = false;
                }), DispatcherPriority.Background, TimeSpan.FromSeconds(1));
            }

            _firstSelectedCard = _secondSelectedCard = null;
            _isProcessingCards = false;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (CurrentGame == null || IsGameOver) return;

            TimeSpan elapsed = DateTime.Now - CurrentGame.StartTime;
            CurrentGame.ElapsedTime = elapsed;
            TimeSpan remaining = CurrentGame.TotalTime - elapsed;

            if (remaining.TotalSeconds <= 0)
            {
                _timer.Stop();
                GameLost();
            }
            else
            {
                Minutes = remaining.Minutes;
                Seconds = remaining.Seconds;
                UpdateTimeDisplay();
            }
        }

        private void UpdateTimeDisplay() => TimeDisplay = $"{Minutes:D2}:{Seconds:D2}";

        private void GameWon()
        {
            _timer.Stop();
            IsGameOver = true;
            IsGameWon = true;
            _gameService.CompleteGame(CurrentGame, true);
            MessageBox.Show("Congratulations! You won the game!", "Game Won", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GameLost()
        {
            _timer.Stop();
            IsGameOver = true;
            IsGameWon = false;
            _gameService.CompleteGame(CurrentGame, false);
            MessageBox.Show("Game over! You ran out of time.", "Game Lost", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit(object parameter)
        {
            _timer.Stop();
            if (CurrentGame != null && !IsGameOver)
            {
                var result = MessageBox.Show("Do you want to save the current game before exiting?", "Save Game", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) SaveGame(null);
                else if (result == MessageBoxResult.Cancel) return;
            }
            ExitRequested?.Invoke();
        }

        private void SelectCategory(object parameter)
        {
            if (parameter is string categoryName)
                SelectedCategory = Categories.FirstOrDefault(c => c.Name == categoryName) ?? new Category { Name = categoryName };
        }

        private void ShowStatistics(object parameter)
        {
            if (CurrentGame != null && !IsGameOver) _timer.Stop();
            StatisticsRequested?.Invoke();
        }

        private void ShowAbout(object parameter)
        {
            if (CurrentGame != null && !IsGameOver) _timer.Stop();
            AboutRequested?.Invoke();
        }
    }
}