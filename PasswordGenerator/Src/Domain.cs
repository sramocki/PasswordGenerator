using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator.Src
{
    public enum Type
    {
        Bank,
        Game,
        General,
        Forum,
        School,
        Shopping,
        Work
    }

    [Serializable]
    public class Domain
    {
        private string _address, _login, _password, _comment;
        private DateTime _timeUpdated;
        private Type _type;

        public Domain(string address, string login, string password, DateTime timeUpdated, string comment, Type type)
        {
            Address = address;
            Login = login;
            Password = password;
            TimeUpdated = timeUpdated;
            Comment = comment;
            Type = type;
        }

        public string Address
        {
            get => _address;
            set => _address = value;
        }

        public string Login
        {
            get => _login;
            set => _login = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string Comment
        {
            get => _comment;
            set => _comment = value;
        }

        public DateTime TimeUpdated
        {
            get => _timeUpdated;
            set => _timeUpdated = value;
        }

        public Type Type
        {
            get => _type;
            set => _type = value;
        }
    }
}