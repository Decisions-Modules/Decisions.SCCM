using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Create Device", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Create Device", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMCreateDeviceStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Device Id", false, true, false)})
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string netbiosName = data.Data["NetBIOS Name"] as string;
            string macAddress = data.Data["MAC Address"] as string;

            try
            {
                string smBiosGuid = Guid.NewGuid().ToString();

                if (netbiosName.Trim().Length == 0 || macAddress.Trim().Length == 0)
                {
                    throw new ArgumentNullException("Computer Name and Mac Address must be defined");
                }

                // Reformat macAddress to : separator.
                if (string.IsNullOrEmpty(macAddress) == false)
                {
                    macAddress = macAddress.Replace("-", ":");
                }

                // Create the computer.
                //Documentation for ImportMachineEntry method: http://msdn.microsoft.com/en-us/library/hh949040.aspx
                Dictionary<string, object> inParams = new Dictionary<string, object>();
                inParams.Add("NetbiosName", netbiosName);
                //inParams.Add("SMBIOSGUID", smBiosGuid);
                inParams.Add("MACAddress", macAddress);
                inParams.Add("OverwriteExistingRecord", false);

                IResultObject outParams = connection.ExecuteMethod(
                    "SMS_Site",
                    "ImportMachineEntry",
                    inParams);

                connection.Close();
                connection.Dispose();

                Dictionary<string, object> resultData = new Dictionary<string, object>();
                resultData.Add("Device Id", Convert.ToUInt32(outParams.PropertyList["ResourceID"]));
                return new ResultData("Done", resultData);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "NetBIOS Name"),
            new DataDescription(new DecisionsNativeType(typeof(string)), "MAC Address")};
            }
        }
    }
}