using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinPeOperator
{
    internal class PackageManager
    {
        private bool _alreadyInitialzed = false;
        private DismLogLevel _dismLogLevel = DismLogLevel.LogErrors;
        internal string WindowsPath { get; init; }
        private void InitClass()
        {
            if (!_alreadyInitialzed)
            {
                DismApi.Initialize(_dismLogLevel);
                _alreadyInitialzed = true;
            }
        }

        private void Dispose()
        {
            if (_alreadyInitialzed)
            {
                DismApi.Shutdown();
                _alreadyInitialzed = false;
            }
        }

        public void InstallSinglePackage(string PackagePath)
        {
            InitClass();

            using (DismSession session = DismApi.OpenOfflineSession(WindowsPath))
            {
                DismApi.AddPackage(session, PackagePath, false, false);
            }

            Dispose();
        }
    }
}
