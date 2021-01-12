using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Site by Server Name", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Site by Server Name", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSiteByServerNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSite)), "Site", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string serverName = data.Data["Server Name"] as string;

            try
            {
                string queryFromWhere = string.Format("FROM SMS_Site where ServerName = '{0}'", serverName);
                string querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
                string querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

                if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
                {
                    throw new Exception(string.Format("More than one site was found by server name {0}", serverName));
                }

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(querySelectStar);

                foreach (IResultObject result in queryResults)
                {
                    SCCMSite site = new SCCMSite(result);

                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Site", site);
                    return new ResultData("Done", resultData);
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }

            return new ResultData("No Data");
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Server Name") }; }
        }
    }
}