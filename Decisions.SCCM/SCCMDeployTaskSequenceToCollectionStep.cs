using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;

namespace SCCM_2012
{
    [AutoRegisterStep("Deploy Task Sequence To Collection", "SCCM Steps", "Task Sequence")]
    //[AutoRegisterAgentFlowElementStep("Deploy Task Sequence To Collection", "SCCM Steps", "Task Sequence")]
    [Writable]
    public class SCCMDeployTaskSequenceToCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer
    {
        public override DecisionsFramework.Design.Flow.Mapping.OutcomeScenarioData[] OutcomeScenarios
        {
            get {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(string)), "Advertisement Id", false, true, false)})
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();
            string collectionId = data.Data["Collection Id"] as string;
            string taskSequenceId = data.Data["Task Sequence Id"] as string;
            bool useGtmStartTime = (bool)data.Data["Use UTC Start Time"];
            DateTime scheduledStartTime = Convert.ToDateTime(data.Data["Scheduled Start Time"]);
            bool setScheduledExpirationTime = (bool)data.Data["Set Scheduled Expiration Time"];
            bool useGtmExpirationTime = (bool)data.Data["Use UTC Expiration Time"];
            DateTime scheduledExpirationTime = Convert.ToDateTime(data.Data["Scheduled Expiration Time"]);

            try
            {
                String PackageName = "New Package"; //Will update later if package name is found in query below

                string query = "select * from SMS_TaskSequencePackage where PackageID = '" + taskSequenceId + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject queryResult in queryResults)
                    PackageName = queryResult["Name"].StringValue;

                //For details on the SMS_Advertisement class including HEX values for flags see: http://msdn.microsoft.com/en-us/library/hh948575.aspx
                IResultObject DeployTaskSequences = connection.CreateInstance("SMS_Advertisement");
                DeployTaskSequences["CollectionID"].StringValue = collectionId;
                DeployTaskSequences["PackageID"].StringValue = taskSequenceId;
                DeployTaskSequences["AdvertisementName"].StringValue = PackageName;

                //Set DateTime
                DeployTaskSequences["PresentTimeIsGMT"].BooleanValue = useGtmStartTime;
                DeployTaskSequences["PresentTime"].DateTimeValue = scheduledStartTime;
                DeployTaskSequences["ExpirationTimeEnabled"].BooleanValue = setScheduledExpirationTime;
                DeployTaskSequences["ExpirationTimeIsGMT"].BooleanValue = useGtmExpirationTime;
                DeployTaskSequences["ExpirationTime"].DateTimeValue = scheduledExpirationTime;
                DeployTaskSequences["AssignedScheduleEnabled"].BooleanValue = true;

                DeployTaskSequences["ProgramName"].StringValue = "*";
                
                //Required=0 Available=2 : http://screencast.com/t/HWAm3vmwQw8
                DeployTaskSequences["OfferType"].IntegerValue = 0;

                /*Flag Details
                 * For details on queries that allow you to inspect the values of these flags in the DB see: http://myitforum.com/cs2/blogs/jnelson/archive/2011/06/21/158110.aspx
                 * 0x00400000 = Send wake-up packets http://screencast.com/t/HWAm3vmwQw8
                 * 0x00000020 = Assign Schedule - As soon as possible : http://screencast.com/t/bvSTXY2O
                 * 0x00800000 = Show task sequence progress http://screencast.com/t/qcYPLOGv
                 * 0x00200000 = System Restart http://screencast.com/t/qcYPLOGv
                 * 0x00080000 = Allow task sequence to run for client on the Internet (setting this sets option to false, otherwise it will be true)
                */

                UInt32 obj = 0;
                obj = 0x00000020 | 0x00800000 | 0x00200000 | 0x00100000 | 0x02000000 | 0x00020000 | 0x00080000;

                DeployTaskSequences["AdvertFlags"].LongValue = obj;

                /*Flag Details
                 * 0x00001000 = Never rerun the program http://screencast.com/t/bvSTXY2O
                 * 0x00000100 = Download content locally when needed by running task sequence http://screencast.com/t/4dJHwQALVR18
                 */
                DeployTaskSequences["RemoteClientFlags"].LongValue = 0x00000100 | 0x00000020 | 0x00002000;
                
                DeployTaskSequences.Put();

                DeployTaskSequences.Get();

                Dictionary<string, object> resultData = new Dictionary<string, object>();
                resultData.Add("Advertisement Id", DeployTaskSequences["AdvertisementID"].StringValue);
                return new ResultData("Done", resultData);
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
                return new DataDescription[] { 
                    new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id"),
                    new DataDescription(new DecisionsNativeType(typeof(string)), "Task Sequence Id"),
                    new DataDescription(new DecisionsNativeType(typeof(DateTime)), "Scheduled Start Time"),
                    new DataDescription(new DecisionsNativeType(typeof(bool)), "Use UTC Start Time"),
                    new DataDescription(new DecisionsNativeType(typeof(bool)), "Set Scheduled Expiration Time"),
                    new DataDescription(new DecisionsNativeType(typeof(DateTime)), "Scheduled Expiration Time"),
                    new DataDescription(new DecisionsNativeType(typeof(bool)), "Use UTC Expiration Time")};
            }
        }
    }
}
