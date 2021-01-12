using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionsFramework;
using DecisionsFramework.ServiceLayer;

namespace SCCM_2012
{
    public class SCCMInitializer : IModuleLicense
    {
        public void ValidateLicenseData(bool isPlatformOnTempLicense, ModuleLicenseDetail[] moduleLicenses)
        {
        }
    }
}
