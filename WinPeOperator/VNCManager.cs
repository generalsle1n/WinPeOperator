using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;

namespace WinPeOperator
{
    internal class VNCManager
    {
        private const string NameSpacePath = "WinPeOperator.Resources.Vnc.";
        private const string _vncServer = "winvnc.exe";
        private const string _vncConfig = "ultravnc.ini";
        private const string _wpeUtil = "wpeutil.exe";
        private const string _wpeUtilArguments = "DisableFirewall";

        internal string _smtpServer { get; init; }
        internal string _senderMail { get; init;}
        internal string _itMail { get; set; }

        private void SendMailToReceiver()
        {
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            string correctIP = "";
            foreach (IPAddress ip in localIPs)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    correctIP = ip.ToString();
                }
            }

            driveManager drive = new driveManager();
            string systemDrive = drive.getSystemDrive();
            registryManager reg = new registryManager(systemDrive);
            string hostname = reg.GetHostnameFromRegistry(null);

            string emailBody = $"<p>An VNC Server was started via Tasksequence with the follwing Data:</p>" +
                Environment.NewLine +
                $"<p>IP: {correctIP}</p>" +
                $"<p>Hostname(Not 100% Safe): {hostname}</p>";

            SmtpClient smtp = new SmtpClient()
            {
                Host = _smtpServer,
            };

            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_senderMail),
                Subject = "New VNCServer Start",
                IsBodyHtml = true,
                Body = emailBody
            };

            mail.To.Add(_itMail);
            smtp.Send(mail);
        }

        private void DisableFirewall()
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

        private string CopyFileToSystem(string EmbeddedFileName)
        {
            Assembly current = Assembly.GetExecutingAssembly();
            string tempFilePath = null;

            using (Stream rawFile = current.GetManifestResourceStream(NameSpacePath + EmbeddedFileName))
            {
                tempFilePath = Path.Combine(Path.GetTempPath(), EmbeddedFileName);
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    rawFile.CopyTo(fileStream);
                }
            }

            return tempFilePath;
        }

        internal void StartVNCServer()
        {
            DisableFirewall();
            string vncServerPath = CopyFileToSystem(_vncServer);
            string vncConfigPath = CopyFileToSystem(_vncConfig);

            Process vncserver = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = vncServerPath
                }
            };
            vncserver.Start();
            SendMailToReceiver();
        }
    }
}
