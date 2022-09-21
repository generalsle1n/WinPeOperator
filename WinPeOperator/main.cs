using WinPeOperator;
using System.CommandLine;
using System.Dynamic;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

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
    string hostname = reg.getHostnameFromRegistry(alternativePath);
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

},tsEnvGetName);

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

rootCommand.Invoke(args);