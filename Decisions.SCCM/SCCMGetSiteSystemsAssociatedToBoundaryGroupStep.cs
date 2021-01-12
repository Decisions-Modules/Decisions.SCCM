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
    [AutoRegisterStep("Get Site Systems Associated to Boundary Group", "SCCM Steps", "Site")]
    //[AutoRegisterAgentFlowElementStep("Get Site Systems Associated to Boundary Group", "SCCM Steps", "Site")]
    [Writable]
    public class SCCMGetSiteSystemsAssociatedToBoundaryGroupStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

            UInt32 boundaryGroupId = Convert.ToUInt32(data.Data["Boundary Group Id"]);

            try
            {
                string queryboundaryGroupSiteSystems = string.Format("SELECT ServerNALPath, SiteCode from SMS_BoundaryGroupSiteSystems where GroupID = '{0}'", boundaryGroupId);
                
                IResultObject boundGroupSiteSysResults = connection.QueryProcessor.ExecuteQuery(queryboundaryGroupSiteSystems);

                List<SCCMSiteSystem> siteSystems = new List<SCCMSiteSystem>();

                foreach (IResultObject result in boundGroupSiteSysResults)
                {
                    
                    string nalPath = result["ServerNALPath"].StringValue;
                    
                    //WMI SQL requires that the \ character be escaped. The following line does that for us in the nalPath variable.
                    //The documentation for this can be found here: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394054(v=vs.85).aspx
                    //This documentation also says that the " charater should be escapted with a \ character, but this doesn't work.
                    //When we try to escape the " character with a \ character the query engine returns either of these two errors: "InvalidQuery" or "Failed to parse WQL string"
                    //When only escaping the \ character the query works fine.
                    string nalPathEscapedForWmiSql = nalPath.Replace(@"\", @"\\");
                    
                    string querySiteSystemResources = string.Format(@"SELECT * FROM SMS_SystemResourceList where NALPath = '{0}' AND (RoleName = 'SMS Distribution Point' OR RoleName = 'SMS State Migration Point')", nalPathEscapedForWmiSql);
                                                 
                    IResultObject sysResListResults = connection.QueryProcessor.ExecuteQuery(querySiteSystemResources);

                    foreach (IResultObject oSiteSystem in sysResListResults)
                    {
                        if (oSiteSystem != null)
                        {
                            SCCMSiteSystem siteSys = new SCCMSiteSystem(oSiteSystem);
                            if (!siteSystems.Exists(s => s.NALPath == siteSys.NALPath))
                            {
                                siteSystems.Add(siteSys);
                            }
                        }
                    }
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
                return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Group Id") };
            }                
        }
    }
}