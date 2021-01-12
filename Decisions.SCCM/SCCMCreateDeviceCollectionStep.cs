using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.Flow.Mapping.InputImpl;

namespace SCCM_2012
{
    [AutoRegisterStep("Create Device Collection", "SCCM Steps", "Collection")]
    //[AutoRegisterAgentFlowElementStep("Create Device Collection", "SCCM Steps", "Collection")]
    [Writable]
    public class SCCMCreateDeviceCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer, IDefaultInputMappingStep
    {
        const string INPUT_COLLECTION_NAME = "Collection Name";
        const string INPUT_COLLECTION_COMMENT = "Collection Comment";
        const string INPUT_LIMITING_COLLECTION_ID_OR_NAME = "Limiting Collection ID or Name";

        public override DecisionsFramework.Design.Flow.Mapping.OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id", false, true, false)})
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            string collectionName = data.Data[INPUT_COLLECTION_NAME] as string;
            string collectionComment = data.Data[INPUT_COLLECTION_COMMENT] as string;
            string limitingCollectionIdOrName = data.Data[INPUT_LIMITING_COLLECTION_ID_OR_NAME] as string;

            try
            {
                IResultObject oCollection = CreateCollectionBaseMethod(collectionName, collectionComment, 2, limitingCollectionIdOrName);

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
                    new DataDescription(new DecisionsNativeType(typeof(string)), INPUT_LIMITING_COLLECTION_ID_OR_NAME)};
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
