using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Edit Boundary by ID", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Edit Boundary by ID", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMEditBoundaryByIDStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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
            WqlConnectionManager connection = GetWqlConnection();

            UInt32 boundaryId = Convert.ToUInt32(data.Data["Boundary Id"]);
            string updatedBoundaryDisplayName = data.Data["Updated Boundary Display Name"] as string;
            string updatedBoundaryValue = data.Data["Updated Boundary Value"] as string;
            string updatedBoundaryType = data.Data["Upated Boundary Type"] as string;

            IResultObject oBoundary = GetBoundaryObjectById(boundaryId);

            try
            {
                oBoundary["DisplayName"].StringValue = updatedBoundaryDisplayName;
                oBoundary["Value"].StringValue = updatedBoundaryValue;
                oBoundary["BoundaryType"].StringValue = GetBoundaryTypeIntFromTypeName(updatedBoundaryType).ToString();

                oBoundary.Put();
                return new ResultData("Done");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            //UInt32 boundaryId, string updatedBoundaryValue, string updatedBoundaryDisplayName, string updatedBoundaryType)
            get
            {
                return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Id"),
                new DataDescription(new DecisionsNativeType(typeof(string)), "Updated Boundary Value"),
            new DataDescription(new DecisionsNativeType(typeof(string)), "Updated Boundary Display Name"),
                new DataDescription(new DecisionsNativeType(typeof(string)), "Upated Boundary Type")};
            }
        }
    }
}