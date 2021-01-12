using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Advertisment by Id", "SCCM Steps")]
    //[AutoRegisterAgentFlowElementStep("Get Advertisment by Id", "SCCM Steps")]
    [Writable]
    public class SCCMGetAdvertisementById : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMAdvertisment)), "Advertisement", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string advertisementId = data.Data["Advertisement Id"] as string;

            try
            {
                IResultObject oAdvert = GetAdvertisementObjectById(advertisementId);

                if (oAdvert == null)
                {
                    return new ResultData("No Data");
                }

                else
                {
                    SCCMAdvertisment collection = new SCCMAdvertisment(oAdvert);

                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Advertisement", collection);
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
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Advertisement Id") }; }
        }
    }
}
