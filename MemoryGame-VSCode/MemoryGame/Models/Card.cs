using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MemoryGame.Models
{
    public class Card : INotifyPropertyChanged
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private bool _isFlipped;
        public bool IsFlipped
        {
            get => _isFlipped;
            set
            {
                if (SetProperty(ref _isFlipped, value))
                {
                    OnPropertyChanged(nameof(DisplayImage));
                }
            }
        }

        private bool _isMatched;
        public bool IsMatched
        {
            get => _isMatched;
            set
            {
                if (SetProperty(ref _isMatched, value))
                {
                    OnPropertyChanged(nameof(DisplayImage));
                }
            }
        }

        public int PairId { get; set; }

        public string BackImagePath { get; set; } = "Resources/Images/back.png";

        /// <summary>
        /// Returnează calea imaginii care trebuie afișată în funcție de starea cardului.
        /// </summary>
        public string DisplayImage => (IsFlipped || IsMatched) ? ImagePath : BackImagePath;

        // Constructor necesar pentru deserializare
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
                PairId = this.PairId,
                BackImagePath = this.BackImagePath
            };
        }

        // ================== INotifyPropertyChanged ===================

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
