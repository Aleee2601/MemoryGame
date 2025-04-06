# Memory Game

A classic Memory Game implemented in C# and WPF using the MVVM pattern.

## Features

### User Management
- Create new users with profile images
- Select existing users
- Delete users and their associated data

### Game Play
- Standard (4x4) and custom (MxN) game boards
- Three predefined categories: Animals, Nature, and Food
- Configurable game timer
- Card flipping and matching logic
- Win/loss conditions

### Game Persistence
- Save game state (configuration, time remaining, etc.)
- Load saved games
- User-specific saved games

### Statistics
- Track games played and won for each user
- View statistics for all users

## Technical Implementation

### MVVM Architecture
- **Models**: User, Game, Card, Category
- **ViewModels**: LoginViewModel, GameViewModel, StatisticsViewModel, AboutViewModel
- **Views**: LoginView, GameView, StatisticsView, AboutView
- **Services**: FileService, UserService, GameService

### Data Binding
- ObservableCollection for dynamic UI updates
- INotifyPropertyChanged implementation in ViewModels
- Two-way binding for user inputs

### Commands
- ICommand implementation (RelayCommand) for all user actions
- Command binding in XAML

### File Storage
- JSON serialization for user data, game saves, and statistics
- Relative paths for image storage

## How to Run

1. Clone the repository
2. Open the solution in Visual Studio or Visual Studio Code
3. Build and run the application

## Requirements

- .NET 9.0 or later
- Windows operating system

## Screenshots

(Screenshots will be added here)

## Credits

Created as a project for the Advanced Programming Methods course at Transilvania University of Brasov.
