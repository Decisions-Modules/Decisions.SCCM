﻿using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Add Boundaries to Boundary Group", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Add Boundaries to Boundary Group", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMAddBoundariesToBoundaryGroupStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

            UInt32[] boundaryIds = (UInt32[])data.Data["Boundary Ids"];

            UInt32 boundaryGroupId = Convert.ToUInt32(data.Data["Boundary Group Id"]);

            try
            {
                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                methodParams.Add("BoundaryID", boundaryIds);

                IResultObject oBounaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

                if (oBounaryGroup != null)
                {

                    oBounaryGroup.ExecuteMethod("AddBoundary", methodParams);
                }
                else
                {
                    throw new Exception(string.Format("Unable to add Boundaries to Boundary Group because Boundary Group with id {0} does not exist.", boundaryGroupId));
                }

                return new ResultData("Done");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Ids", true, false, false),
            new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Group Id")};
            }
        }
    }
}