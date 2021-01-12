using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Delete Device by Id", "SCCM Steps", "Device")]
    //[AutoRegisterAgentFlowElementStep("Delete Device by Id", "SCCM Steps", "Device")]
    [Writable]
    public class SCCMDeleteDeviceByIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
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
            UInt32 deviceId = Convert.ToUInt32(data.Data["Device Id"]);
            IResultObject oDevice = GetDeviceObjectById(deviceId);

            try
            {
                oDevice.Delete();

                return new ResultData("Done");
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(UInt32)), "Device Id") }; }
        }
    }
}