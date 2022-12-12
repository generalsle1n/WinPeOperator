using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinPeOperator
{
    internal class VNCManager
    {
        private const string _vncServer = "winvnc.exe";
        private const string _vncConfig = "ultravnc.ini";
        private const string _wpeUtil = "wpeutil.exe";
        private const string _wpeUtilArguments = "DisableFirewall";

        internal void DisableFirewall()
        {
            Process _wpeUtilProc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _wpeUtil,
                    Arguments = _wpeUtilArguments,
                    UseShellExecute= true,
                    WindowStyle= ProcessWindowStyle.Hidden,
                }
            };

            _wpeUtilProc.Start();
            _wpeUtilProc.WaitForExit();
        }
    }
}
