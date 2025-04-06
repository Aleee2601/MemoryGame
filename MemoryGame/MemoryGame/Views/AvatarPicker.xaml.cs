using System.Windows;
using MemoryGame.ViewModels;

namespace MemoryGame.Views
{
    public partial class AvatarPicker : Window
    {
        public string? SelectedAvatarPath { get; private set; }

        public AvatarPicker()
        {
            InitializeComponent();

            var viewModel = new ChooseAvatarViewModel();
            viewModel.AvatarSelected += OnAvatarSelected;

            DataContext = viewModel;
        }

        private void OnAvatarSelected(string path)
        {
            SelectedAvatarPath = path;
            DialogResult = true;
            Close();
        }
    }
}
