using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace WinPeOperator
{
    internal class driveManager
    {
        private const string wmiNamespace = @"root\cimv2";
        public string getSystemDrive()
        {
            string systemDrive = null;
            DriveInfo[] allVolumes = getAllLocalVolumes();
            foreach(DriveInfo singleVolume in allVolumes)
            {
                if (hasWindowsFolder(singleVolume))
                {
                    systemDrive = singleVolume.Name;
                    break;
                }
            }
            return systemDrive;
        }

        private DriveInfo[] getAllLocalVolumes()
        {
            List<DriveInfo> allLocalDrives = new List<DriveInfo>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach(DriveInfo drive in allDrives)
            {
                if(drive.DriveType == DriveType.Fixed)
                {
                    allLocalDrives.Add(drive);
                }
            }

            return allLocalDrives.ToArray<DriveInfo>();
        }

        private bool hasWindowsFolder(DriveInfo info)
        {
            bool folderExists = File.Exists($@"{info.Name}Windows\System32\ntoskrnl.exe");
            if(folderExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
