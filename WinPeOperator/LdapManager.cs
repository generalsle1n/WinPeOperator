using LdapForNet;
using LdapForNet.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
        public string DeleteComputerObject(string ComputerName)
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

        public string GetComputerSID(string ComputerName)
        {
            string result = "";
            using (_ldap = new LdapConnection())
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

                    List<DirectoryAttribute> attributes = search.DirectoryAttributes.ToList();

                    byte[] rawSid = attributes.Find(attribute => attribute.Name.Equals("objectSid"))?.GetValue<byte[]>();

                    result = getObjectSIDString(rawSid);
                }
                catch
                {
                    result = "The computer cannot be found or not be deleted";
                }

            }
            return result;
        }

        private string getObjectSIDString(byte[] objectSid)
        {
            //S - 1 - 5 - 21 - 1200248873 - 372014781 - 619646970 - 4285
            //1 Means SID Version 1
            //5 Means AD
            //21 Means Domain Auth
            //The next thre parts are unique for each object
            //Random Int
            StringBuilder sidBuilder = new StringBuilder("S-1-");

            //Default = 5, five means ad
            byte authority = objectSid[1];

            // IdentifierAuthority (6 bytes starting from the second) (big endian)
            long identifierAuthority = 0;

            var offset = 2;
            var size = 6;
            int i;

            for (i = 0; i <= size - 1; i++)
            {
                identifierAuthority = identifierAuthority | System.Convert.ToInt64(objectSid[offset + i]) << 8 * (size - 1 - i);
            }

            sidBuilder.Append(identifierAuthority.ToString());

            // Iterate all the SubAuthority (little-endian)
            offset = 8;
            size = 4; // 32-bits (4 bytes) for each SubAuthority
            i = 0;

            while (i < authority)
            {
                long subAuthority = 0;

                for (var j = 0; j <= size - 1; j++)
                    // the below "Or" is a logical Or not a boolean operator
                    subAuthority = subAuthority | System.Convert.ToInt64(objectSid[offset + j]) << 8 * j;
                sidBuilder.Append("-").Append(subAuthority);
                i += 1;
                offset += size;
            }

            return sidBuilder.ToString();
        }
    }
}
