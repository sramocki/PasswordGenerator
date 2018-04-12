using System;
using System.Diagnostics;
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
            Title = "Welcome " + Environment.UserName;
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

            var newKeybind = new RoutedCommand();
            newKeybind.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newKeybind, KeyCreate_Click));

            var unlockKeybind = new RoutedCommand();
            unlockKeybind.InputGestures.Add(new KeyGesture(Key.U));
            CommandBindings.Add(new CommandBinding(unlockKeybind, USBRead_Click));

            var importKey = new RoutedCommand();
            importKey.InputGestures.Add(new KeyGesture(Key.I));
            CommandBindings.Add(new CommandBinding(importKey, ImportData_Click));
        }

        private void LocateData_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Path.GetDirectoryName(Utility.WorkingPath));
        }

        private void DecryptionByKey_Key(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return && e.Key != Key.Enter) return;
            Decrypt();
        }

        private void DecryptionByKey_Click(object sender, RoutedEventArgs e)
        {
            Decrypt();
        }

        private void Decrypt()
        {
            if (!Utility.Load(KeyField.Text)) return;
            var table = new TableView();
            table.Show();
            Close();
        }

        private void KeyCreate_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Utility.WorkingPath))
            {
                var result1 = MessageBox.Show( "Local data found\n\n Backup data first?", "Data Conflict",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result1 == MessageBoxResult.Yes)
                    File.Move(Utility.WorkingPath,
                        Utility.WorkingPath + "_backup" + string.Format(DateTime.Now.Ticks.ToString()));
                else if (result1 == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            var result2 = MessageBox.Show("Would you like to generate a USB key?",
                "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
            string key;
            using (var rijndael = System.Security.Cryptography.Rijndael.Create())
            {
                rijndael.GenerateKey();
                key = Convert.ToBase64String(rijndael.Key);
            }

            switch (result2)
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
                                    MessageBox.Show(exception.ToString());
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
                    MessageBox.Show("Key: " + key, "Key Generated", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to close this program", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
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