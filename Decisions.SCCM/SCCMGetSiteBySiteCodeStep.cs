using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Site by Site Code", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Site by Site Code", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSiteBySiteCodeStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

            string siteCode = data.Data["Site Code"] as string;

            try
            {
                string queryFromWhere = string.Format("FROM SMS_Site where SiteCode = '{0}'", siteCode);
                string querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
                string querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

                if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
                {
                    throw new Exception(string.Format("More than one site was found by site code '{0}'", siteCode));
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Site Code") }; }
        }
    }
}