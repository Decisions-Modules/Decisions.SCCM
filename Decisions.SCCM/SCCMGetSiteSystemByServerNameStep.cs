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
    [AutoRegisterStep("Get Site System by Server Name", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Site System by Server Name", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSiteSystemByServerNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSiteSystem)), "Site System", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string serverName = data.Data["Server Name"] as string;

            //The SCCM UI lists the server name like \\SCCM1.ph.local. But the real server name
            //doesn't include the \\ so we are stripping that out in case the user entered it here.
            serverName = serverName.Replace(@"\\", "");

            try
            {
                string queryFromWhere = string.Format("FROM SMS_SystemResourceList where ServerName = '{0}' AND RoleName = 'SMS Site System'", serverName);
                string querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
                string querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

                if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
                {
                    throw new Exception(string.Format("More than one site system was found by server name '{0}' and role name 'SMS Site System'", serverName));
                }

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(querySelectStar);

                List<SCCMSiteSystem> siteSystems = new List<SCCMSiteSystem>();

                foreach (IResultObject result in queryResults)
                {
                    SCCMSiteSystem siteSys = new SCCMSiteSystem(result);
                    
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Site System", siteSys);
                    return new ResultData("Done", resultData);
                }
                return new ResultData("No Data");
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
                return new DataDescription[] { new DataDescription((DecisionsNativeType)typeof(string), "Server Name") };
            }                
        }
    }
}