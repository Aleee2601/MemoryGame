using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class User
    {
        public string Username { get; set; }
        public string ProfileImagePath { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public List<Guid> SavedGames { get; set; } = new List<Guid>();

        public User()
        {
        }

        public User(string username, string profileImagePath)
        {
            Username = username;
            ProfileImagePath = profileImagePath;
            GamesPlayed = 0;
            GamesWon = 0;
        }
    }
}
