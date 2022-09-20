using WinPeOperator;
using System.CommandLine;
using System.Dynamic;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;

RootCommand rootCommand = new RootCommand();
Command getHostNameCommand = new Command(name: "--GetHostName", description: "Get Hostname from Offline Registry");
Option<string> getHostNamePathOption = new Option<string>(name: "--RegistryPath", description: "Set an alternative Path to the SYSTEM File");

getHostNameCommand.Add(getHostNamePathOption);

getHostNameCommand.SetHandler((alternativePath) =>
{
    registryManager reg = new registryManager("C:");
    string hostname= reg.getHostnameFromRegistry(alternativePath);
    Console.WriteLine(hostname);

}, getHostNamePathOption);

rootCommand.Add(getHostNameCommand);

rootCommand.Invoke(args);