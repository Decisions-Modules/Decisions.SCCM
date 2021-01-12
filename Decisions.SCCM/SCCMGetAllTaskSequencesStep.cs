using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Task Sequences", "SCCM Steps", "Task Sequence")]
    //[AutoRegisterAgentFlowElementStep("Get All Task Sequences", "SCCM Steps", "Task Sequence")]
    [Writable]
    public class SCCMGetAllTaskSequencesStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMTaskSequence)), "All Task Sequences", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select * from SMS_TaskSequencePackage   ";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMTaskSequence> TaskSequences = new List<SCCMTaskSequence>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMTaskSequence taskSeqence = new SCCMTaskSequence(queryResult);

                    TaskSequences.Add(taskSeqence);
                }

                if (TaskSequences == null || TaskSequences.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Task Sequences", TaskSequences.ToArray());
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
            get { return null; }
        }
    }
}
