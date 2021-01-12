using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;
using DecisionsFramework.Design.Flow.Mapping;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using System.ComponentModel;
using DecisionsFramework.Design.Properties;

namespace SCCM_2012
{
    [AutoRegisterStep("Get Application by Id", "SCCM Steps", "Application")]
    //[AutoRegisterAgentFlowElementStep("Get Application by Id", "SCCM Steps", "Application")]
    [Writable]
    public class SCCMGetApplicationByIdStep : SCCMBaseStep, ISyncStep, IDataConsumer, IDataProducer, INotifyPropertyChanged
    {
        [WritableValue]
        private ApplicationIdType applicationIdField;

        public override OutcomeScenarioData[] OutcomeScenarios
        {
            get
            {
                return new OutcomeScenarioData[] { 
                    new OutcomeScenarioData("Done",new DataDescription[] { new DataDescription(new DecisionsNativeType(typeof(SCCMApplication)), "Application", false,true,false)}),
                    new OutcomeScenarioData("No Data")
                };
            }
        }

        public ResultData Run(StepStartData data)
        {
            WqlConnectionManager connection = GetWqlConnection();

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

            /*
            if (ApplicationIdField == applicationIdType.CI_ID)
            {
                applicationId = Convert.ToUInt32(data.Data["CI ID"]);
            }

            if (ApplicationIdField == applicationIdType.ModelID)
            {
                applicationId = Convert.ToUInt32(data.Data["Model ID"]);
            }

            else
            {
                throw new Exception(string.Format("Getting application by id field of {0} is not handled at this time", Enum.GetName(typeof(applicationIdType), applicationIdField)));
            }
             */

            try
            {
                IResultObject oApplication = GetApplicationObjectById(applicationId, ApplicationIdField);

                if (oApplication == null)
                {
                    return new ResultData("No Data");
                }
                else
                {
                    SCCMApplication application = new SCCMApplication(oApplication);
                    Dictionary<string, object> resultData = new Dictionary<string, object>();
                    resultData.Add("Application", application);
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
