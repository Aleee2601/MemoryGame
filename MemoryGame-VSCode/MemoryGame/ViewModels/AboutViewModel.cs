using MemoryGame.Commands;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private string _studentName;
        private string _email;
        private string _group;
        private string _specialization;

        public string StudentName
        {
            get => _studentName;
            set => SetProperty(ref _studentName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }

        public string Specialization
        {
            get => _specialization;
            set => SetProperty(ref _specialization, value);
        }

        public ICommand SendEmailCommand { get; }
        public ICommand CloseCommand { get; }

        public event Action CloseRequested;

        public AboutViewModel()
        {
            StudentName = "Your Name"; // Replace with your name
            Email = "your.email@student.unitbv.ro"; // Replace with your email
            Group = "Your Group"; // Replace with your group
            Specialization = "Your Specialization"; // Replace with your specialization
            
            SendEmailCommand = new RelayCommand(SendEmail);
            CloseCommand = new RelayCommand(Close);
        }

        private void SendEmail(object parameter)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"mailto:{Email}",
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {
                // Handle exception if email client cannot be opened
            }
        }

        private void Close(object parameter)
        {
            CloseRequested?.Invoke();
        }
    }
}
