using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;

namespace SCCM_2012
{
    [AutoRegisterStep("Get All Packages", "SCCM Steps")]
    //[AutoRegisterAgentFlowElementStep("Get All Packages", "SCCM Steps")]
    [Writable]
    public class SCCMGetAllPackagesStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMPackage)), "All Packages", true,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select PackageID,Name,Manufacturer,Version,Language from SMS_Package  ";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMPackage> packages = new List<SCCMPackage>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMPackage package = new SCCMPackage(queryResult);

                    packages.Add(package);
                }

                if (packages == null || packages.Count == 0)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("All Packages", packages.ToArray());
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
