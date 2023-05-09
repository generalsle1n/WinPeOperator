using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Diagnostics;
using System.Net.Mail;
using System.Reflection;
using WinPeOperator.Model;

namespace WinPeOperator
{
    internal class driveManager
    {
        private const string wmiNamespace = @"root\cimv2";
        private const string diskpartScriptName = "WinPeOperator.Resources.diskpartScript.txt";
        private const string wehrleLogo = "WinPeOperator.Resources.wehrleLogo.png";
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

        public void CreateCertificate(string HostName, string SmtpServer, string ToAddress, string FromMail)
        {
            Stream Logo = Assembly.GetExecutingAssembly().GetManifestResourceStream(wehrleLogo);
           
            ComputerInfo Info = ComputerInfo.CreateObject();
            Document PDFFile = Document.Create(pdf =>
            {
                pdf.Page(CurrentPage =>
                {
                    CurrentPage.Margin(50);

                    CurrentPage.Header().Row(CurrentRow =>
                    {
                        CurrentRow.RelativeItem().Column(CurrentColumn =>
                        {
                            CurrentColumn
                                .Item()
                                .Text($"Zertifikat für {HostName}")
                                .FontSize(20)
                                .SemiBold()
                                .FontColor(Colors.Cyan.Medium);
                            CurrentColumn
                                .Item()
                                .Text(text =>
                                {
                                    text.Span("Austellungs Date: ").SemiBold();
                                    text.Span($"{(DateTime.Now).ToString():d}");
                                });
                        });
                        CurrentRow
                            .ConstantItem(200)
                            .Height(50)
                            .Image(Logo);
                    });
                    
                    CurrentPage.Content()
                        
                        .AlignCenter()
                        .AlignLeft()
                        .Text(text =>
                        {
                            int count = 0;
                            while (count < 10)
                            {
                                text.EmptyLine();
                                count++;
                            }

                            text.Span("Hiermit wird bestätigt das dass oben stehende Gerät sicher und unwiederuflich gelöscht wurde" + Environment.NewLine);
                            text.Span("Die Zerstörung der Daten ist nur elektronisch erfolgt, die Hardware kann sofern benötigt weiterverwendet werden" + Environment.NewLine);

                            for(int i = 0; i < 5; i++)
                            {
                                text.EmptyLine();
                            }

                            text.Span($"Seriennummer: {Info.SerialNumber}{Environment.NewLine}");
                            text.Span($"Model {Info.Name}{Environment.NewLine}");
                            text.Span($"Version: {Info.Version}{Environment.NewLine}");
                            text.Span($"UUID: {Info.SystemUUID}{Environment.NewLine}");
                            text.Span($"Vendor: {Info.Vendor}{Environment.NewLine}");
                        });


                    CurrentPage.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Dieses Dokument wurde digital erstellt und ist ohne Unterschrift gültig.");
                        text.EmptyLine();
                        text.CurrentPageNumber();
                        text.Span(" - ");
                        text.TotalPages();
                    });
                });
            });
            SendMailToIT(PDFFile.GeneratePdf(), SmtpServer, ToAddress);
        }

        private void SendMailToIT(byte[] Attachment, string smtpserver, string toMail)
        {
            using(SmtpClient Client = new SmtpClient(smtpserver, 25))
            {
                MailAddressCollection Destination = new MailAddressCollection();
                Destination.Add(new MailAddress(toMail));
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress("wipe@wehrle-werk.internal"),
                    Subject = "Wipe Certificate"
                };
                message.To.Add(new MailAddress(toMail));
                message.Attachments.Add(new Attachment(new MemoryStream(Attachment), "Certificate.pdf"));
                Client.Send(message);
            }
        }
    }
}
