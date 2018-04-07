using System;
using System.Collections.Generic;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Storage
    {
        public List<Domain> DomainList { get; set; }

        public Storage()
        {
            DomainList = new List<Domain>();
        }
    }
}
