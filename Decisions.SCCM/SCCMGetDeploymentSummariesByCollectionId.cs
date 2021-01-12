using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Deployment Summaries by Collection Id", "SCCM Steps")]
    //[AutoRegisterAgentFlowElementStep("Get Deployment Summaries by Collection Id", "SCCM Steps")]
    [Writable]
    public class SCCMGetDeploymentSummariesByCollectionId : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMDeploymentSummary)), "Deployment Summaries", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string collectionId = data.Data["Collection Id"] as string;

            try
            {
                String queryGetDeployementSummariesByCollectionId = string.Format("select * from SMS_DeploymentSummary WHERE CollectionID = '{0}'", collectionId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(queryGetDeployementSummariesByCollectionId);

                List<SCCMDeploymentSummary> deploymentSummaries = new List<SCCMDeploymentSummary>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMDeploymentSummary depSum = new SCCMDeploymentSummary(queryResult);
                    deploymentSummaries.Add(depSum);
                }

                if (deploymentSummaries == null || deploymentSummaries.Count == 0)
                {
                    return new ResultData("No Data");

                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Deployment Summaries", deploymentSummaries.ToArray());
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id") }; }
        }
    }
}
