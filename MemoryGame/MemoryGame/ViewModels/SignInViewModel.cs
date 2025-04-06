using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Helpers;
using MemoryGame.Models;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<UserModel> Users { get; set; } = new();

        private UserModel? _selectedUser;
        public UserModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand CreateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }

        public SignInViewModel()
        {
            LoadUsers();
            CreateUserCommand = new RelayCommand(CreateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, () => SelectedUser != null);
            PlayCommand = new RelayCommand(PlayGame, () => SelectedUser != null);
        }

        private void LoadUsers()
        {
            if (File.Exists("users.json"))
            {
                var json = File.ReadAllText("users.json");
                var list = JsonSerializer.Deserialize<List<UserModel>>(json);
                Users.Clear();
                foreach (var user in list!)
                    Users.Add(user);
            }
        }

        private void SaveUsers()
        {
            var json = JsonSerializer.Serialize(Users);
            File.WriteAllText("users.json", json);
        }

        private void CreateUser()
        {
            var inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                string newUsername = inputDialog.Username;

                if (Users.Any(u => u.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("User already exists.");
                    return;
                }

                var picker = new AvatarPicker();
                if (picker.ShowDialog() == true)
                {
                    var avatarRelativePath = picker.SelectedAvatarPath;

                    var newUser = new UserModel
                    {
                        Username = newUsername,
                        AvatarPath = avatarRelativePath
                    };

                    Users.Add(newUser);
                    SelectedUser = newUser;
                    SaveUsers();
                }
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SaveUsers();
                // TODO: șterge avatarul, salvările și statisticile
            }
        }

        private void PlayGame()
        {
            if (SelectedUser != null)
            {
                MessageBox.Show($"Playing as {SelectedUser.Username}!");
                // TODO: navighează către fereastra de joc
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
