using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Collections", "SCCM Steps", "Collection")]
    //[AutoRegisterAgentFlowElementStep("Get All Collections", "SCCM Steps", "Collection")]
    [Writable]
    public class SCCMGetAllCollectionsStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMCollection)), "All Collections", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select CollectionID,Name,LocalMemberCount from SMS_Collection ";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMCollection> collections = new List<SCCMCollection>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMCollection collection = new SCCMCollection(queryResult);

                    collections.Add(collection);

                }

                if (collections == null || collections.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Collections", collections.ToArray());
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
