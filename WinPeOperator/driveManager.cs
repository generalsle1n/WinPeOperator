using System.Diagnostics;
using System.Reflection;

namespace WinPeOperator
{
    internal class driveManager
    {
        private const string wmiNamespace = @"root\cimv2";
        private const string diskpartScriptName = "WinPeOperator.Resources.diskpartScript.txt";
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
            bool folderExists = File.Exists($@"{info.Name}Windows\System32\mmc.exe");
            if(folderExists == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string createTempDiskPartFile()
        {
            Assembly current = Assembly.GetExecutingAssembly();
            string tempFilePath = null;
            
            using (Stream rawScriptStream = current.GetManifestResourceStream(diskpartScriptName))
            {
                tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    rawScriptStream.CopyTo(fileStream);
                }
            }
            
            return tempFilePath;
        }

        public void wipeLocalDrives()
        {
            string currentPath = createTempDiskPartFile();

            Process diskpart = new Process()
            {
                StartInfo = new ProcessStartInfo("diskpart.exe")
                {
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = $"/s {currentPath}"
                }
            };

            diskpart.Start();
            diskpart.WaitForExit();
        }
    }
}
