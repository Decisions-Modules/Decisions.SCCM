using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Boundaries by Boundary Group Id", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Get Boundaries by Boundary Group Id", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMGetBoundariesByBoundaryGroupIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMBoundary)), "Boundaries", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            UInt32 boundaryGroupId = Convert.ToUInt32(data.Data["Boundary Group Id"]);


            try
            {
                String query = string.Format("SELECT * FROM SMS_BoundaryGroupMembers WHERE GroupID = {0}", boundaryGroupId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundary> boundaries = new List<SCCMBoundary>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundary boundary = new SCCMBoundary(GetBoundaryObjectById(Convert.ToUInt32(queryResult["BoundaryID"].IntegerValue)));

                    boundaries.Add(boundary);
                }

                if (boundaries == null || boundaries.Count == 0)
                {
                    return new ResultData("No Data"); 
                    
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Boundaries", boundaries.ToArray());
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Group Id") }; }
        }
    }
}