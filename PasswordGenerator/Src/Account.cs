using System;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Account
    {
        public Storage Storage { get; set; }

        public Account()
        {
            Storage = new Storage();
        }
    }
}
