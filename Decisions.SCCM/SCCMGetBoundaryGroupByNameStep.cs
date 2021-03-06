﻿using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Boundary Group by Name", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Get Boundary Group by Name", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMGetBoundaryGroupByNameStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMBoundaryGroup)), "Boundary Group", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string boundaryGroupName = data.Data["Boundary Group Name"] as string;

            try
            {
                IResultObject oBoundaryGroup = GetBoundaryGroupObjectByName(boundaryGroupName);

                if (oBoundaryGroup == null)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    SCCMBoundaryGroup boundaryGroup = new SCCMBoundaryGroup(oBoundaryGroup);
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Boundary Group", boundaryGroup);
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Boundary Group Name") }; }
        }
    }
}