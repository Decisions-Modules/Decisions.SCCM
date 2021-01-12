using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Deployment Summaries", "SCCM Steps")]
    //[AutoRegisterAgentFlowElementStep("Get All Deployment Summaries", "SCCM Steps")]
    [Writable]
    public class SCCMGetAllDeploymentSummaries : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMDeploymentSummary)), "All Deployment Summaries", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = "select * from SMS_DeploymentSummary";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMDeploymentSummary> deploymentSummaries = new List<SCCMDeploymentSummary>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMDeploymentSummary depSummary = new SCCMDeploymentSummary(queryResult);

                    deploymentSummaries.Add(depSummary);
                }

                if (deploymentSummaries == null || deploymentSummaries.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Deployment Summaries", deploymentSummaries.ToArray());
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

