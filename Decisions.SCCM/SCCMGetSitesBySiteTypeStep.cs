using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Sites by Site Type", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Sites by Site Type", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSitesBySiteTypeStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMSite)), "Sites", true, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            SiteType siteType = (SiteType)data.Data["Site Type"];

            UInt32 siteTypeUInt = (UInt32)Enum.Parse(typeof(SiteType), siteType.ToString());

            try
            {
                string query = string.Format("SELECT * FROM SMS_Site where Type = '{0}'", siteTypeUInt);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMSite> sites = new List<SCCMSite>();

                foreach (IResultObject result in queryResults)
                {
                    SCCMSite site = new SCCMSite(result);
                    sites.Add(site);
                }

                if (sites == null || sites.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Sites", sites.ToArray());
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SiteType)), "Site Type") }; }
        }
    }
}