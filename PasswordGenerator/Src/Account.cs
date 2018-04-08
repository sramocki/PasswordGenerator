using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Account
    {
        private const int SaltSize = 8;
        private static Dictionary<string, Account> map = new Dictionary<string, Account>();
        private static readonly string DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Ramocki\data.ramocki");
        private static string previousPath = WorkingPath;
        public static string WorkingPath { set; get; } = DefaultPath;
        public static FileInfo CurrentFile { set; get; } = new FileInfo(WorkingPath);

        [NonSerialized]
        private static string currentPassword;

        private string username;
        public Storage Storage { get; set; }

        public Account(string username)
        {
            this.username = username;
            Storage = new Storage();
        }

        public void ResetPathPrevious()
        {
            WorkingPath = DefaultPath;
        }

        public void ResetPathDefault()
        {
            WorkingPath = previousPath;
        }

        public string ReturnPrint()
        {
            return currentPassword;
        }

        public static bool Save()
        {
            try
            {
                var keyGenerator = new Rfc2898DeriveBytes(currentPassword, SaltSize);
                var rijndael = Rijndael.Create();

                rijndael.IV = keyGenerator.GetBytes(rijndael.BlockSize / 8);
                rijndael.Key = keyGenerator.GetBytes(rijndael.KeySize / 8);

                using (var fs = new FileStream(WorkingPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(keyGenerator.Salt, 0, SaltSize);
                    using (var cryptoStream = new CryptoStream(fs, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(cryptoStream, map);
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.InnerException);
                return false;
            }
        }

        public static void CreateAccount(string key)
        {
            Account temp = new Account("home");
            map.Add("home", temp);
            currentPassword = key;
        }

        public static bool Load(string password)
        {
            try
            {
                Dictionary<string, Account> ret;
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

                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        ret = (Dictionary<string, Account>) binaryFormatter.Deserialize(cryptoStream);
                    }
                }

                currentPassword = password;
                map = ret;
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

        public static Account GetAccount()
        {
            Account value;
            if (map.TryGetValue("home", out value))
            {
                return value;

            }
            else
            {
                return null;
            }
        }
    }
}
