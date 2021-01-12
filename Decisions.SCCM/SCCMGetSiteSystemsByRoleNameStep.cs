using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Properties;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Site Systems by Role Name", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Site Systems by Role Name", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSiteSystemsByRoleNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSiteSystem)), "Site Systems", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string roleName = data.Data["Role Name"] as string;

            try
            {
                string query = string.Format("SELECT * FROM SMS_SystemResourceList where RoleName = '{0}'", roleName);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMSiteSystem> siteSystems = new List<SCCMSiteSystem>();

                foreach (IResultObject result in queryResults)
                {
                    SCCMSiteSystem siteSys = new SCCMSiteSystem(result);
                    siteSystems.Add(siteSys);
                }

                if (siteSystems == null || siteSystems.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Site Systems", siteSystems.ToArray());
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
            get
            {
                return new DataDescription[] { new DataDescription((DecisionsNativeType)typeof(string), "Role Name") { EditorAttribute = new SelectStringEditorAttribute(new string[] { 
                    "AI Update Service Point",
                    "SMS AMT Service Point",
                    "SMS Application Web Service",
                    "SMS Component Server",
                    "SMS Distribution Point",
                    "SMS Endpoint Protection Point",
                    "SMS Enrollment Server",
                    "SMS Enrollment Web Site",
                    "SMS Fallback Status Point",
                    "SMS Management Point",
                    "SMS Portal Web Site",
                    "SMS Provider",
                    "SMS Site Server",
                    "SMS Site System",
                    "SMS Software Update Point",
                    "SMS SQL Server",
                    "SMS SRS Reporting Point",
                    "SMS State Migration Point",
                    "SMS System Health Validator" }) } };
            }                
                //return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Role Name") }; }
        }
    }
}