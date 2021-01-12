using System;
using System.Collections.Generic;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Flow.Mapping;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using DecisionsFramework.Design.Properties;
using System.ComponentModel;

namespace SCCM_2012
{
    [AutoRegisterStep("Deploy Application to Collection", "SCCM Steps", "Application")]
    //[AutoRegisterAgentFlowElementStep("Deploy Application to Collection", "SCCM Steps", "Application")]
    [Writable]
    public class SCCMDeployApplicationToCollectionStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer, INotifyPropertyChanged
    {
        private const string INPUT_SEND_WAKE_UP_PACKETS = "Send wake-up packets";
        private const string INPUT_PURPOSE = "Purpse";

        [WritableValue]
        private ApplicationIdType applicationIdField;

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
            //Documentation on SMS_ApplicationAssignment Server WMI Class
            //http://msdn.microsoft.com/en-us/library/hh949469.aspx

            //Helpful forum post on how to build an SMS_ApplicationAssignment object
            //http://social.technet.microsoft.com/Forums/en-US/configmanagersdk/thread/a910ef16-ff58-4724-a4a8-c0205ed42c81

            WqlConnectionManager connection = GetWqlConnection();
            string collectionId = data.Data["Collection Id"] as string;
            bool useGtmTimes = Convert.ToBoolean(data.Data["Use GTM Times"]);
            UInt32 applicationId = 0;

            switch (ApplicationIdField)
            {
                case ApplicationIdType.CI_ID:
                    applicationId = Convert.ToUInt32(data.Data["CI ID"]);
                    break;
                case ApplicationIdType.ModelID:
                    applicationId = Convert.ToUInt32(data.Data["Model ID"]);
                    break;
                default:
                    throw new Exception(string.Format("Getting application by id field of {0} is not handled at this time", Enum.GetName(typeof(ApplicationIdType), applicationIdField)));
            }
            
            //DesiredConfigType: 1 = Install, 2 = Uninstall
            int desiredConfigType = 1;
            if ((bool)data.Data["Uninstall Application"])
            { desiredConfigType = 2; }

            DateTime scheduledStartTime = Convert.ToDateTime(data.Data["Scheduled Start Time"]);
            DateTime installationDeadline = Convert.ToDateTime(data.Data["Installation Deadline"]);
            bool sendWolPackets = (bool)data.Data[INPUT_SEND_WAKE_UP_PACKETS];

            TaskSequenceDeploymentPurpose offerType = (TaskSequenceDeploymentPurpose)data.Data[INPUT_PURPOSE];
            int offerTypeInt = (int)Enum.Parse(typeof(TaskSequenceDeploymentPurpose), offerType.ToString());

