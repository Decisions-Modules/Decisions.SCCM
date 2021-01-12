using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Remove Site Server from Boundary Group", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Remove Site Server from Boundary Group", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMRemoveSiteServerFromBoundaryGroupStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
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

            string serverName = data.Data["Server Name"] as string;
            string siteCode = data.Data["Site Code"] as string;
            UInt32 boundaryGroupId = (UInt32)data.Data["Boundary Group Id"];

            try
            {
                string serverNALPath = GetServerNalPath(serverName, siteCode);

                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                methodParams.Add("ServerNALPath", new string[1] { serverNALPath });

                IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

                oBoundaryGroup.ExecuteMethod("RemoveSiteSystem", methodParams);

                return new ResultData("Done");

            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Site Code"),
                new DataDescription(new DecisionsNativeType(typeof(string)), "Server Name"),
            new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Group Id")}; }
        }
    }
}