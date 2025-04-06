using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MemoryGame.Helpers;

namespace MemoryGame.ViewModels
{
    public class ChooseAvatarViewModel
    {
        public ObservableCollection<string> Avatars { get; }
        public ICommand SelectAvatarCommand { get; }

        // Event ce notifică fereastra că a fost selectat un avatar
        public event Action<string>? AvatarSelected;

        public ChooseAvatarViewModel()
        {
            Avatars = new ObservableCollection<string>
            {
                "Resources/Avatars/cat.jpeg",
                "Resources/Avatars/dog.jpeg",
                "Resources/Avatars/fox.jpeg"
            };

            SelectAvatarCommand = new RelayCommand<string>(SelectAvatar);
        }

        private void SelectAvatar(string imagePath)
        {
            AvatarSelected?.Invoke(imagePath);
        }
    }
}
