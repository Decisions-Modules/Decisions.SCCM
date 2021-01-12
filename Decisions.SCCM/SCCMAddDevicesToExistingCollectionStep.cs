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
    [AutoRegisterStep("Add Devices to Existing Collection", "SCCM Steps", "Collection")]
    //[AutoRegisterAgentFlowElementStep("Add Devices to Existing Collection", "SCCM Steps", "Collection")]
    [Writable]
    public class SCCMAddDevicesToExistingCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string collectionId = data.Data["Collection Id"] as string;
            UInt32[] resourceIds = data.Data["Resource Ids"] as UInt32[];
            IResultObject oCollection = GetCollectionObjectById(collectionId);

            try
            {
                foreach (UInt32 dId in resourceIds)
                {
                    IResultObject instDirectRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                    instDirectRule["ResourceClassName"].StringValue = "SMS_R_System";
                    instDirectRule["ResourceID"].IntegerValue = Convert.ToInt32(dId);
                    instDirectRule["RuleName"].StringValue = "DirectRuleForDevice";

                    Dictionary<string, object> methodParams = new Dictionary<string, object>();
                    methodParams.Add("collectionRule", instDirectRule);

                    oCollection.ExecuteMethod("AddMembershipRule", methodParams);

                }
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
                    new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id"),
                    new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Resource Ids", true, true, false)};
            }
        }
    }
}
