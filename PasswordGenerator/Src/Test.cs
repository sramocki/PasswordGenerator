using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Test
    {
        private int number;
        public Test()
        {
            Number = 5;
        }

        public int Number { get => number; set => number = value; }
    }
}
