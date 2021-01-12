using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Create Boundary", "SCCM Steps", "Boundary")]
    //[AutoRegisterAgentFlowElementStep("Create Boundary", "SCCM Steps", "Boundary")]
    [Writable]
    public class SCCMCreateBoundaryStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Boundary Id", false, true, false)})
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string boundaryValue = data.Data["Boundary Value"] as string;
            string boundaryDisplayName = data.Data["Boundary Display Name"] as string;
            string boundaryType = data.Data["Boundary Type"] as string;

            try
            {
                IResultObject oExistingBoundary = GetBoundaryObjectByName(boundaryDisplayName);
                if (oExistingBoundary == null)
                {
                    int type = GetBoundaryTypeIntFromTypeName(boundaryType);

                    IResultObject oBoundary = connection.CreateInstance("SMS_Boundary");
                    oBoundary["DisplayName"].StringValue = boundaryDisplayName;
                    oBoundary["BoundaryType"].IntegerValue = type;
                    oBoundary["Value"].StringValue = boundaryValue;
                    oBoundary.Put();
                    oBoundary.Get();

                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Boundary Id", Convert.ToUInt32(oBoundary["BoundaryID"].IntegerValue));
                    return new ResultData("Done", resultData);
                }
                else
                {
                    SCCMBoundary existingBoundary = new SCCMBoundary(oExistingBoundary);
                    throw new Exception(string.Format("Cannot add boundary '{0}'. Boundary with id of {1} already exists with that same name.", boundaryDisplayName, existingBoundary.BoundaryID));
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
                return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Boundary Value"),
                new DataDescription(new DecisionsNativeType(typeof(string)), "Boundary Display Name"),
            new DataDescription(new DecisionsNativeType(typeof(string)), "Boundary Type")};
            }
        }
    }
}