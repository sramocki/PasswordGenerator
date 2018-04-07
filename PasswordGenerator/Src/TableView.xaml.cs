using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        public TableView()
        {
            Account = Account.GetAccount();
            InitializeComponent();
            listTable.ItemsSource = Account.Storage.DomainList;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (File.Exists(Account.WorkingPath))
            {
                //Account.CurrentFile.Attributes |= FileAttributes.Hidden;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (File.Exists(Account.WorkingPath))
            {
                //Account.CurrentFile.Attributes |= FileAttributes.Hidden;
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
            bool temp = Account.Save();
            if (temp)
            {
                MessageBox.Show("Data saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show("Could not save data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            //todo logout
        }

        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "data";
            dlg.DefaultExt = ".ramocki";
            dlg.Filter = "Stored data (.ramocki)|*.ramocki";
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                Account.WorkingPath = dlg.FileName;
                bool temp = Account.Save();
                if (temp)
                {
                    MessageBox.Show("Data saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("Could not save data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }

                Account.ResetPathPrevious();
            }
        }
        
        public void RefreshList()
        {
            listTable.ItemsSource = null;
            listTable.ItemsSource = Account.Storage.DomainList;
        }

        public void Modify(Domain domain)
        {
            Account.Storage.DomainList.Add(domain);
            RefreshList();
        }

        public void PrintData_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.PrintDialog dlg = new System.Windows.Controls.PrintDialog();
            dlg.PageRangeSelection = PageRangeSelection.AllPages;
            dlg.UserPageRangeEnabled = true;

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Print document
            }

        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listTable.ItemsSource = null;
            switch ((FilterList.SelectedItem as ListViewItem)?.Content.ToString())
            {
                case "All":
                    listTable.ItemsSource = Account.Storage.DomainList;
                    break;
                case "Bank":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Bank).ToList();
                    break;
                case "Game":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Game).ToList();
                    break;
                case "General":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.General).ToList();
                    break;
                case "Forum":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Forum).ToList();
                    break;
                case "School":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.School).ToList();
                    break;
                case "Shopping":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Shopping).ToList();
                    break;
                case "Work":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Work).ToList();
                    break;
                default:
                    Console.WriteLine("Something went wrong here...");
                    break;
            }
        }
    }
}
