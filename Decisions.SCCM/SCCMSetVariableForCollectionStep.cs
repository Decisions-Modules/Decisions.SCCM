using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Set Variable for Collection", "SCCM Steps", "Variable")]
    //[AutoRegisterAgentFlowElementStep("Set Variable for Collection", "SCCM Steps", "Variable")]
    [Writable]
    public class SCCMSetVariableForCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string collectionId = data.Data["Collection Id"] as string;
            SCCMVariable variable = data.Data["Variable"] as SCCMVariable;

            try
            {

                IResultObject collectionSettings = connection.CreateInstance("SMS_CollectionSettings");
                collectionSettings["CollectionID"].StringValue = collectionId;
                collectionSettings.Put();

                collectionSettings.Get();

                List<IResultObject> collectionVariables = collectionSettings.GetArrayItems("CollectionVariables");

                IResultObject collectionVariable = connection.CreateEmbeddedObjectInstance("SMS_CollectionVariable");
                collectionVariable["Name"].StringValue = variable.Name;
                collectionVariable["Value"].StringValue = variable.Value;
                collectionVariable["IsMasked"].BooleanValue = variable.IsMasked;


                collectionVariables.Add(collectionVariable);
                collectionSettings.SetArrayItems("CollectionVariables", collectionVariables);

                collectionSettings.Put();

                return new ResultData("Done");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }

        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id"),
            new DataDescription(new DecisionsNativeType(typeof(SCCMVariable)), "Variable")};
            }
        }
    }
}