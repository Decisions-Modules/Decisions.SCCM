using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Device Details By Device Id", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Get Device Details By Device Id", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMGetDeviceDetailsByDeviceIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSystem)), "Device Details", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            UInt32 resourceId = Convert.ToUInt32(data.Data["Device Id"]);

            try
            {
                IResultObject oResource = GetDeviceObjectById(resourceId);

                if (oResource == null)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    SCCMSystem resource = new SCCMSystem(oResource);
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Device Details", resource);
                    return new ResultData("Done", resultData);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Device Id") }; }
        }
    }
}
