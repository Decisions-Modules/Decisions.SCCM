﻿using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Remove Assigned Site from Boundary Group", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Remove Assigned Site from Boundary Group", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMRemoveAssignedSiteFromBoundaryGroupStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            UInt32 boundaryGroupId = Convert.ToUInt32(data.Data["Boundary Group Id"]);
            IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

            try
            {
                oBoundaryGroup["DefaultSiteCode"].StringValue = "";
                oBoundaryGroup.Put();

                return new ResultData("Done");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Group Id") }; }
        }
    }
}