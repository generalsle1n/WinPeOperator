using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinPeOperator
{
    internal class driveManager
    {
        public string getSystemDrive()
        {
            var lol = getAllLocalDrives();
            foreach(DriveInfo a in lol)
            {
                Console.WriteLine($"{a.Name}: " + hasWindowsFolder(a));
            }
            return "";
        }

        private DriveInfo[] getAllLocalDrives()
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
            bool folderExists = Directory.Exists($@"{info.Name}Windows\System32\");
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
