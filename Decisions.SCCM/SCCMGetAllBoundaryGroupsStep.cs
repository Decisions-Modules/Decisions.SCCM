using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Boundary Groups", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Get All Boundary Groups", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMGetAllBoundaryGroupsStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMBoundaryGroup)), "All Boundary Groups", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "SELECT * FROM SMS_BoundaryGroup";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundaryGroup> boundaryGroups = new List<SCCMBoundaryGroup>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundaryGroup boundaryGroup = new SCCMBoundaryGroup(queryResult);

                    boundaryGroups.Add(boundaryGroup);
                }

                if (boundaryGroups == null || boundaryGroups.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Boundary Groups", boundaryGroups.ToArray());
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