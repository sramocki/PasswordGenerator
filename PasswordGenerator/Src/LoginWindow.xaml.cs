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
            WelcomeText.Text = "Welcome "+ Environment.UserName.ToUpper();
            if (File.Exists(Account.WorkingPath))
            {
                MessageBox.Show("Data detected, please decrypt", "Data found", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("No data found, would you like to create a new key?",
                    "No data found", MessageBoxButton.YesNo, MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    KeyCreate_Click(null, null);
                }
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

        private void DecryptionByKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                if (Account.Load(KeyField.Text) == 1)
                {
                    MessageBox.Show("Data successfully loaded", "Data loaded", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    TableView table = new TableView();
                    table.Show();
                    Close();
                }
                else if (Account.Load(KeyField.Text) == -1)
                {
                    MessageBox.Show("Incorrect key", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("No data found", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void KeyCreate_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Account.WorkingPath))
            {
                MessageBox.Show("Data detected, please delete/move it before generating a new key", "Data found",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Would you like to generate a USB key?",
                    "Prompt", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var drives = DriveInfo.GetDrives()
                        .Where(drive => drive.IsReady && drive.DriveType == DriveType.Removable);
                    var driveInfos = drives.ToList();
                    if (driveInfos.Count == 1)
                    {
                        string path = driveInfos[0] + "key.ramocki";
                        if(File.Exists(path))
                        {
                            MessageBoxResult resultz = MessageBox.Show("Key already found on this USB, would you like to override it?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
                            if (resultz == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    using (var rijndael = System.Security.Cryptography.Rijndael.Create())
                                    {
                                        rijndael.GenerateKey();
                                        var key = Convert.ToBase64String(rijndael.Key);
                                        Account.CreateAccount(key);
                                        Account.Save();
                                        Clipboard.SetText(key);
                                        MessageBox.Show("Key added to " + driveInfos[0] + "\n\n" + key, "Key Generated", MessageBoxButton.OK,
                                            MessageBoxImage.None);
                                        File.WriteAllText(path, key);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                using (var rijndael = System.Security.Cryptography.Rijndael.Create())
                                {
                                    rijndael.GenerateKey();
                                    var key = Convert.ToBase64String(rijndael.Key);
                                    Account.CreateAccount(key);
                                    Account.Save();
                                    Clipboard.SetText(key);
                                    MessageBox.Show("Key added to " + driveInfos[0] + "\n\n" + key, "Key Generated", MessageBoxButton.OK,
                                        MessageBoxImage.None);
                                    File.WriteAllText(path, key);
                                }
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
                        MessageBox.Show("Multiple USB devices found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("No USB device found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    using (var rijndael = System.Security.Cryptography.Rijndael.Create())
                    {
                        rijndael.GenerateKey();
                        var key = Convert.ToBase64String(rijndael.Key);
                        Account.CreateAccount(key);
                        Account.Save();
                        Clipboard.SetText(key);
                        MessageBox.Show("Key: " + key, "Key Generated", MessageBoxButton.OK, MessageBoxImage.None);
                    }
                }
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;
            if (MessageBox.Show("Do you want to close this program", "Confirmation", buttons, icon) ==
                MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Sean Ramocki\n\nseanramocki@gmail.com", "About");
        }

        private void ImportData_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "data";
            dlg.DefaultExt = ".ramocki";
            dlg.Filter = "Stored data (.ramocki)|*.ramocki";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                Account.WorkingPath = dlg.FileName;
                Account.CurrentFile = new FileInfo(Account.WorkingPath);
            }
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
                string path = driveInfos[0] + "key.ramocki";
                if (File.Exists(path))
                {
                    try
                    {
                        string contents = File.ReadAllText(path);
                        if (!contents.Equals(""))
                        {
                            if (Account.Load(contents) == 1)
                            {
                                MessageBox.Show("Data successfully loaded", "Data loaded", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                                TableView table = new TableView();
                                table.Show();
                                Close();
                            }
                            else if (Account.Load(contents) == -1)
                            {
                                MessageBox.Show("Incorrect key", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else if(Account.Load(contents) == -2)
                            {
                                MessageBox.Show("Nothing to decrypt", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            else
                            {
                                MessageBox.Show("Unknown error", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("This file is blank!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException);
                        MessageBox.Show("This file is corrupt!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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