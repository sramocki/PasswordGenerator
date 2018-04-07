using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

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
                //CurrentFile.Attributes |= FileAttributes.Hidden;
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

        public static int Load(string password)
        {
            try
            {
                //CurrentFile.Attributes &= ~FileAttributes.Hidden;
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
                            return 0;
                        }

                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        ret = (Dictionary<string, Account>) binaryFormatter.Deserialize(cryptoStream);
                    }
                }

                currentPassword = password;
                map = ret;
                return 1;
            }
            catch (SerializationException ex)
            {
                Console.WriteLine(ex.InnerException);
                return -1;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.InnerException);
                return -2;
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
