using System;
using System.Collections.Generic;
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

namespace PasswordGenerator.Src
{
    public partial class Create : Window
    {
        public Create()
        {
            InitializeComponent();
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show("Do you want to close this program", "Confirmation", buttons, icon) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Sean Ramocki", "About");
        }

        private void CreateAcc_Click(object sender, RoutedEventArgs e)
        {
            if (usernameField.Equals("") || passwordField1.Password.ToString().Equals(""))
            {
                MessageBox.Show("Fields can't be left blank", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Account.Exists(usernameField.Text))
            {
                MessageBox.Show("This account already exists", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (passwordField1.Password.ToString().Equals(passwordField2.Password.ToString()))
            {
                Account.CreateAccount(usernameField.Text, passwordField1.Password.ToString());
                MessageBox.Show("Account created", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Close();
            }


        }
    }
}
