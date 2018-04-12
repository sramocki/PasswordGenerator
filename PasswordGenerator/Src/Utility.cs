using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows;

namespace PasswordGenerator.Src
{
    public class Utility
    {
        private const int SaltSize = 8;
        private static readonly string DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Ramocki\data.ramocki");
        public static string WorkingPath { set; get; } = DefaultPath;
        public static FileInfo CurrentFile { set; get; } = new FileInfo(WorkingPath);
        public static Account Account { get; set; }

        [NonSerialized]
        private static string _currentPassword;

        public static string ReturnPrint()
        {
            return _currentPassword;
        }

        public static bool Save()
        {
            try
            {
                var keyGenerator = new Rfc2898DeriveBytes(_currentPassword, SaltSize);
                var rijndael = Rijndael.Create();

                rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
                rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

                using (var fs = new FileStream(WorkingPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(keyGenerator.Salt, 0, SaltSize);
                    using (var cryptoStream = new CryptoStream(fs, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(cryptoStream, Account);
                    }
                }
                return true;
            }
            catch (SerializationException)
            {
                MessageBox.Show("Serialization Error");
                return false;
            }
        }

        public static void CreateAccount(string key)
        {
            var temp = new Account();
            Account = temp;
            _currentPassword = key;
        }

        public static bool Load(string password)
        {
            try
            {
                Account tempAccount;
                var salt = new byte[SaltSize];
                using (Stream fs = new FileStream(WorkingPath, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(salt, 0, SaltSize);
                    var keyGenerator = new Rfc2898DeriveBytes(password, salt);
                    var rijndael = Rijndael.Create();
                    rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
                    rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);
                    using (var cryptoStream = new CryptoStream(fs, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
                    {

                        if (fs.Length.Equals(0))
                        {
                            MessageBox.Show("Nothing to decrypt", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }

                        var binaryFormatter = new BinaryFormatter();
                        tempAccount = (Account)binaryFormatter.Deserialize(cryptoStream);
                    }
                }

                _currentPassword = password;
                Account = tempAccount;
                MessageBox.Show("Data successfully loaded", "Data loaded", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return true;
            }
            catch (SerializationException ex)
            {
                Console.WriteLine(ex.InnerException);
                MessageBox.Show("Unable to decrypt this file", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.InnerException);
                MessageBox.Show("Nothing to decrypt", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(ex.InnerException);
                MessageBox.Show("Incorrect key", "Data error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
