using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Create Boundary Group", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Create Boundary Group", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMCreateBoundaryGroupStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Group Id", false, true, false)})
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string groupName = data.Data["Group Name"] as string;
            string groupDescription = data.Data["Group Description"] as string;

            try
            {
                IResultObject oExistingBoundaryGroup = GetBoundaryGroupObjectByName(groupName);
                if (oExistingBoundaryGroup == null)
                {
                    IResultObject gBoundary = connection.CreateInstance("SMS_BoundaryGroup");
                    gBoundary["Name"].StringValue = groupName;
                    gBoundary["Description"].StringValue = groupDescription;
                    gBoundary["DefaultSiteCode"].StringValue = "";
                    gBoundary.Put();
                    gBoundary.Get();

                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Boundary Group Id", Convert.ToUInt32(gBoundary["GroupID"].IntegerValue));
                    return new ResultData("Done", resultData);
                }
                else
                {
                    SCCMBoundaryGroup existingBoundaryGroup = new SCCMBoundaryGroup(oExistingBoundaryGroup);
                    throw new Exception(string.Format("Cannot add boundary group '{0}'. Boundary group with id of {1} already exists with that same name.", groupName, existingBoundaryGroup.GroupID));
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
                return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Group Name"),
            new DataDescription(new DecisionsNativeType(typeof(string)), "Group Description")};
            }
        }
    }
}