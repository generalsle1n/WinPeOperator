using WmiLight;

namespace WinPeOperator.Model
{
    internal class ComputerInfo
    {
        private const string _namespace = @"root\cimv2";
        private const string _wmiClass = "select * from Win32_ComputerSystemProduct";
        internal string SerialNumber;
        internal string SystemUUID;
        internal string Vendor;
        internal string Name;
        internal string Version;

        private static ComputerInfo InsertFromWMI(ComputerInfo Info)
        {

            using(WmiConnection Connection = new WmiConnection(_namespace))
            {
                foreach(WmiObject Result in Connection.CreateQuery(_wmiClass))
                {
                    Info.SerialNumber = Result.GetPropertyValue<string>("IdentifyingNumber") ?? "Serial not found";
                    Info.SystemUUID = Result.GetPropertyValue<string>("UUID") ?? "UUID not found";
                    Info.Vendor = Result.GetPropertyValue<string>("Vendor") ?? "Vendor not found";
                    Info.Name = Result.GetPropertyValue<string>("Name") ?? "Name not found";
                    Info.Version = Result.GetPropertyValue<string>("Version") ?? "Version not found";
                }
            }
            return Info;
        }

        internal static ComputerInfo CreateObject()
        {
            ComputerInfo Info = new ComputerInfo();
            Info = InsertFromWMI(Info);
            return Info;
        }
    }
}
