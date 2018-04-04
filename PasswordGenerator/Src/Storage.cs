using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Storage
    {
        private List<Domain> domainList;
        public List<Domain> DomainList { get => domainList; set => domainList = value; }

        public Storage()
        {
            domainList = new List<Domain>();
            DomainList = domainList;
        }

    }
}
