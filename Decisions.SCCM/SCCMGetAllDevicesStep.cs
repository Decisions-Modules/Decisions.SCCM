using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Devices", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Get All Devices", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMGetAllDevicesStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSystem)), "All Devices", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = "select * from SMS_R_System";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMSystem> resources = new List<SCCMSystem>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMSystem resource = new SCCMSystem(queryResult);

                    resources.Add(resource);
                }

                if (resources == null || resources.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Devices", resources.ToArray());
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
            get { return null; }
        }
    }
}

