using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Account
    {
        private static Dictionary<string, Account> map = new Dictionary<string, Account>();
        private static string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Ramocki\passgen.dat");
        private static FileInfo currentFile = new FileInfo(fileName);

        private string username, password;
        public Storage Storage { get; set; }
        public Test Test { get; set; }
        public string Username { get => username; set => username = value; }

        public Account(string username, string password)
        {
            this.username = username;
            this.password = password;
            Storage storage2 = new Storage();
            Storage = storage2;
            Test test = new Test();
            Test = test;
        }

        public static bool CreateAccount(string passedUser, string passedPassword)
        {
            if(map.ContainsKey(passedUser))
            {
                return false;
            }
            Account temp = new Account(passedUser, passedPassword);
            map.Add(passedUser, temp);
            Save();
            return true;
        }

        public static bool ValidLogin(string username, string password)
        {
            if (map.ContainsKey(username))
            {
                map.TryGetValue(username, out Account tempAccount);
                if (tempAccount.password.Equals(password))
                {
                    return true;
                }

            }
            return false;
        }

        public static bool Exists(string username)
        {
            if(map.ContainsKey(username))
            {
                return true;
            }
            return false;
        }

        public static void Save()
        {
            try
            {
                using (Stream fstream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fstream, map);
                    Console.WriteLine("Saved");
                }
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public static void Load()
        {
            Dictionary<string, Account> ret;
            try
            {
                using (Stream fstream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
                {
                    if(fstream.Length.Equals(0))
                    {
                        return;
                    }
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    ret = (Dictionary<string, Account>)binaryFormatter.Deserialize(fstream);
                }
                map = ret;   
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public static Account GetAccount(string disusername)
        {
            Account value;
            if (map.TryGetValue(disusername, out value))
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
