// Fisier: ViewModels/MainViewModel.cs
using MemoryGame.Commands;
using MemoryGame.Helpers;
using MemoryGame.Models;
using MemoryGame.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object currentView;
        public object CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<UserModel> Users { get; set; }
        public UserModel SelectedUser { get; set; }

        public ICommand PlayCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand CreateUserCommand { get; set; }

        public MainViewModel()
        {
            Users = LoadUsers(); // Metodă care încarcă userii din fișier JSON
            PlayCommand = new RelayCommand(ExecutePlay, CanExecutePlay);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            CreateUserCommand = new RelayCommand(ExecuteCreateUser);

            CurrentView = new SignInView();
        }

        private void ExecutePlay(object obj)
        {
            if (SelectedUser != null)
            {
                var gameVM = new GameViewModel(SelectedUser);
                CurrentView = new GameView { DataContext = gameVM };
            }
        }

        private bool CanExecutePlay(object obj) => SelectedUser != null;

        private void ExecuteDelete(object obj)
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SaveUsers();
                SelectedUser = null;
                OnPropertyChanged(nameof(Users));
            }
        }

        private bool CanExecuteDelete(object obj) => SelectedUser != null;

        private void ExecuteCreateUser(object obj)
        {
            CurrentView = new CreateUserView();
        }

        private ObservableCollection<UserModel> LoadUsers()
        {
            // TODO: încarcă din fișier JSON
            return new ObservableCollection<UserModel>();
        }

        private void SaveUsers()
        {
            // TODO: salvează în fișier JSON
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
