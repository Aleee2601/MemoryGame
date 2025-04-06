using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace MemoryGame.Services
{
    public class FileService
    {
        private readonly string _usersFilePath;
        private readonly string _gamesDirectoryPath;
        private readonly string _statisticsFilePath;

        public FileService()
        {
            string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _usersFilePath = Path.Combine(appDataPath, "users.json");
            _gamesDirectoryPath = Path.Combine(appDataPath, "Games");
            _statisticsFilePath = Path.Combine(appDataPath, "statistics.json");

            if (!Directory.Exists(_gamesDirectoryPath))
            {
                Directory.CreateDirectory(_gamesDirectoryPath);
            }
        }

        public List<User> LoadUsers()
        {
            try
            {
                if (File.Exists(_usersFilePath))
                {
                    string json = File.ReadAllText(_usersFilePath);
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return new List<User>();
        }

        public void SaveUsers(List<User> users)
        {
            try
            {
                string json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_usersFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveGame(Game game)
        {
            try
            {
                string filePath = Path.Combine(_gamesDirectoryPath, $"{game.Id}.json");
                string json = JsonSerializer.Serialize(game, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Game LoadGame(Guid gameId)
        {
            try
            {
                string filePath = Path.Combine(_gamesDirectoryPath, $"{gameId}.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    return JsonSerializer.Deserialize<Game>(json, options);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return null;
        }

        public void DeleteGame(Guid gameId)
        {
            try
            {
                string filePath = Path.Combine(_gamesDirectoryPath, $"{gameId}.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting game: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteAllUserGames(string username)
        {
            try
            {
                List<User> users = LoadUsers();
                User user = users.Find(u => u.Username == username);
                
                if (user != null)
                {
                    foreach (Guid gameId in user.SavedGames)
                    {
                        DeleteGame(gameId);
                    }
                    
                    user.SavedGames.Clear();
                    SaveUsers(users);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user games: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveStatistics(List<User> users)
        {
            try
            {
                string json = JsonSerializer.Serialize(users.ConvertAll(u => new { u.Username, u.GamesPlayed, u.GamesWon }), 
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_statisticsFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
