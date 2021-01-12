using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Task Sequence by Id", "SCCM Steps", "Task Sequence")]
    //[AutoRegisterAgentFlowElementStep("Get Task Sequence by Id", "SCCM Steps", "Task Sequence")]
    [Writable]
    public class SCCMGetTaskSequenceByIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMTaskSequence)), "Task Sequence", false, true, false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            string taskSequenceId = data.Data["Task Sequence Id"] as string;
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select * from SMS_TaskSequencePackage  WHERE PackageID = '" + taskSequenceId + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                SCCMTaskSequence taskSequence = null;

                foreach (IResultObject queryResult in queryResults)
                {
                    taskSequence = new SCCMTaskSequence(queryResult);
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Task Sequence", taskSequence);
                    return new ResultData("Done", resultData);
                }                
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }

            return new ResultData("No Data");
        }

        public DataDescription[] InputData
        {
            get { return new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Task Sequence Id") }; }
        }
    }
}
