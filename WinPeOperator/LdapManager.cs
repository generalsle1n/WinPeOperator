using LdapForNet;
using LdapForNet.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinPeOperator
{
    internal class LdapManager
    {
        public string DomainName { get; init; }
        public int Port { get; init; }
        public string UserName { get; init; }
        public string Password { get; init; }
        public string Searchbase { get; init; }
        private LdapConnection _ldap;
        public string deleteComputerObject(string ComputerName)
        {
            string result = "Success";
            using(_ldap = new LdapConnection())
            {
                try
                {
                    LdapCredential _cred = new LdapCredential()
                    {
                        UserName = UserName,
                        Password = Password,
                    };
                    _ldap.Connect(DomainName, Port);
                    _ldap.Bind(Native.LdapAuthType.Simple, _cred);
                    LdapEntry search = _ldap.Search(Searchbase, $"(&(objectCategory=computer)(name={ComputerName}))")[0];
                    _ldap.Delete(search.Dn);
                }
                catch
                {
                    result = "The computer cannot be found or not be deleted";
                }
                
            }
            return result;
        }
    }
}
