using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly UserService _userService;
        private readonly FileService _fileService;
        
        private string _newUsername;
        private string _selectedProfileImagePath;
        private User _selectedUser;
        private ObservableCollection<User> _users;

        public string NewUsername
        {
            get => _newUsername;
            set => SetProperty(ref _newUsername, value);
        }

        public string SelectedProfileImagePath
        {
            get => _selectedProfileImagePath;
            set => SetProperty(ref _selectedProfileImagePath, value);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    ((RelayCommand)DeleteUserCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)PlayCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ICommand CreateUserCommand { get; }
        public ICommand BrowseImageCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }

        public event Action<User> PlayRequested;

        public LoginViewModel(UserService userService, FileService fileService)
        {
            _userService = userService;
            _fileService = fileService;
            
            Users = new ObservableCollection<User>(_userService.GetAllUsers());
            
            CreateUserCommand = new RelayCommand(CreateUser, CanCreateUser);
            BrowseImageCommand = new RelayCommand(BrowseImage);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
            PlayCommand = new RelayCommand(Play, CanPlay);
        }

        private bool CanCreateUser(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(SelectedProfileImagePath);
        }

        private void CreateUser(object parameter)
        {
            try
            {
                // Create a relative path for the profile image
                string fileName = Path.GetFileName(SelectedProfileImagePath);
                string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData", "ProfileImages");
                
                if (!Directory.Exists(appDataPath))
                {
                    Directory.CreateDirectory(appDataPath);
                }
                
                string destinationPath = Path.Combine(appDataPath, fileName);
                
                // Copy the image file if it's not already in the app data folder
                if (!SelectedProfileImagePath.StartsWith(appDataPath))
                {
                    File.Copy(SelectedProfileImagePath, destinationPath, true);
                }
                
                // Create a relative path for storage
                string relativePath = Path.Combine("AppData", "ProfileImages", fileName);
                
                User newUser = new User(NewUsername, relativePath);
                bool success = _userService.AddUser(newUser);
                
                if (success)
                {
                    Users.Add(newUser);
                    NewUsername = string.Empty;
                    SelectedProfileImagePath = string.Empty;
                }
                else
                {
                    MessageBox.Show("A user with this username already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BrowseImage(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif",
                Title = "Select a profile image"
            };
            
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedProfileImagePath = openFileDialog.FileName;
            }
        }

        private bool CanDeleteUser(object parameter)
        {
            return SelectedUser != null;
        }

        private void DeleteUser(object parameter)
        {
            if (SelectedUser != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the user '{SelectedUser.Username}'? This will also delete all saved games and statistics.",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    bool success = _userService.DeleteUser(SelectedUser.Username);
                    if (success)
                    {
                        Users.Remove(SelectedUser);
                        SelectedUser = null;
                    }
                }
            }
        }

        private bool CanPlay(object parameter)
        {
            return SelectedUser != null;
        }

        private void Play(object parameter)
        {
            PlayRequested?.Invoke(SelectedUser);
        }
    }
}
