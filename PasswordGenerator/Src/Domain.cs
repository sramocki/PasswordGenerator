using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordGenerator.Src
{
    [Serializable]
    public class Domain
    {
        private string domainSite, siteLogin, sitePassword;
        public Domain(string domain, string login, string password)
        {
            DomainSite = domain;
            SiteLogin = login;
            SitePassword = password;
        }

        public string DomainSite { get => domainSite; set => domainSite = value; }
        public string SiteLogin { get => siteLogin; set => siteLogin = value; }
        public string SitePassword { get => sitePassword; set => sitePassword = value; }
    }
}
