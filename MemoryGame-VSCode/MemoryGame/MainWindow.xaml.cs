using System;
using System.Windows;
using System.Windows.Threading;
using MemoryGame.ViewModels;

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            DataContext = ViewModel;

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();
                LoadingGrid.Visibility = Visibility.Collapsed;

                // Nu mai seta alt LoginViewModel, lasă-l pe cel deja setat în MainViewModel
            };

            timer.Start();
        }
    }
}
