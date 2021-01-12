using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;

namespace SCCM_2012
{
    [AutoRegisterStep("Create Dynamic Collection for Devices", "SCCM Steps", "Collection")]
    //[AutoRegisterAgentFlowElementStep("Create Dynamic Collection for Devices", "SCCM Steps", "Collection")]
    [Writable]
    public class SCCMCreateDynamicCollectionForDevicesStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer, IDefaultInputMappingStep
    {
        const string INPUT_COLLECTION_NAME = "Collection Name";
        const string INPUT_COLLECTION_COMMENT = "Collection Comment";
        const string INPUT_LIMITING_COLLECTION_ID_OR_NAME = "Limiting Collection ID or Name";
        const string INPUT_RESOURCE_IDS = "Resource Ids";

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id",false, true, false)})
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string collectionName = data.Data[INPUT_COLLECTION_NAME] as string;
            string collectionComment = data.Data[INPUT_COLLECTION_COMMENT] as string;
            string limitingCollectionIdOrName = data.Data[INPUT_LIMITING_COLLECTION_ID_OR_NAME] as string;
            UInt32[] resourceIds = data.Data[INPUT_RESOURCE_IDS] as UInt32[];

            try
            {
                IResultObject oCollection = CreateCollectionBaseMethod(collectionName, collectionComment, 2, limitingCollectionIdOrName);

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

                Dictionary<string, object> resultData = new Dictionary<string, object>();
                resultData.Add("Collection Id", oCollection["CollectionID"].StringValue.ToString());
                return new ResultData("Done", resultData);
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
                    new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_COLLECTION_NAME),
                    new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_COLLECTION_COMMENT),
                    new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_LIMITING_COLLECTION_ID_OR_NAME),
                    new DataDescription(new DecisionsNativeType(typeof(UInt32)), INPUT_RESOURCE_IDS, true, true, false)};
            }
        }

        public IInputMapping[] DefaultInputs
        {
            get
            {
                return new IInputMapping[] { new ConstantInputMapping() { InputDataName = INPUT_LIMITING_COLLECTION_ID_OR_NAME, Value = "All Systems" } };
            }
        }
    }
}
