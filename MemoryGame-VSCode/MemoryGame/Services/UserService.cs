using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MemoryGame.Services
{
    public class UserService
    {
        private readonly FileService _fileService;
        private List<User> _users;

        public UserService(FileService fileService)
        {
            _fileService = fileService;
            _users = _fileService.LoadUsers();
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }

        public User GetUser(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public bool AddUser(User user)
        {
            if (_users.Any(u => u.Username == user.Username))
            {
                return false;
            }

            _users.Add(user);
            _fileService.SaveUsers(_users);
            return true;
        }

        public bool DeleteUser(string username)
        {
            User user = _users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return false;
            }

            _fileService.DeleteAllUserGames(username);
            _users.Remove(user);
            _fileService.SaveUsers(_users);
            _fileService.SaveStatistics(_users);
            return true;
        }

        public void UpdateStatistics(string username, bool isWon)
        {
            User user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                user.GamesPlayed++;
                if (isWon)
                {
                    user.GamesWon++;
                }

                _fileService.SaveUsers(_users);
                _fileService.SaveStatistics(_users);
            }
        }

        public void AddSavedGame(string username, Guid gameId)
        {
            User user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                if (!user.SavedGames.Contains(gameId))
                {
                    user.SavedGames.Add(gameId);
                }

                _fileService.SaveUsers(_users);
            }
        }

        public List<Guid> GetSavedGames(string username)
        {
            User user = _users.FirstOrDefault(u => u.Username == username);
            return user?.SavedGames ?? new List<Guid>();
        }
    }
}
