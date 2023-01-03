using Registry;
using Registry.Abstractions;

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

        private string GenerateRegistryPath(RegistryHive Hive, string alternativePath = "")
        {
            string registryEndPath;

            string hivePath = "";

            switch (Hive)
            {
                case RegistryHive.SYSTEM:
                    hivePath = _systemRegistryPath;
                    break;
                case RegistryHive.SOFTWARE:
                    hivePath = _softwareRegistryPath;
                    break;
            }

            if (alternativePath == null)
            {
                registryEndPath = _systemDrive + hivePath;
            }
            else
            {
                registryEndPath = alternativePath;
            }

            return registryEndPath;
        }

        private RegistryHiveOnDemand TryLoadRegistry(string Path)
        {
            RegistryHiveOnDemand hive = null;
            try
            {
                hive = new RegistryHiveOnDemand(Path);
            }
            catch (FileNotFoundException error)
            {
                Console.WriteLine($"The file is not found: {Path}");
                return null;
            }
            catch (IOException error)
            {
                Console.WriteLine($"There is an IO Error, this is common, when the file is already in use");
                return null;
            }

            return hive;
        }

        public string getHostnameFromRegistry(string alternativePath = "")
        {
            string computerName = null;
            string registryEndPath = GenerateRegistryPath(RegistryHive.SYSTEM ,alternativePath);
            RegistryHiveOnDemand hive = TryLoadRegistry(registryEndPath);
            if(hive != null)
            {
                RegistryKey keys = hive.GetKey(_nameKeyPath);

            if (keys == null)
            {
                Console.WriteLine("There are no Keys found");
            }
            else
            {
                    computerName = keys.Values.Find(reg => reg.ValueName.Equals("ComputerName")).ValueData;
                }
            }

            return computerName;
        }
    }
}
