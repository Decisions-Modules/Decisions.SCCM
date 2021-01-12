using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Delete Boundary", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Delete Boundary", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMDeleteBoundaryStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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

           UInt32 boundaryId = Convert.ToUInt32(data.Data["Boundary Id"]);

           IResultObject oBoundary = GetBoundaryObjectById(boundaryId);

           try
           {
               oBoundary.Delete();

               return new ResultData("Done");
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