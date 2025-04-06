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

        public required User _currentUser;
        public required Game _currentGame;
        public required ObservableCollection<Card> _cards;
        public required ObservableCollection<Category> _categories;
        public required Category _selectedCategory;
        private int _rows;
        private int _columns;
        private bool _isCustomSize;
        private int _minutes;
        private int _seconds;
        public required string _timeDisplay;
        private Card? _firstSelectedCard;
        private Card? _secondSelectedCard;
        private bool _isProcessingCards;
        private bool _isGameOver;
        private bool _isGameWon;
        public required ObservableCollection<Guid> _savedGames;
        private Guid _selectedSavedGame;

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public Game CurrentGame
        {
            get => _currentGame;
            set => SetProperty(ref _currentGame, value);
        }

        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set => SetProperty(ref _cards, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    ((RelayCommand)NewGameCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                if (SetProperty(ref _rows, value))
                {
                    ((RelayCommand)NewGameCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public int Columns
        {
            get => _columns;
            set
            {
                if (SetProperty(ref _columns, value))
                {
                    ((RelayCommand)NewGameCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsCustomSize
        {
            get => _isCustomSize;
            set
            {
                if (SetProperty(ref _isCustomSize, value))
                {
                    if (!_isCustomSize)
                    {
                        Rows = 4;
                        Columns = 4;
                    }
                    OnPropertyChanged(nameof(IsStandardSize));
                }
            }
        }

        public bool IsStandardSize
        {
            get => !_isCustomSize;
            set => IsCustomSize = !value;
        }

        public int Minutes
        {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }

        public int Seconds
        {
            get => _seconds;
            set => SetProperty(ref _seconds, value);
        }

        public string TimeDisplay
        {
            get => _timeDisplay;
            set => SetProperty(ref _timeDisplay, value);
        }

        public bool IsGameOver
        {
            get => _isGameOver;
            set => SetProperty(ref _isGameOver, value);
        }

        public bool IsGameWon
        {
            get => _isGameWon;
            set => SetProperty(ref _isGameWon, value);
        }

        public ObservableCollection<Guid> SavedGames
        {
            get => _savedGames;
            set => SetProperty(ref _savedGames, value);
        }

        public Guid SelectedSavedGame
        {
            get => _selectedSavedGame;
            set
            {
                if (SetProperty(ref _selectedSavedGame, value))
                {
                    ((RelayCommand)OpenGameCommand).RaiseCanExecuteChanged();
                }
            }
        }

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

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            _cards = new ObservableCollection<Card>();
            _categories = new ObservableCollection<Category>(_gameService.LoadCategories());
            _savedGames = new ObservableCollection<Guid>();
            _timeDisplay = "00:00";

            Rows = 4;
            Columns = 4;
            Minutes = 2;
            Seconds = 0;
            UpdateTimeDisplay();

            NewGameCommand = new RelayCommand(StartNewGame, CanStartNewGame);
            SaveGameCommand = new RelayCommand(SaveGame, CanSaveGame);
            OpenGameCommand = new RelayCommand(OpenGame, CanOpenGame);
            CardClickCommand = new RelayCommand(CardClick, CanCardClick);
            ExitCommand = new RelayCommand(Exit);
            SelectCategoryCommand = new RelayCommand(SelectCategory);
            StatisticsCommand = new RelayCommand(ShowStatistics);
            AboutCommand = new RelayCommand(ShowAbout);
        }

        public void Initialize(User user)
        {
            CurrentUser = user;
            SavedGames = new ObservableCollection<Guid>(_userService.GetSavedGames(user.Username));
        }

        private bool CanStartNewGame(object parameter)
        {
            if (SelectedCategory == null)
                return false;

            if (IsCustomSize)
            {
                // Check if rows and columns are valid
                if (Rows < 2 || Rows > 6 || Columns < 2 || Columns > 6)
                    return false;

                // Check if total cards is even
                if ((Rows * Columns) % 2 != 0)
                    return false;
            }

            return true;
        }

        private void StartNewGame(object parameter)
        {
            try
            {
                // Stop any existing timer
                _timer.Stop();

                // Create a new game
                TimeSpan totalTime = new TimeSpan(0, Minutes, Seconds);
                CurrentGame = _gameService.CreateNewGame(
                    CurrentUser.Username,
                    SelectedCategory.Name,
                    Rows,
                    Columns,
                    totalTime);

                // Initialize the cards collection
                Cards = new ObservableCollection<Card>(CurrentGame.Cards);

                // Reset game state
                _firstSelectedCard = null;
                _secondSelectedCard = null;
                _isProcessingCards = false;
                IsGameOver = false;
                IsGameWon = false;

                // Start the timer
                CurrentGame.ElapsedTime = TimeSpan.Zero;
                UpdateTimeDisplay();
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting new game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSaveGame(object parameter)
        {
            return CurrentGame != null && !IsGameOver;
        }

        private void SaveGame(object? parameter)
        {
            if (CurrentGame != null)
            {
                try
                {
                    // Pause the timer
                    _timer.Stop();

                    // Update the elapsed time
                    CurrentGame.ElapsedTime = DateTime.Now - CurrentGame.StartTime;

                    // Save the game
                    _gameService.SaveGame(CurrentGame);

                    // Update the saved games list
                    if (!SavedGames.Contains(CurrentGame.Id))
                    {
                        SavedGames.Add(CurrentGame.Id);
                    }

                    MessageBox.Show("Game saved successfully.", "Save Game", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanOpenGame(object parameter)
        {
            return SelectedSavedGame != Guid.Empty;
        }

        private void OpenGame(object parameter)
        {
            try
            {
                // Stop any existing timer
                _timer.Stop();

                // Load the saved game
                CurrentGame = _gameService.LoadGame(SelectedSavedGame);

                if (CurrentGame != null)
                {
                    // Initialize the cards collection
                    Cards = new ObservableCollection<Card>(CurrentGame.Cards);

                    // Set the game parameters
                    Rows = CurrentGame.Rows;
                    Columns = CurrentGame.Columns;
                    IsCustomSize = !(Rows == 4 && Columns == 4);

                    // Set the category
                    SelectedCategory = Categories.FirstOrDefault(c => c.Name == CurrentGame.Category) ?? new Category();

                    // Reset game state
                    _firstSelectedCard = null;
                    _secondSelectedCard = null;
                    _isProcessingCards = false;
                    IsGameOver = false;
                    IsGameWon = false;

                    // Calculate remaining time
                    TimeSpan remainingTime = CurrentGame.TotalTime - CurrentGame.ElapsedTime;
                    if (remainingTime.TotalSeconds <= 0)
                    {
                        remainingTime = TimeSpan.Zero;
                        IsGameOver = true;
                    }

                    Minutes = remainingTime.Minutes;
                    Seconds = remainingTime.Seconds;
                    UpdateTimeDisplay();

                    // Start the timer if the game is not over
                    if (!IsGameOver)
                    {
                        CurrentGame.StartTime = DateTime.Now - CurrentGame.ElapsedTime;
                        _timer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanCardClick(object parameter)
        {
            if (IsGameOver || _isProcessingCards)
                return false;

            if (parameter is not Card card)
                return false;

            return !card.IsFlipped && !card.IsMatched;
        }

        private void CardClick(object parameter)
        {
            if (parameter is not Card card)
                return;

            // Flip the card
            card.IsFlipped = true;

            // Process the card selection
            if (_firstSelectedCard == null)
            {
                _firstSelectedCard = card;
            }
            else if (_secondSelectedCard == null)
            {
                _secondSelectedCard = card;

                // Check for a match
                _isProcessingCards = true;

                // Use the dispatcher to delay the processing
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ProcessCardMatch();
                }), DispatcherPriority.Background);
            }
        }

        private void ProcessCardMatch()
        {
            if (_firstSelectedCard != null && _secondSelectedCard != null)
            {
                if (_firstSelectedCard.PairId == _secondSelectedCard.PairId)
                {
                    // Match found
                    _firstSelectedCard.IsMatched = true;
                    _secondSelectedCard.IsMatched = true;

                    // Check if all cards are matched
                    if (Cards.All(c => c.IsMatched))
                    {
                        GameWon();
                    }
                }
                else
                {
                    // No match, flip cards back
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _firstSelectedCard.IsFlipped = false;
                        _secondSelectedCard.IsFlipped = false;
                    }), DispatcherPriority.Background, new TimeSpan(0, 0, 1));
                }

                // Reset selected cards
                _firstSelectedCard = null;
                _secondSelectedCard = null;
                _isProcessingCards = false;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (CurrentGame != null && !IsGameOver)
            {
                // Calculate elapsed time
                TimeSpan elapsed = DateTime.Now - CurrentGame.StartTime;
                CurrentGame.ElapsedTime = elapsed;

                // Calculate remaining time
                TimeSpan remaining = CurrentGame.TotalTime - elapsed;

                if (remaining.TotalSeconds <= 0)
                {
                    // Game over
                    _timer.Stop();
                    GameLost();
                }
                else
                {
                    // Update time display
                    Minutes = remaining.Minutes;
                    Seconds = remaining.Seconds;
                    UpdateTimeDisplay();
                }
            }
        }

        private void UpdateTimeDisplay()
        {
            TimeDisplay = $"{Minutes:D2}:{Seconds:D2}";
        }

        private void GameWon()
        {
            _timer.Stop();
            IsGameOver = true;
            IsGameWon = true;

            // Update statistics
            _gameService.CompleteGame(CurrentGame, true);

            MessageBox.Show("Congratulations! You won the game!", "Game Won", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GameLost()
        {
            _timer.Stop();
            IsGameOver = true;
            IsGameWon = false;

            // Update statistics
            _gameService.CompleteGame(CurrentGame, false);

            MessageBox.Show("Game over! You ran out of time.", "Game Lost", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit(object parameter)
        {
            // Stop the timer
            _timer.Stop();

            // If a game is in progress, ask to save
            if (CurrentGame != null && !IsGameOver)
            {
                MessageBoxResult result = MessageBox.Show(
                    "Do you want to save the current game before exiting?",
                    "Save Game",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SaveGame(null!);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            ExitRequested?.Invoke();
        }

        private void SelectCategory(object parameter)
        {
            if (parameter is string categoryName)
            {
                Category category = Categories.FirstOrDefault(c => c.Name == categoryName) ?? new Category();
                if (category != null)
                {
                    SelectedCategory = category;
                }
            }
        }

        private void ShowStatistics(object parameter)
        {
            // Pause the timer if a game is in progress
            if (CurrentGame != null && !IsGameOver)
            {
                _timer.Stop();
            }

            StatisticsRequested?.Invoke();
        }

        private void ShowAbout(object parameter)
        {
            // Pause the timer if a game is in progress
            if (CurrentGame != null && !IsGameOver)
            {
                _timer.Stop();
            }

            AboutRequested?.Invoke();
        }
    }
}
