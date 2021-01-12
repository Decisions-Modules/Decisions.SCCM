using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Devices By Collection Name", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Get Devices By Collection Name", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMGetDevicesByCollectionNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSystem)), "Collection Items", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string collectionName = data.Data["Collection Name"] as string;
                String queryGetCollection = string.Format("select CollectionID,Name,LocalMemberCount from SMS_Collection where Name = '{0}'", collectionName);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(queryGetCollection);

                foreach (IResultObject queryResult in queryResults)
                {
                    if (queryResult["LocalMemberCount"].IntegerValue > 0)
                    {
                        string collectionId = queryResult["CollectionID"].StringValue;
                        SCCMSystem[] systemsInCollection = GetSCCMSystemsByCollectionIdBaseMethod(collectionId);

                        if (systemsInCollection == null || systemsInCollection.Length == 0)
                        {
                            return new ResultData("No Data");
                        }

                        else
                        {
                            Dictionary<string, object> resultData = new Dictionary<string, object>();
                            resultData.Add("Collection Items", systemsInCollection);
                            return new ResultData("Done", resultData);
                        }
                    }
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Name") }; }
        }
    }
}
