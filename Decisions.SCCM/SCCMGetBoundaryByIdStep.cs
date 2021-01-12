using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Boundary by Id", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Get Boundary by Id", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMGetBoundaryByIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMBoundary)), "Boundary", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            UInt32 boundaryId = Convert.ToUInt32(data.Data["Boundary Id"]);

            try
            {
                IResultObject oboundary = GetBoundaryObjectById(boundaryId);
                
                if (oboundary == null)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    SCCMBoundary boundary = new SCCMBoundary(oboundary);
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Boundary", boundary);
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Id") }; }
        }
    }
}