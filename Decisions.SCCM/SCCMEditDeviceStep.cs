using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace SCCM_2012
{
    [AutoRegisterStep("Edit Device", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Edit Device", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMEditDeviceStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override DecisionsFramework.Design.Flow.Mapping.OutcomeScenarioData[] OutcomeScenarios
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
            string[] updatedMACAddress = data.Data["Updated MAC Addresses"] as string[];
            UInt32 resourceId = Convert.ToUInt32(data.Data["Device Id"]);
            try
            {

                String query = string.Format("select * from SMS_R_System where ResourceID = '{0}'", resourceId);
                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject queryResult in queryResults)
                {
                    queryResult["MACAddresses"].StringValue = updatedMACAddress[0];
                    queryResult.Put();
                    break;                
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
                    new DataDescription(new DecisionsNativeType(typeof(string)), "Updated MAC Addresses", true, false, false),
                    new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Device Id")};
            }
        }
    }
}
