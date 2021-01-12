using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Set Variable for Device", "SCCM Steps", "Variable")]
    //[AutoRegisterAgentFlowElementStep("Set Variable for Device", "SCCM Steps", "Variable")]
    [Writable]
    public class SCCMSetVariableForDeviceStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

            UInt32 resourceId = Convert.ToUInt32(data.Data["Device Id"]);
            SCCMVariable variable = data.Data["Variable"] as SCCMVariable;

            try
            {
                #region may be able to put this in a method so that get and set device variable methods can share this code
                //Commenting out because this approach does not work when run against a newly created device. 
                //It takes some time for the device membership to update, so we need to get the site code directly from the device.
                /*
                /String queryGetCollectionMemberByName = "select SiteCode from SMS_FullCollectionMembership  WHERE ResourceId = '" + resourceId + "'";

                IResultObject queryResults1 = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                String SourceSite = "";

                foreach (IResultObject queryResult1 in queryResults1)
                    SourceSite = queryResult1["SiteCode"].StringValue.ToString();
                */

                //Creating empty source site. Will try setting value just below.
                string SourceSite = "";

                try
                {
                    SCCMSystem deviceDetails = new SCCMSystem(GetDeviceObjectById(resourceId));
                    SourceSite = deviceDetails.SMSAssignedSites[0];
                }
                catch
                {
                    throw new ArgumentNullException("Unable to find site code for this device. Without site code variables cannot be added.");
                }

                IResultObject computerSettings = connection.CreateInstance("SMS_MachineSettings");
                computerSettings["ResourceID"].IntegerValue = Convert.ToInt32(resourceId);
                computerSettings["SourceSite"].StringValue = SourceSite;
                computerSettings["LocaleID"].IntegerValue = 1033; // The default locale ID is 1033, English (United States).

                computerSettings.Put();
                computerSettings.Get();

                List<IResultObject> collectionVariables = computerSettings.GetArrayItems("MachineVariables");

                computerSettings.Get();
                #endregion

                IResultObject collectionVariable = connection.CreateEmbeddedObjectInstance("SMS_MachineVariable");
                collectionVariable["Name"].StringValue = variable.Name;
                collectionVariable["Value"].StringValue = variable.Value;
                collectionVariable["IsMasked"].BooleanValue = variable.IsMasked;

                collectionVariables.Add(collectionVariable);

                computerSettings.SetArrayItems("MachineVariables", collectionVariables);

                computerSettings.Put();

                connection.Close();
                connection.Dispose();

                return new ResultData("Done");

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Device Id"),
            new DataDescription(new DecisionsNativeType(typeof(SCCMVariable)), "Variable")};
            }
        }
    }
}