using WinPeOperator;
using System.CommandLine;

RootCommand rootCommand = new RootCommand();

//Hostname Command
Command getHostNameCommand = new Command(name: "--GetHostName", description: "Get Hostname from Offline Registry");
Option<string> getHostNamePathOption = new Option<string>(name: "--RegistryPath", description: "Set an alternative Path to the SYSTEM File");

getHostNameCommand.Add(getHostNamePathOption);

getHostNameCommand.SetHandler((alternativePath) =>
{
    driveManager drive = new driveManager();
    string systemDrive = drive.getSystemDrive();
    registryManager reg = new registryManager(systemDrive);
    string hostname = reg.GetHostnameFromRegistry(alternativePath);
    Console.WriteLine(hostname);
}, getHostNamePathOption);

rootCommand.Add(getHostNameCommand);

//WipeCommand
Command wipeDiskCommand = new Command(name: "--WipeDisk", description: "Wipes all local Disks");

wipeDiskCommand.SetHandler(() =>
{
    driveManager drive = new driveManager();
    drive.wipeLocalDrives();
});

rootCommand.Add(wipeDiskCommand);

//TSEnvGetVariable
Command tsEnvGet = new Command(name: "--GetTSVar", description: "Get Task Sequence Variable Data");
Option<string> tsEnvGetName = new Option<string>(name: "--Name", description: "Enter the variable Name")
{
    IsRequired = true,
};


tsEnvGet.Add(tsEnvGetName);

tsEnvGet.SetHandler((variable) =>
{
    tsManager ts = new tsManager();
    string value = ts.getTSVariableData(variable);
    Console.WriteLine(value);

}, tsEnvGetName);

rootCommand.Add(tsEnvGet);

//TSEnvVariableSet

Command tsEnvSet = new Command(name: "--SetTSVar", description: "Set Task Sequence Variable Data");
Option<string> tsEnvSetName = new Option<string>(name: "--Name", description: "Enter the variable Name")
{
    IsRequired = true,
};
Option<string> tsEnvSetNameValue = new Option<string>(name: "--Value", description: "Enter the variable Value")
{
    IsRequired = true,
};


tsEnvSet.Add(tsEnvSetName);
tsEnvSet.Add(tsEnvSetNameValue);
tsEnvSet.SetHandler((variableName, variableValue) =>
{
    tsManager ts = new tsManager();
    ts.setTSVariableData(variableName, variableValue);
}, tsEnvSetName, tsEnvSetNameValue);

rootCommand.Add(tsEnvSet);

//LdapHandler
Command LdapDelete = new Command(name: "--DeleteLdapComputer", description: "Delete Ldap Computer Object");
Option<string> LdapDeleteDomain = new Option<string>(name: "--Domain", description: "Enter the Domain Name")
{
    IsRequired = true,
};
Option<string> LdapDeletePort = new Option<string>(name: "--Port", description: "Enter the Network Port")
{
    IsRequired = true,
};
Option<string> LdapDeleteUserName = new Option<string>(name: "--User", description: "Enter the User Name")
{
    IsRequired = true,
};
Option<string> LdapDeleteUserPassword = new Option<string>(name: "--Password", description: "Enter the Password Name")
{
    IsRequired = true,
};
Option<string> LdapDeleteSearchbase = new Option<string>(name: "--SearchBase", description: "Enter the Ldap Searchbase")
{
    IsRequired = true,
};
Option<bool> LdapForceDelete= new Option<bool>(name: "--Force", description: "Force to delete AD Object when SID are not Equal")
{
    IsRequired = false,
};
LdapDelete.Add(LdapDeleteDomain);
LdapDelete.Add(LdapDeletePort);
LdapDelete.Add(LdapDeleteUserName);
LdapDelete.Add(LdapDeleteUserPassword);
LdapDelete.Add(LdapDeleteSearchbase);
LdapDelete.Add(LdapForceDelete);

LdapDelete.SetHandler((variableDomain, variablePort, variableUser, variablePassword, varibaleSearchBase, variableForceDelete) =>
{
    driveManager drive = new driveManager();
    string systemDrive = drive.getSystemDrive();
    registryManager reg = new registryManager(systemDrive);
    string hostname = reg.GetHostnameFromRegistry(null);

    LdapManager Manager = new LdapManager()
    {
        DomainName = variableDomain,
        Port = int.Parse(variablePort),
        UserName = variableUser,
        Password = variablePassword,
        Searchbase = varibaleSearchBase
    };

    string computerSID = Manager.GetComputerSID(hostname);

    if (reg.CheckIfADAndComputerSIDAreSame(computerSID) || variableForceDelete)
    {
        Console.WriteLine(Manager.DeleteComputerObject(hostname));
    }

}, LdapDeleteDomain, LdapDeletePort, LdapDeleteUserName, LdapDeleteUserPassword, LdapDeleteSearchbase, LdapForceDelete);
rootCommand.Add(LdapDelete);

//VNCServer
Command vncServerStart = new Command(name: "--StartVNCServer", description: "Start an local VNC Server");
Option<string> VNCSmtpServer = new Option<string>(name: "--SMTPServer", description: "Enter the Smtp Server Name")
{
    IsRequired = true,
};
Option<string> VNCMailDestinatation= new Option<string>(name: "--DestinationMail", description: "Enter the Mail where the status mail should be send")
{
    IsRequired = true,
};
Option<string> VNCMailSource = new Option<string>(name: "--SourceMail", description: "Enter the Mail which should be used to send")
{
    IsRequired = true,
};

vncServerStart.AddOption(VNCSmtpServer);
vncServerStart.AddOption(VNCMailDestinatation);
vncServerStart.AddOption(VNCMailSource);
vncServerStart.SetHandler((smtpserver, maildestination, mailsource) =>
{
    VNCManager _vnc = new VNCManager()
{
        _itMail = maildestination,
        _senderMail = mailsource,
        _smtpServer = smtpserver
    };
    _vnc.StartVNCServer();
}, VNCSmtpServer, VNCMailDestinatation, VNCMailSource);

rootCommand.Add(vncServerStart);

//Install Packages
Command PackageInstall = new Command(name: "--InstallPackage", description: "Install .CAB/.MSU to Offline Windows");
Option<string> SourcePath = new Option<string>(name: "--PackagePath", description: "Enter the Path to the Package")
{
    IsRequired = true
};
Option<string> WindowsOffline = new Option<string>(name: "--WindowsPath", description: @"Enter the WindowsPath. eg. C: instead of C:\, when not specified, it gets read from task sequence")
{
    IsRequired = false
};


PackageInstall.Add(WindowsOffline);
PackageInstall.Add(SourcePath);

PackageInstall.SetHandler((variableWindowsOfflinePath, variableSourcePath) =>
{
    if(variableWindowsOfflinePath == null)
    {
        tsManager ts = new tsManager();
        variableWindowsOfflinePath = ts.getTSVariableData("OSDisk");
    }

    PackageManager pkg = new PackageManager()
    {
        WindowsPath = variableWindowsOfflinePath
    };
    pkg.InstallSinglePackage(variableSourcePath);

}, WindowsOffline, SourcePath);

rootCommand.Add(PackageInstall);

rootCommand.Invoke(args);