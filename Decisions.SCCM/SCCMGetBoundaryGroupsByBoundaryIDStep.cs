using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Boundary Groups by Boundary Id", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Get Boundary Groups by Boundary Id", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMGetBoundaryGroupsByBoundaryIDStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMBoundaryGroup)), "Boundary Groups", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            UInt32 boundaryId = Convert.ToUInt32(data.Data["Boundary Id"]);

            try
            {
                String query = string.Format("SELECT * FROM SMS_BoundaryGroupMembers WHERE BoundaryID = {0}", boundaryId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundaryGroup> boundaryGroups = new List<SCCMBoundaryGroup>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundaryGroup boundaryGroup = new SCCMBoundaryGroup(GetBoundaryGroupObjectById(Convert.ToUInt32(queryResult["GroupID"].IntegerValue)));
                    boundaryGroups.Add(boundaryGroup);
                }

                if (boundaryGroups == null || boundaryGroups.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Boundary Groups", boundaryGroups.ToArray());
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Id") }; }
        }
    }
}