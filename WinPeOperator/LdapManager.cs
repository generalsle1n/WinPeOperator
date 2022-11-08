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

        public bool deleteComputerObject(string ComputerName)
        {
            bool success = true;
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
                    List<LdapEntry> result = _ldap.SearchByCn(ComputerName).ToList();
                    _ldap.Delete(result[0].Dn);
                }
                catch
                {
                    success = false;
                }
                
            }
            return success;
        }
    }
}
