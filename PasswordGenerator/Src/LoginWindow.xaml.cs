using System.Windows;


namespace PasswordGenerator.Src
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            Account.Load(); 
        }

        public Account Account { get; set; }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if(usernameField.Text.Equals("") || passwordField.Password.ToString().Equals(""))
            {
                //TODO
            }

            else if(Account.ValidLogin(usernameField.Text.ToString(), passwordField.Password.ToString()))
            {
                TableView tableView = new TableView(Account.GetAccount(usernameField.Text));
                tableView.Show();
                Close();

            }
            else
            {
                //TODO
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            Create win3 = new Create();
            win3.ShowDialog();
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

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Create win3 = new Create();
            win3.ShowDialog();
        }
    }
}
