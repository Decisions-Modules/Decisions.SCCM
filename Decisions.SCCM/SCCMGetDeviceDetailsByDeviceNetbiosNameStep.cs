using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Device Details By Device NetBIOS Name", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Get Device Details By Device NetBIOS Name", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMGetDeviceDetailsByDeviceNetbiosNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

            string deviceNetbiosName = data.Data["NetBIOS Name"] as string;

            try
            {
                string queryFromWhere = string.Format("FROM SMS_R_System WHERE NetbiosName = '{0}'", deviceNetbiosName);
                string querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
                string querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

                if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
                {
                    throw new Exception(string.Format("More than one device was found by Net BIOS Name {0}", deviceNetbiosName));
                }

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(querySelectStar);

                List<SCCMSystem> resources = new List<SCCMSystem>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMSystem resource = new SCCMSystem(queryResult);
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Device Details", resource);
                    return new ResultData("Done", resultData);
                }
            }
            

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }

            return new ResultData("No Data");
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "NetBIOS Name") }; }
        }
    }
}
