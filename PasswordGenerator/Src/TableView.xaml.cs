using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class TableView : Window
    {
        public Account Account { get; set; }

        public TableView(Account passedAccount)
        {
            Account = passedAccount;
            InitializeComponent();
            Console.WriteLine(Account.Storage.DomainList.Count);
            Console.WriteLine(Account.Test.Number);
            foreach(Domain dm in Account.Storage.DomainList)
            {
                listTable.Items.Add(dm);
            }
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

        private void GenerateView_Click(object sender, RoutedEventArgs e)
        {
            Generator generator = new Generator();
            generator.ShowDialog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Account.Save();
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Account.Test.Number = 6;
        }
        
        public void RefreshList(Domain domain)
        {

            listTable.Items.Add(domain);
        }
    }
}
