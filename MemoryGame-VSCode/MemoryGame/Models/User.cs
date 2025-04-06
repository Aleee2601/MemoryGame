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

        // This constructor is needed for deserialization
        public User()
        {
            Username = string.Empty;
            ProfileImagePath = string.Empty;
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
