using System;

namespace MemoryGame.Models
{
    public class Card
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int PairId { get; set; }

        public Card()
        {
        }

        public Card(string imagePath, int pairId)
        {
            ImagePath = imagePath;
            PairId = pairId;
            IsFlipped = false;
            IsMatched = false;
        }

        public Card Clone()
        {
            return new Card
            {
                Id = Guid.NewGuid(),
                ImagePath = this.ImagePath,
                IsFlipped = this.IsFlipped,
                IsMatched = this.IsMatched,
                PairId = this.PairId
            };
        }
    }
}
