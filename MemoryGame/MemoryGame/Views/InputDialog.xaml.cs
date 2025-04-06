using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MemoryGame.Views
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string Username { get; private set; }

        public InputDialog()
        {
            InitializeComponent();
            InputBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Username = InputBox.Text.Trim();
            DialogResult = !string.IsNullOrEmpty(Username);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

}
