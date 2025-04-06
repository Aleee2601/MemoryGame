using System;
using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public DateTime StartTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsWon { get; set; }

        // This constructor is needed for deserialization
        public Game()
        {
            StartTime = DateTime.Now;
            Username = string.Empty;
            Category = string.Empty;
        }

        public Game(string username, string category, int rows, int columns, TimeSpan totalTime)
        {
            Username = username;
            Category = category;
            Rows = rows;
            Columns = columns;
            TotalTime = totalTime;
            StartTime = DateTime.Now;
            ElapsedTime = TimeSpan.Zero;
            IsCompleted = false;
            IsWon = false;
        }
    }
}
