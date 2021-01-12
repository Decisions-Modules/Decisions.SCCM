using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Delete Variable From Collection", "SCCM Steps", "Variable")]
    //[AutoRegisterAgentFlowElementStep("Delete Variable From Collection", "SCCM Steps", "Variable")]
    [Writable]
    public class SCCMDeleteVariableFromCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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
            string variableName = data.Data["Variable Name"] as string;

            try
            {

                string query = string.Format("select * from SMS_CollectionSettings where CollectionID = '{0}'", collectionId);

                IResultObject queryResult = connection.QueryProcessor.ExecuteQuery(query);

                //Create null instance of collectionSettings. We will populate this later when iterating query result
                IResultObject collectionSettings = null;

                //Extract one instance of SMS_CollectionSettings out of the query result
                foreach (IResultObject cs in queryResult)
                {
                    collectionSettings = cs;
                    break;
                }

                //Get this instance of CollectonSettings
                collectionSettings.Get();

                //Get CollectionVariables from this instance of collection settings
                List<IResultObject> collectionVariables = collectionSettings.GetArrayItems("CollectionVariables");

                //Iterate collection variables and remove variable if we find a match with the one to remove
                foreach (IResultObject collVar in collectionVariables)
                {
                    if (collVar["Name"].StringValue == variableName)
                    {
                        //Since we found a match remove this variable from the collection of machine variables
                        collectionVariables.Remove(collVar);

                        //Update this instance of SMS_CollectionSettings with the new list of variables
                        collectionSettings.SetArrayItems("CollectionVariables", collectionVariables);
                        collectionSettings.Put();

                        break;
                    }
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
                return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id"),
            new DataDescription(new DecisionsNativeType(typeof(string)), "Variable Name")};
            }
        }
    }
}