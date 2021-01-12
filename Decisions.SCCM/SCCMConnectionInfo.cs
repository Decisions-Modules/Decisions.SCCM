using DecisionsFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCCM_2012
{
    /// <summary>
    /// This class holds the SCCM module settings properties
    /// and can be used on the agent side.  
    /// You should never use SCCM module settings directly, but instead use this class.
    /// </summary>
    [DataContract]
    public partial class SCCMConnectionInfo
    {
        public static SCCMConnectionInfo SETTINGS;

        #region properties

        [DataMember]
        public string SCCMServerAddress
        {
            get;
            set;
        }

        [DataMember]
        public string SCCMServerUsername
        {
            get;
            set;
        }


        [DataMember]
        public string SCCMServerUserPassword
        {
            get;
            set;
        }

        #endregion

        internal static SCCMConnectionInfo GetSettings()
        {
            if (SETTINGS == null) {
                throw new BusinessRuleException("No SCCM settings could be found.  You may need to use a SetSCCMConnection step.");
            }
            return SETTINGS;
        }
    }
}
