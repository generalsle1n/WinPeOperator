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
    string hostname= reg.getHostnameFromRegistry(alternativePath);
    Console.WriteLine(hostname);

}, getHostNamePathOption);

rootCommand.Add(getHostNameCommand);

//Drivecommand

rootCommand.Invoke(args);