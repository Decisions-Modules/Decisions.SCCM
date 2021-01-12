using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SCCM_2012
{
    [AutoRegisterStep("Refresh Collection Membership", "SCCM Steps", "Collection")]
    //[AutoRegisterAgentFlowElementStep("Refresh Collection Membership", "SCCM Steps", "Collection")]
    [Writable]
    public class SCCMRefreshCollectionMembershipStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { new OutcomeScenarioData("Done") };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string collectionId = data.Data["Collection Id"] as string;

            IResultObject oCollection = GetCollectionObjectById(collectionId);

            try
            {
                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                oCollection.ExecuteMethod("RequestRefresh", methodParams);
                return new ResultData("Done");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get
            {
                return new DataDescription[] { 
                    new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id") };
            }
        }
    }
}
