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
        private void InitClass()
        {
            if (!_alreadyInitialzed)
            {
                DismApi.Initialize(_dismLogLevel);
                _alreadyInitialzed = true;
            }
        }
        public void InstallDotNetThree()
        {
            if
            DismApi.Initialize(DismLogLevel.LogErrors);
        }
    }
}
