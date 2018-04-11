using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PasswordGenerator.Src
{
    public partial class LoginWindow : Window
    {
        public Account Account { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            Title = "Welcome " + Environment.UserName.ToUpper();
            var exists =
                Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Ramocki\"));

            if (!exists)
                Directory.CreateDirectory(
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Ramocki\"));

            if (File.Exists(Utility.WorkingPath)) return;
            var result = MessageBox.Show("No data found, would you like to create a new key?",
                "No data found", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes) KeyCreate_Click(null, null);
        }

        private void DecryptionByKey_Key(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter) return;
            if (!Utility.Load(KeyField.Text)) return;
            var table = new TableView();
            table.Show();
            Close();
        }

        private void DecryptionByKey_Click(object sender, RoutedEventArgs e)
        {
            if (!Utility.Load(KeyField.Text)) return;
            var table = new TableView();
            table.Show();
            Close();
        }

        private void USBKeyCreate_Click(object sender, RoutedEventArgs e)
        {
            //todo move usb creation here
        }

        private void KeyCreate_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Utility.WorkingPath))
            {
                var result = MessageBox.Show(
                    "Local data already exists\n Backup data?", "Data Conflict",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    File.Move(Utility.WorkingPath, Utility.WorkingPath + "_backup" + string.Format(DateTime.Now.Ticks.ToString()));
                }
            }
            else
            {
                var result = MessageBox.Show("Would you like to generate a USB key?",
                    "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
                string key;
                using (var rijndael = System.Security.Cryptography.Rijndael.Create())
                {
                    rijndael.GenerateKey();
                    key = Convert.ToBase64String(rijndael.Key);
                }

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        var drives = DriveInfo.GetDrives()
                            .Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);
                        var driveInfos = drives.ToList();
                        if (driveInfos.Count == 1)
                        {
                            var path = driveInfos[0] + "key.ramocki";
                            if (File.Exists(path))
                            {
                                var resultz = MessageBox.Show(
                                    "Key already found on this USB, would you like to override it?", "Error",
                                    MessageBoxButton.YesNo, MessageBoxImage.Error);
                                if (resultz == MessageBoxResult.Yes)
                                    try
                                    {
                                        Utility.CreateAccount(key);
                                        Utility.Save();
                                        Clipboard.SetText(key);
                                        MessageBox.Show("Key added to " + driveInfos[0] + "\n\n" + key, "Key Generated",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.None);
                                        File.WriteAllText(path, key);
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                            }
                            else
                            {
                                try
                                {
                                    Utility.CreateAccount(key);
                                    Utility.Save();
                                    Clipboard.SetText(key);
                                    MessageBox.Show("Key added to " + driveInfos[0] + "\n\n" + key, "Key Generated",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.None);
                                    File.WriteAllText(path, key);
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                    throw;
                                }
                            }
                        }
                        else if (driveInfos.Count > 1)
                        {
                            MessageBox.Show("Multiple USB devices found", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("No USB device found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        break;
                    case MessageBoxResult.No:
                        Utility.CreateAccount(key);
                        Utility.Save();
                        Clipboard.SetText(key);

                        break;
                    case MessageBoxResult.None:
                        break;
                    case MessageBoxResult.OK:
                        break;
                    case MessageBoxResult.Cancel:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            const MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon;
            icon = MessageBoxImage.Question;
            if (MessageBox.Show("Do you want to close this program", "Confirmation", buttons, icon) ==
                MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Sean Ramocki\n\nseanramocki@gmail.com", "About");
        }

        private void ImportData_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "data",
                DefaultExt = ".ramocki",
                Filter = "Stored data (.ramocki)|*.ramocki"
            };
            var result = dlg.ShowDialog();
            if (result != true) return;
            Utility.WorkingPath = dlg.FileName;
            Utility.CurrentFile = new FileInfo(Utility.WorkingPath);
        }

        public void YourGotFocusEvent(object sender, RoutedEventArgs e)
        {
            KeyField.Text = string.Empty;
            KeyField.GotFocus -= YourGotFocusEvent;
        }

        private void USBRead_Click(object sender, RoutedEventArgs e)
        {
            var drives = DriveInfo.GetDrives()
                .Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);
            var driveInfos = drives.ToList();
            if (driveInfos.Count == 1)
            {
                var path = driveInfos[0] + "key.ramocki";

                if (File.Exists(path))
                {
                    if (!Utility.Load(File.ReadAllText(path))) return;
                    var table = new TableView();
                    table.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("No key found on this USB", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (driveInfos.Count > 1)
            {
                MessageBox.Show("Multiple USB devices found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("No USB device found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}