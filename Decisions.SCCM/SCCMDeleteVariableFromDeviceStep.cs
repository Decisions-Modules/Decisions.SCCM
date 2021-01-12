using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Delete Variable From Device", "SCCM Steps", "Variable")]
    //[AutoRegisterAgentFlowElementStep("Delete Variable From Device", "SCCM Steps", "Variable")]
    [Writable]
    public class SCCMDeleteVariableFromDeviceStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

            UInt32 resourceId = Convert.ToUInt32(data.Data["Resource Id"]);
            string variableName = data.Data["Variable Name"] as string;

            try
            {

                string query = string.Format("select * from SMS_MachineSettings where ResourceID = '{0}'", resourceId);

                IResultObject queryResult = connection.QueryProcessor.ExecuteQuery(query);

                //Create null instance of computerSettings. We will populate this later when iterating query result
                IResultObject computerSettings = null;

                //Extract one instance of SMS_MachineSettings out of the query result
                foreach (IResultObject cs in queryResult)
                {
                    computerSettings = cs;
                    break;
                }

                //Get this instance of MachineSettings
                computerSettings.Get();

                //Get MachineVariables from this instance of machine settings
                List<IResultObject> machineVariables = computerSettings.GetArrayItems("MachineVariables");

                //Iterate machine variables and remove variable if we find a match with the one to remove
                foreach (IResultObject machVar in machineVariables)
                {
                    if (machVar["Name"].StringValue == variableName)
                    {
                        //Since we found a match remove this variable from the collection of machine variables
                        machineVariables.Remove(machVar);

                        //Update this instance of SMS_MachineSettings with the new list of variables
                        computerSettings.SetArrayItems("MachineVariables", machineVariables);
                        computerSettings.Put();

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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Resource Id"),
            new DataDescription(new DecisionsNativeType(typeof(string)), "Variable Name")};
            }
        }
    }
}