using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace PasswordGenerator.Src
{
    public partial class TableView : Window
    {
        public Account Account { get; set; }
        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        private string[] _currentItem;
        private bool _modified;

        public TableView()
        {
            Account = Utility.Account;
            InitializeComponent();
            ListTable.ItemsSource = Account.Storage.DomainList.OrderBy(p => p.Address).ToList();

            var saveKeybind = new RoutedCommand();
            saveKeybind.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(saveKeybind, Save_Click));

            var addEventBind = new RoutedCommand();
            addEventBind.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(addEventBind, Add_Click));

            var exportBind = new RoutedCommand();
            exportBind.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(exportBind, ExportData_Click));

            var printBind = new RoutedCommand();
            printBind.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(printBind, PrintData_Click));
            int numOutdated = Account.Storage.SearchOutdated();
            if(numOutdated > 0)
            {
                MessageBox.Show("You have " + numOutdated + " passwords that are over 90 days old!", "Outdated passwords", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            if (_modified)
                if (MessageBox.Show("Save before closing?", "Confirmation", MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Utility.Save();
                    Application.Current.Shutdown();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            else
                Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_modified)
            {
                var result = MessageBox.Show("Save before closing?", "Confirmation", MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) Utility.Save();
                else Application.Current.Shutdown();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Sean Ramocki", "About");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var temp = Utility.Save();
            if (temp)
            {
                MessageBox.Show("Data saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _modified = false;
            }

            else
            {
                MessageBox.Show("Could not save data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }        
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
                MessageBox.Show("Data exported", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show("Could not export data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void RefreshList()
        {
            ListTable.ItemsSource = null;
            ListTable.ItemsSource = Account.Storage.DomainList.OrderBy(p => p.Address).ToList();
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
            ListTable.ItemsSource = null;
            switch ((FilterList.SelectedItem as ListViewItem)?.Content.ToString())
            {
                case "All":
                    ListTable.ItemsSource = Account.Storage.DomainList.OrderBy(p => p.Address).ToList();
                    break;
                case "Bank":
                    ListTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Bank).OrderBy(p => p.Address).ToList();
                    break;
                case "Game":
                    ListTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Game).OrderBy(p => p.Address).ToList();
                    break;
                case "General":
                    ListTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.General).OrderBy(p => p.Address).ToList();
                    break;
                case "Forum":
                    ListTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Forum).OrderBy(p => p.Address).ToList();
                    break;
                case "School":
                    ListTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.School).OrderBy(p => p.Address).ToList(); ;
                    break;
                case "Shopping":
                    ListTable.ItemsSource = Account.Storage.DomainList.Where(domain => domain.Type == Type.Shopping).OrderBy(p => p.Address).ToList();
                    break;
                case "Work":
                    ListTable.ItemsSource =
                        Account.Storage.DomainList.Where(domain => domain.Type == Type.Work).OrderBy(p => p.Address).ToList();
                    break;
                default:
                    break;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var generator = new Generator
            {
                TimeUpdatedField = {Text = DateTime.Now.ToString(CultureInfo.InvariantCulture)}
            };
            generator.ShowDialog();
            if (generator.Domain == null) return;
            Account.Storage.DomainList.Add(generator.Domain);
            RefreshList();
            _modified = true;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            IEditableCollectionView items = ListTable.Items;
            if (items.CanRemove) items.Remove(ListTable.SelectedItem);
            _currentItem = null;
            _modified = true;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (_currentItem == null) return;
            var generator = new Generator
            {
                DomainField = {Text = _currentItem[0]},
                UsernameField = {Text = _currentItem[1]},
                OutputField = {Text = _currentItem[2]},
                TimeUpdatedField = {Text = _currentItem[3]},
                CommentField = {Text = _currentItem[4]},
                TypeSelector = {Text = _currentItem[5]}
            };
            generator.ShowDialog();
            if (generator.Domain == null) return;
            Account.Storage.DomainList.Add(generator.Domain);
            IEditableCollectionView items = ListTable.Items;
            if (items.CanRemove) items.Remove(ListTable.SelectedItem);
            RefreshList();
            _modified = true;
            _currentItem = null;
        }

        private void ChangedSelection(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedItem = (Domain) ListTable.SelectedItem;
            if (selectedItem == null) return;
            _currentItem = new string[6];
            _currentItem[0] = selectedItem.Address;
            _currentItem[1] = selectedItem.Login;
            _currentItem[2] = selectedItem.Password;
            _currentItem[3] = selectedItem.TimeUpdated.ToString();
            _currentItem[4] = selectedItem.Comment;
            _currentItem[5] = selectedItem.Type.ToString();
        }

        private void CopyLogin_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = (Domain)ListTable.SelectedItem;
            if (selectedItem == null) return;
            Clipboard.SetText(selectedItem.Password);
            System.Threading.Thread.Sleep(10000);
            Clipboard.SetText("Empty!");
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = (Domain) ListTable.SelectedItem;
            if (selectedItem == null) return;
            Clipboard.SetText(selectedItem.Password);
            System.Threading.Thread.Sleep(10000);
            Clipboard.SetText("Empty!");
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
                ListTable.ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(
                new SortDescription(header, _sortDirection));
        }
    }
}