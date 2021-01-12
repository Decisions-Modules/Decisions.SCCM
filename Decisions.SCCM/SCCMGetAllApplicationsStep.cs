using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Applications", "SCCM Steps", "Application")]
    //[k[==AutoRegisterAgentFlowElementStep("Get All Applications", "SCCM Steps", "Application")]
    [Writable]
    public class SCCMGetAllApplicationsStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMApplication)), "All Applications", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "SELECT * FROM SMS_Application WHERE IsLatest = 1";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMApplication> applications = new List<SCCMApplication>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMApplication application = new SCCMApplication(queryResult);

                    applications.Add(application);
                }

                if (applications == null || applications.Count == 0)
                {
                    return new ResultData("No Data");
                }

                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Applications", applications.ToArray());
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
