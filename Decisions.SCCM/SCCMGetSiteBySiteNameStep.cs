using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Site by Site Name", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Site by Site Name", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSiteBySiteNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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
            

            string siteName = data.Data["Site Name"] as string;

            try
            {
                SCCMSite site = getSiteBySiteName(siteName);

                //If we didn't get a site using that name, we'll try modifying the siteName and trying again.
                //The SCCM UI lists site name like "PH1 - PHSCCM" (i.e. SiteCode - SiteName) even though the 
                //site name really is just "PHSCCM".
                //Because of this discrepancy a user name have incorrectly entered the site name by copying the value from SCCM UI.
                if (site == null)
                {
                    string[] siteNameBits = siteName.Split(new char[] { ' ' });
                    siteName = siteNameBits[siteNameBits.Length - 1];
                    site = getSiteBySiteName(siteName);

                    if (site == null)
                    {
                        return new ResultData("No Data");
                    }
                }

                Dictionary<string, object> resultData = new Dictionary<string, object>();
                resultData.Add("Site", site);
                return new ResultData("Done", resultData);
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }

            
        }

        private SCCMSite getSiteBySiteName(string siteName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string queryFromWhere = string.Format("FROM SMS_Site where SiteName = '{0}'", siteName);
            string querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
            string querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

            if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
            {
                throw new Exception(string.Format("More than one site was found by site name {0}", siteName));
            }

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(querySelectStar);

                foreach (IResultObject result in queryResults)
                {
                    return new SCCMSite(result);
                }
                return null;
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Site Name") }; }
        }
    }
}