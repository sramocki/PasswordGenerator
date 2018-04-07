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
        private string address, login, password, comment;
        private DateTime timeUpdated;
        private Type type;

        public Domain(string address, string login, string password, DateTime timeUpdated, string comment, Type type)
        {
            Address = address;
            Login = login;
            Password = password;
            TimeUpdated = timeUpdated;
            Comment = comment;
            Type = type;
        }

        public string Address { get => address; set => address = value; }
        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
        public string Comment { get => comment; set => comment = value; }
        public DateTime TimeUpdated { get => timeUpdated; set => timeUpdated = value; }
        public Type Type { get => type; set => type = value; }
    }
}
