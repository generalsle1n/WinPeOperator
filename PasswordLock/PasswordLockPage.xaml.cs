using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
namespace PasswordLock
{
    /// <summary>
    /// Interaktionslogik für PasswordLockPage.xaml
    /// </summary>
    
    public partial class PasswordLockPage : Window
    {
        private string installPassword = string.Empty;
        private int trys = 1;
        private const int maxWrongPassword = 5;
        private const string wpeName = "wpeutil.exe";
        private const string wpeAction = "shutdown";

        public PasswordLockPage(string Secret)
        {
            installPassword = Secret;
            InitializeComponent();
        }

        private void checkPassword_Click(object sender, RoutedEventArgs e)
        {
            string password = passwordBox.Password.ToString();

            if (password.Equals(installPassword))
            {
                Environment.Exit(0);
            }
            else
            {
                if (trys == maxWrongPassword)
                {
                    Process wpe = new Process();
                    wpe.StartInfo.FileName = wpeName;
                    wpe.StartInfo.Arguments = wpeAction;

                    wpe.Start();
                    wpe.WaitForExit();
                }
                passwordLabel.Content = $"{trys}. Wrong Password";
                trys++;

            }
        }
    }
}
