using Registry;
using Registry.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinPeOperator
{
    internal class registryManager
    {
        private string systemDrive;
        private const string registryPath = @"\Windows\System32\config\SYSTEM";
        private const string keyPath = @"ControlSet001\Control\ComputerName\ComputerNaame";
        public registryManager(string systemDrive)
        {
            //this.systemDrive = systemDrive;
            this.systemDrive = @$"{systemDrive}\temp\SYSTEM";
        }

        public string getHostnameFromRegistry()
        {
            string computerName = null;
            string fullPath = systemDrive + registryPath;

            try
            {
                RegistryHiveOnDemand hive = new RegistryHiveOnDemand(systemDrive);
                RegistryKey keys = hive.GetKey(keyPath);
                List<KeyValue> allValues = keys.Values;
                foreach (KeyValue single in allValues)
                {
                    if (single.ValueName.Equals("ComputerName"))
                    {
                        computerName = single.ValueData;
                    }
                }
            }
            catch (Exception e)
            {

            }
            

            return computerName;
        }
    }
}
