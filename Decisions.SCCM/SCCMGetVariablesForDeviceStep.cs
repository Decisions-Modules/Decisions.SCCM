using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Variables for Device", "SCCM Steps", "Variable")]
    //[AutoRegisterAgentFlowElementStep("Get Variables for Device", "SCCM Steps", "Variable")]
    [Writable]
    public class SCCMGetVariablesForDeviceStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMVariable)), "Device Variables", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            UInt32 resourceId = Convert.ToUInt32(data.Data["Resource Id"]);
            

            try
            {
                //Commenting out because this approach does not work when run against a newly created device. 
                //It takes some time for the device membership to update, so we need to get the site code directly from the device.
                /*
                String queryGetCollectionMemberByName = "select SiteCode from SMS_FullCollectionMembership  WHERE ResourceId = '" + resourceId + "'";

                IResultObject queryResults1 = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                String SourceSite = "";

                foreach (IResultObject queryResult1 in queryResults1)
                    SourceSite = queryResult1["SiteCode"].StringValue.ToString();
                 */

                //Creating empty source site. Will try setting value just below.
                string SourceSite = "";

                SCCMSystem deviceDetails = new SCCMSystem(GetDeviceObjectById(resourceId));

                try
                {                    
                    SourceSite = deviceDetails.SMSAssignedSites[0];
                }

                catch
                {
                    throw new ArgumentNullException("Unable to find site code for this device. Without site code variables cannot be fetched.");
                }

                IResultObject computerSettings = connection.CreateInstance("SMS_MachineSettings");
                computerSettings["ResourceID"].IntegerValue = Convert.ToInt32(resourceId);
                computerSettings["SourceSite"].StringValue = SourceSite;
                computerSettings["LocaleID"].IntegerValue = 1033; // The default locale ID is 1033, English (United States).

                computerSettings.Put();

                computerSettings.Get();

                List<IResultObject> collectionVariables = computerSettings.GetArrayItems("MachineVariables");

                computerSettings.Get();

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
                    resultData.Add("Device Variables", variables.ToArray());
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Resource Id") }; }
        }
    }
}