            try
            {                
                SCCMApplication appToDeploy = new SCCMApplication(GetApplicationObjectById(applicationId, ApplicationIdField));

                IResultObject oSMS_ApplicationAssignment = connection.CreateInstance("SMS_ApplicationAssignment");
                oSMS_ApplicationAssignment["ApplicationName"].StringValue = appToDeploy.LocalizedDisplayName;
                oSMS_ApplicationAssignment["ApplyToSubTargets"].BooleanValue = false;
                oSMS_ApplicationAssignment["AppModelID"].IntegerValue = Convert.ToInt32(appToDeploy.ModelID);
                oSMS_ApplicationAssignment["AssignedCI_UniqueID"].StringValue = appToDeploy.CI_UniqueID;
                oSMS_ApplicationAssignment["AssignedCIs"].IntegerArrayValue = new Int32[] { Convert.ToInt32(appToDeploy.CI_ID) };
                oSMS_ApplicationAssignment["AssignmentAction"].IntegerValue = 2;
                oSMS_ApplicationAssignment["AssignmentDescription"].StringValue = "";
                oSMS_ApplicationAssignment["AssignmentName"].StringValue = appToDeploy.LocalizedDisplayName + " Install";
                oSMS_ApplicationAssignment["AssignmentType"].IntegerValue = 2;
                oSMS_ApplicationAssignment["ContainsExpiredUpdates"].BooleanValue = false;
                oSMS_ApplicationAssignment["CreationTime"].DateTimeValue = DateTime.Now;
                oSMS_ApplicationAssignment["DesiredConfigType"].IntegerValue = desiredConfigType;
                oSMS_ApplicationAssignment["DisableMomAlerts"].BooleanValue = false;
                oSMS_ApplicationAssignment["DPLocality"].IntegerValue = 80;
                oSMS_ApplicationAssignment["Enabled"].BooleanValue = true;
                
                //It is only valid to set installationDeadline if the offer type is Required.
                //If you try to set installation deadline when offer type is "Available" 
                //you will get the followin erro from SCCM: "Property EnforcementDeadline cannot be set for this offer type"
                if (offerType == TaskSequenceDeploymentPurpose.Required)
                {
                    oSMS_ApplicationAssignment["EnforcementDeadline"].DateTimeValue = installationDeadline;
                }

                oSMS_ApplicationAssignment["EvaluationSchedule"].StringValue = null;
                oSMS_ApplicationAssignment["ExpirationTime"].StringValue = null;
                oSMS_ApplicationAssignment["LastModificationTime"].DateTimeValue = DateTime.Now;
                oSMS_ApplicationAssignment["LastModifiedBy"].StringValue = Environment.UserName;
                oSMS_ApplicationAssignment["LocaleID"].StringValue = "1033";
                oSMS_ApplicationAssignment["LogComplianceToWinEvent"].BooleanValue = false;
                oSMS_ApplicationAssignment["NonComplianceCriticality"].StringValue = null;
                oSMS_ApplicationAssignment["NotifyUser"].BooleanValue = true;
                oSMS_ApplicationAssignment["OfferFlags"].IntegerValue = 0;
                oSMS_ApplicationAssignment["OfferTypeID"].IntegerValue = offerTypeInt;
                oSMS_ApplicationAssignment["OverrideServiceWindows"].BooleanValue = true;
                oSMS_ApplicationAssignment["Priority"].IntegerValue = 1;
                oSMS_ApplicationAssignment["RaiseMomAlertsOnFailure"].BooleanValue = false;
                oSMS_ApplicationAssignment["RebootOutsideOfServiceWindows"].BooleanValue = true;
                oSMS_ApplicationAssignment["RequireApproval"].BooleanValue = false;
                oSMS_ApplicationAssignment["SendDetailedNonComplianceStatus"].BooleanValue = false;
                oSMS_ApplicationAssignment["StartTime"].DateTimeValue = scheduledStartTime;
                oSMS_ApplicationAssignment["StateMessagePriority"].IntegerValue = 5;
                oSMS_ApplicationAssignment["SuppressReboot"].IntegerValue = 0;
                oSMS_ApplicationAssignment["TargetCollectionID"].StringValue = collectionId;
                oSMS_ApplicationAssignment["UpdateDeadline"].StringValue = null;
                oSMS_ApplicationAssignment["UpdateSupersedence"].BooleanValue = false;
                oSMS_ApplicationAssignment["UseGMTTimes"].BooleanValue = useGtmTimes;
                oSMS_ApplicationAssignment["UserUIExperience"].BooleanValue = true;
                oSMS_ApplicationAssignment["WoLEnabled"].BooleanValue = sendWolPackets;

                oSMS_ApplicationAssignment.Put(); 
                
                return new ResultData("Done");
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
                List<DataDescription> dd = new List<DataDescription>();

                //dd.Add(new DataDescription(new DecisionsNativeType(typeof(string)), "Folder Path"));
                if (this.ApplicationIdField == ApplicationIdType.CI_ID)
                {
                    dd.Add(new DataDescription(new DecisionsNativeType(typeof(string)), "CI ID"));
                }
                if (this.ApplicationIdField == ApplicationIdType.ModelID)
                {
                    dd.Add(new DataDescription(new DecisionsNativeType(typeof(string)), "Model ID"));
                }

                dd.Add(new DataDescription(new DecisionsNativeType(typeof(string)), "Collection Id"));
                dd.Add(new DataDescription(new DecisionsNativeType(typeof(bool)), "Use GTM Times"));
                dd.Add(new DataDescription(new DecisionsNativeType(typeof(bool)), "Uninstall Application"));
                dd.Add(new DataDescription(new DecisionsNativeType(typeof(DateTime)), "Scheduled Start Time"));
                dd.Add(new DataDescription(new DecisionsNativeType(typeof(DateTime)), "Installation Deadline"));
                dd.Add(new DataDescription(new DecisionsNativeType(typeof(bool)), INPUT_SEND_WAKE_UP_PACKETS));
                dd.Add(new DataDescription(new DecisionsNativeType(typeof(TaskSequenceDeploymentPurpose)), INPUT_PURPOSE));

                return dd.ToArray();
            }
        }

        [PropertyClassification(new string[] { "Inputs" }, "Application ID Field", 0)]
        public ApplicationIdType ApplicationIdField
        {
            get { return applicationIdField; }
            set
            {
                applicationIdField = value;
                //Call OnPropertyChanged method for each property you want to update
                this.OnPropertyChanged("ApplicationIdField");
                //If any of the inputs you want to update are in InputData (not a property),
                //you need to update InputData and shown below.
                this.OnPropertyChanged("InputData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
