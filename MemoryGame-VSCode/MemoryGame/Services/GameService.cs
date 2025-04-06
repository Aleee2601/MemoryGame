using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace MemoryGame.Services
{
    public class GameService
    {
        private readonly FileService _fileService;
        private readonly UserService _userService;
        private readonly Random _random = new Random();

        // Predefined colors for cards
        private readonly List<Color> _colors = new List<Color>
        {
            Color.FromRgb(231, 76, 60),   // Red
            Color.FromRgb(46, 204, 113),  // Green
            Color.FromRgb(52, 152, 219),  // Blue
            Color.FromRgb(155, 89, 182),  // Purple
            Color.FromRgb(241, 196, 15),  // Yellow
            Color.FromRgb(230, 126, 34),  // Orange
            Color.FromRgb(26, 188, 156),  // Turquoise
            Color.FromRgb(236, 240, 241), // Light Gray
            Color.FromRgb(149, 165, 166), // Gray
            Color.FromRgb(211, 84, 0),    // Dark Orange
            Color.FromRgb(41, 128, 185),  // Dark Blue
            Color.FromRgb(39, 174, 96),   // Dark Green
            Color.FromRgb(142, 68, 173),  // Dark Purple
            Color.FromRgb(44, 62, 80),    // Dark Gray
            Color.FromRgb(22, 160, 133),  // Dark Turquoise
            Color.FromRgb(192, 57, 43),   // Dark Red
            Color.FromRgb(243, 156, 18),  // Dark Yellow
            Color.FromRgb(127, 140, 141)  // Medium Gray
        };

        public GameService(FileService fileService, UserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        public List<Category> LoadCategories()
        {
            // Create sample categories
            List<Category> categories = new List<Category>
            {
                new Category { Name = "Animals", ImagePaths = GenerateDummyImagePaths(18) },
                new Category { Name = "Nature", ImagePaths = GenerateDummyImagePaths(18) },
                new Category { Name = "Food", ImagePaths = GenerateDummyImagePaths(18) }
            };

            return categories;
        }

        private List<string> GenerateDummyImagePaths(int count)
        {
            // Generate dummy image paths with color codes
            List<string> paths = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Color color = _colors[i % _colors.Count];
                paths.Add($"#{color.R:X2}{color.G:X2}{color.B:X2}");
            }
            return paths;
        }

        public Game CreateNewGame(string username, string category, int rows, int columns, TimeSpan totalTime)
        {
            Game game = new Game(username, category, rows, columns, totalTime);
            
            // Generate cards
            List<Card> cards = GenerateCards(category, rows * columns);
            game.Cards = cards;
            
            return game;
        }

        private List<Card> GenerateCards(string category, int totalCards)
        {
            List<Card> cards = new List<Card>();
            List<Category> categories = LoadCategories();
            Category selectedCategory = categories.FirstOrDefault(c => c.Name == category);
            
            if (selectedCategory == null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            // Shuffle the image paths
            List<string> shuffledImages = selectedCategory.ImagePaths.OrderBy(x => _random.Next()).ToList();
            
            // Take the required number of images for pairs
            int pairsNeeded = totalCards / 2;
            List<string> selectedImages = shuffledImages.Take(pairsNeeded).ToList();
            
            // Create pairs of cards
            int pairId = 0;
            foreach (string imagePath in selectedImages)
            {
                // Create two cards with the same image
                cards.Add(new Card(imagePath, pairId));
                cards.Add(new Card(imagePath, pairId));
                pairId++;
            }
            
            // Shuffle the cards
            cards = cards.OrderBy(x => _random.Next()).ToList();
            
            return cards;
        }

        public void SaveGame(Game game)
        {
            _fileService.SaveGame(game);
            _userService.AddSavedGame(game.Username, game.Id);
        }

        public Game LoadGame(Guid gameId)
        {
            return _fileService.LoadGame(gameId);
        }

        public void DeleteGame(Guid gameId)
        {
            _fileService.DeleteGame(gameId);
        }

        public void CompleteGame(Game game, bool isWon)
        {
            game.IsCompleted = true;
            game.IsWon = isWon;
            _fileService.SaveGame(game);
            _userService.UpdateStatistics(game.Username, isWon);
        }
    }
}
