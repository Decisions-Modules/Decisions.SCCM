using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Collection by Id", "SCCM Steps", "Collection")]
    //[AutoRegisterAgentFlowElementStep("Get Collection by Id", "SCCM Steps", "Collection")]
    [Writable]
    public class SCCMGetCollectionByIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMCollection)), "Collection", false, true, false)}),
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
                IResultObject oCollection = GetCollectionObjectById(collectionId);

                if (oCollection == null)
                {
                    return new ResultData("No Data");
                }

                else
                {
                    SCCMCollection collection = new SCCMCollection(oCollection);

                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Collection", collection);
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
