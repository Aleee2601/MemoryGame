using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly UserService _userService;
        private ObservableCollection<User> _users;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ICommand CloseCommand { get; }

        public event Action CloseRequested;

        public StatisticsViewModel(UserService userService)
        {
            _userService = userService;
            Users = new ObservableCollection<User>(_userService.GetAllUsers());
            
            CloseCommand = new RelayCommand(Close);
        }

        private void Close(object parameter)
        {
            CloseRequested?.Invoke();
        }
    }
}
