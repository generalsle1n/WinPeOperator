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
        private string _systemDrive;
        private const string _systemRegistryPath = @"Windows\System32\config\SYSTEM";
        private const string _softwareRegistryPath = @"Windows\System32\config\SOFTWARE";
        private const string _nameKeyPath = @"ControlSet001\Control\ComputerName\ComputerName";
        private const string _sidKeyPath = @"Microsoft\Windows\CurrentVersion\Group Policy\GroupMembership";
        public registryManager(string systemDrive)
        {
            _systemDrive = systemDrive;
        }

        public string getHostnameFromRegistry(string alternativePath = "")
        {
            RegistryHiveOnDemand hive = null;
            List<KeyValue> allValues = null;
            string computerName = null;
            string registryEndPath;
            if (alternativePath == null)
            {
                registryEndPath = _systemDrive + hivePath;
            }
            else
            {
                registryEndPath = alternativePath;
            }

            try
            {
                hive = new RegistryHiveOnDemand(registryEndPath);
            }
            catch (FileNotFoundException error)
            {
                Console.WriteLine($"The file is not found: {registryEndPath}");
                return null;
            }
            catch (IOException error)
            {
                Console.WriteLine($"There is an IO Error, this is common, when the file is already in use");
                return null;
            }

            RegistryKey keys = hive.GetKey(keyPath);

            if (keys == null)
            {
                Console.WriteLine("There are no Keys found");
            }
            else
            {
                allValues = keys.Values.ToList<KeyValue>();
            }
            foreach (KeyValue single in allValues)
            {
                if (single.ValueName.Equals("ComputerName"))
                {
                    computerName = single.ValueData;
                    break;
                }
            }

            return computerName;
        }
    }
}
