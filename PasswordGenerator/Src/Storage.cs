using System;
using System.Collections.ObjectModel;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Storage
    {
        public ObservableCollection<Domain> DomainList { get; set; }

        public Storage()
        {
            DomainList = new ObservableCollection<Domain>();
        }
    }
}