using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Xps.Packaging;

namespace PasswordGenerator.Src
{
    public partial class TableView : Window
    {
        public Account Account { get; set; }
        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        private string[] _currentItem;

        public TableView()
        {
            Account = Utility.Account;
            InitializeComponent();
            listTable.ItemsSource = Account.Storage.DomainList;
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            const MessageBoxButton buttons = MessageBoxButton.YesNo;
            const MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show("Save before closing?", "Confirmation", buttons, icon) == MessageBoxResult.Yes)
            {
                Utility.Save();
                Application.Current.Shutdown();
            }
            else
            {
                Application.Current.Shutdown();
            }
                
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Save before closing?", "Error", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) Utility.Save();
            else Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Sean Ramocki", "About");
        }

        private void GenerateView_Click(object sender, RoutedEventArgs e)
        {
            var generator = new Generator {TimeUpdatedField = {Text = DateTime.Now.ToShortTimeString()}};
            generator.ShowDialog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var temp = Utility.Save();
            if (temp)
                MessageBox.Show("Data saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Could not save data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "data",
                DefaultExt = ".ramocki",
                Filter = "Stored data (.ramocki)|*.ramocki"
            };
            var result = dlg.ShowDialog();

            if (result != true) return;
            Utility.WorkingPath = dlg.FileName;
            var temp = Utility.Save();
            if (temp)
                MessageBox.Show("Data saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Could not save data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Utility.ResetPathPrevious();
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
            var pDialog = new PrintDialog
            {
                PageRangeSelection = PageRangeSelection.AllPages,
                UserPageRangeEnabled = true
            };
            var print = pDialog.ShowDialog();

            if (print != true) return;
            try
            {
                var xpsDocument = new XpsDocument("C:\\FixedDocumentSequence.xps", FileAccess.ReadWrite);
                var fixedDocSeq = xpsDocument.GetFixedDocumentSequence();
                if (fixedDocSeq != null)
                    pDialog.PrintDocument(fixedDocSeq.DocumentPaginator,
                        "Encryption key: " + Utility.ReturnPrint());
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void FilterSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listTable.ItemsSource = null;
            switch ((FilterList.SelectedItem as ListViewItem)?.Content.ToString())
            {
                case "All":
                    listTable.ItemsSource = Account.Storage.DomainList;
                    break;
                case "Bank":
                    listTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Bank).ToList();
                    break;
                case "Game":
                    listTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Game).ToList();
                    break;
                case "General":
                    listTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.General).ToList();
                    break;
                case "Forum":
                    listTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Forum).ToList();
                    break;
                case "School":
                    listTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.School).ToList();
                    break;
                case "Shopping":
                    listTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Shopping)
                        .ToList();
                    break;
                case "Work":
                    listTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Work).ToList();
                    break;
                default:
                    break;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            IEditableCollectionView items = listTable.Items;
            if (items.CanRemove) items.Remove(listTable.SelectedItem);
            _currentItem = null;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var generator = new Generator {TimeUpdatedField = {Text = DateTime.Now.ToShortTimeString()}};
            generator.ShowDialog();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = (Domain)listTable.SelectedItem;
            if (selectedItem == null) return;
            Clipboard.SetText(selectedItem.Password);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_currentItem == null) return;
            IEditableCollectionView items = listTable.Items;
            if (items.CanRemove) items.Remove(listTable.SelectedItem);
            var generator = new Generator
            {
                DomainField = {Text = _currentItem[0]},
                UsernameField = {Text = _currentItem[1]},
                OutputField = {Text = _currentItem[2]},
                TimeUpdatedField = { Text = _currentItem[3] },
                CommentField = {Text = _currentItem[4]}
            };
            generator.ShowDialog();
            _currentItem = null;

        }

        private void ChangedSelection(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedItem = (Domain) listTable.SelectedItem;
            if (selectedItem == null) return;
            _currentItem = new string[5];
            _currentItem[0] = selectedItem.Address;
            _currentItem[1] = selectedItem.Login;
            _currentItem[2] = selectedItem.Password;
            _currentItem[3] = selectedItem.TimeUpdated.ToString();
            _currentItem[4] = selectedItem.Comment;
        }

        private void SortColumnClick(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader column)) return;

            if (Equals(_sortColumn, column))
            {
                _sortDirection = _sortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                if (_sortColumn != null)
                {
                    _sortColumn.Column.HeaderTemplate = null;
                    _sortColumn.Column.Width = _sortColumn.ActualWidth - 20;
                }

                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
                column.Column.Width = column.ActualWidth + 20;
            }

            if (_sortDirection == ListSortDirection.Ascending)
                column.Column.HeaderTemplate = Resources["ArrowUp"] as DataTemplate;
            else
                column.Column.HeaderTemplate = Resources["ArrowDown"] as DataTemplate;

            var header = string.Empty;
            if (_sortColumn.Column.DisplayMemberBinding is Binding b) header = b.Path.Path;

            var resultDataView = CollectionViewSource.GetDefaultView(
                listTable.ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(
                new SortDescription(header, _sortDirection));
        }
    }
}