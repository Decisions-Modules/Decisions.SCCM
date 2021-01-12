using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Variables for Collection", "SCCM Steps", "Variable")]
    //[AutoRegisterAgentFlowElementStep("Get Variables for Collection", "SCCM Steps", "Variable")]
    [Writable]
    public class SCCMGetVariablesForCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMVariable)), "Collection Variables", true, true, false)}),
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
                
                #region may be able to break this out into a method so that get and set collection vars can share this code
                IResultObject collectionSettings = connection.CreateInstance("SMS_CollectionSettings");
                collectionSettings["CollectionID"].StringValue = collectionId;
                collectionSettings.Put();

                collectionSettings.Get();

                List<IResultObject> collectionVariables = collectionSettings.GetArrayItems("CollectionVariables");

                collectionSettings.Get();
                #endregion

                List<SCCMVariable> variables = new List<SCCMVariable>();

                foreach (IResultObject item in collectionVariables)
                {
                    if (item != null)
                    {
                        SCCMVariable variable = new SCCMVariable(item);

                        variables.Add(variable);
                    }
                }
                if (variables == null || variables.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Collection Variables", variables.ToArray());
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