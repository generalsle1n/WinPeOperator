using WinPeOperator;

registryManager a = new registryManager("C:");
Console.WriteLine(a.getHostnameFromRegistry());

driveManager b = new driveManager();
b.getSystemDrive();