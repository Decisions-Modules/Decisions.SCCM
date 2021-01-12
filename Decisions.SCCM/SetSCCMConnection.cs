using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCCM_2012
{
    [AutoRegisterStep("Set SCCM Connection Info", "SCCM Steps")]
    //[AutoRegisterAgentFlowElementStep("Set SCCM Connection Info", "SCCM Steps")]
    [Writable]
    public class SetSCCMConnectionInfo : ISyncStep, IDataConsumer, IDataProducer
    {
        public OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done")
                };
            }
        }

        private static Log log = new Log("SCCM");

        public ResultData Run(StepStartData data)
        {
            SCCMConnectionInfo connectionData = data["SCCM Connection"] as SCCMConnectionInfo;

            if (SCCMConnectionInfo.SETTINGS == null) {
                SCCMConnectionInfo.SETTINGS = new SCCMConnectionInfo();
            }

            if (connectionData != null)
            {
                SCCMConnectionInfo.SETTINGS.SCCMServerAddress = connectionData.SCCMServerAddress;
                SCCMConnectionInfo.SETTINGS.SCCMServerUsername = connectionData.SCCMServerUsername;
                SCCMConnectionInfo.SETTINGS.SCCMServerUserPassword = connectionData.SCCMServerUserPassword;
            }
            else {
                log.LogWarn(null, "No SCCM Settings passed to Set SCCM Settings step, step ignored.");
            }


             return new ResultData("Done");
        
        }

        public DataDescription[] InputData
        {
            get {

                return new DataDescription[] {
                    new DataDescription(typeof(SCCMConnectionInfo), "SCCM Connection")
                };

            }
        }
    }
}
