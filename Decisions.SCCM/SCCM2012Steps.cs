using System;
using System.Collections.Generic;
using Microsoft.ConfigurationManagement.ManagementProvider;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using DecisionsFramework.Design.Flow;
using System.Runtime.Serialization;
using System.Net;
using DecisionsFramework;
using System.ComponentModel;
using DecisionsFramework.Design.Properties;

namespace SCCM_2012
{
    
    //[AutoRegisterMethodsOnClass(true, "SCCM", "SCCM Steps", RegisterForAgents = true)]
    public class SCCM2012Steps
    {
     
        const string WQLCACHEID = "wqlCacheId";

        private static TimeBoundCache<String, WqlConnectionManager> wqlConnectionCache = new TimeBoundCache<string, WqlConnectionManager>(TimeSpan.FromHours(1), TimeSpan.FromHours(1));

        private static WqlConnectionManager GetWqlConnection()
        {
            //WqlConnectionManager connection = wqlConnectionCache[WQLCACHEID];
            WqlConnectionManager connection = null;

            if (connection == null)
            {
                SCCMConnectionInfo settings = SCCMConnectionInfo.GetSettings();

                connection = ConnectToSCCM(settings.SCCMServerAddress, settings.SCCMServerUsername, settings.SCCMServerUserPassword);

                wqlConnectionCache[WQLCACHEID] = connection;
            }

            return connection;
        }

        private static WqlConnectionManager ConnectToSCCM(string serverName, string userName, string password)
        {
            SmsNamedValuesDictionary namedValues = new SmsNamedValuesDictionary();

            WqlConnectionManager connection = new WqlConnectionManager(namedValues);

            //SCCM does not allow you to specify username and password when building this 
            //connection object for a local connection. So we have this check to see if serverName 
            //is refering to the machine we are currently running on.
            try
            {
                if (serverName.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || serverName.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase) || serverName.Equals(Dns.GetHostEntry("localhost").HostName, StringComparison.InvariantCultureIgnoreCase))
                {
                    connection.Connect(serverName);
                }

                //Looks like we must be off box so we should use userName and password to build our connection object
                else
                {
                    connection.Connect(serverName, userName, password);
                }
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return connection;
        }
/*
        public static SCCMCollection[] GetAllCollections()
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select CollectionID,Name,LocalMemberCount from SMS_Collection ";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMCollection> collections = new List<SCCMCollection>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMCollection collection = new SCCMCollection(queryResult);

                    collections.Add(collection);

                }

                return collections.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMSystem[] GetCollectionItemsByCollectionID(String CollectionID)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String queryGetCollectionMemberByName = "select ResourceID, Name from SMS_FullCollectionMembership  WHERE CollectionID = '" + CollectionID + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                List<SCCMSystem> resources = new List<SCCMSystem>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMSystem resource = new SCCMSystem(queryResult);

                    resources.Add(resource);

                }

                return resources.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMSystem[] GetCollectionItemsByCollectionName(String CollectionName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                List<SCCMSystem> resources = new List<SCCMSystem>();

                String queryGetCollection = "select CollectionID,Name,LocalMemberCount from SMS_Collection where Name = '" + CollectionName + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(queryGetCollection);

                foreach (IResultObject queryResult in queryResults)
                {
                    if (queryResult["LocalMemberCount"].IntegerValue > 0)
                    {
                        String queryGetCollectionMemberByName = "select ResourceID, Name from SMS_FullCollectionMembership  WHERE CollectionID = '" + queryResult["CollectionID"].StringValue + "'";

                        IResultObject queryResults1 = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                        foreach (IResultObject queryResult1 in queryResults1)
                        {
                            SCCMSystem resource = new SCCMSystem(queryResult1);

                            resources.Add(resource);
                        }
                    }
                }

                return resources.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static string CreateCollection(string collectionName, string collectionComment)
        {
            try
            {
                IResultObject oCollection = CreateCollectionBaseMethod(collectionName, collectionComment);
                return oCollection["CollectionID"].StringValue.ToString();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static void AddDevicesToExistingCollection(string collectionId, UInt32[] resourceIds)
        {
            WqlConnectionManager connection = GetWqlConnection();

            IResultObject oCollection = GetCollectionObjectById(collectionId);

            try
            {
                foreach (UInt32 dId in resourceIds)
                {
                    IResultObject instDirectRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                    instDirectRule["ResourceClassName"].StringValue = "SMS_R_System";
                    instDirectRule["ResourceID"].IntegerValue = Convert.ToInt32(dId);
                    instDirectRule["RuleName"].StringValue = "DirectRuleForDevice";

                    Dictionary<string, object> methodParams = new Dictionary<string, object>();
                    methodParams.Add("collectionRule", instDirectRule);

                    oCollection.ExecuteMethod("AddMembershipRule", methodParams);
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static string CreateDynamicCollectionForDevices(string collectionName, string collectionComment, UInt32[] resourceIds)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                IResultObject oCollection = CreateCollectionBaseMethod(collectionName, collectionComment);

                foreach (UInt32 dId in resourceIds)
                {
                    IResultObject instDirectRule = connection.CreateInstance("SMS_CollectionRuleDirect");
                    instDirectRule["ResourceClassName"].StringValue = "SMS_R_System";
                    instDirectRule["ResourceID"].IntegerValue = Convert.ToInt32(dId);
                    instDirectRule["RuleName"].StringValue = "DirectRuleForDevice";

                    Dictionary<string, object> methodParams = new Dictionary<string, object>();
                    methodParams.Add("collectionRule", instDirectRule);

                    oCollection.ExecuteMethod("AddMembershipRule", methodParams);
                }

                return oCollection["CollectionID"].StringValue.ToString();
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        private static IResultObject CreateCollectionBaseMethod(string collectionName, string collectionComment)
        {
            //Possible room for improvements:
            //Check to see if collection already exists before creating it
            //Allow user to specifiy id of limiting collection. If they don't specify, then set to SMS00001
            //If they want to create user collections, either we need a new method for that, or allow as input to this method
            //since collection type 2 = device collection and type 1 = user collection
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                IResultObject oCollection = connection.CreateInstance("SMS_Collection");
                oCollection["Name"].StringValue = collectionName;
                oCollection["OwnedByThisSite"].BooleanValue = true;
                oCollection["Comment"].StringValue = collectionComment;
                oCollection["CollectionType"].StringValue = "2";
                oCollection["LimitToCollectionID"].StringValue = "SMS00001";
                oCollection.Put();
                oCollection.Get();

                return oCollection;
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static void RefreshCollectionMembership(string collectionId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            IResultObject oCollection = GetCollectionObjectById(collectionId);

            try
            {
                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                oCollection.ExecuteMethod("RequestRefresh", methodParams);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

        }

        public static SCCMPackage[] GetAllPackages()
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

                return packages.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMApplication[] GetAllApplications()
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select * from SMS_Application  ";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMApplication> applications = new List<SCCMApplication>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMApplication application = new SCCMApplication(queryResult);

                    applications.Add(application);
                }

                return applications.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMApplication GetApplicationById(UInt32 applicationId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select * from SMS_Application where CI_ID = '" + applicationId + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                SCCMApplication application = null;

                foreach (IResultObject queryResult in queryResults)
                {
                    application = new SCCMApplication(queryResult);
                }

                return application;
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMTaskSequence[] GetAllTaskSequences()
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

                return TaskSequences.ToArray();

            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMTaskSequence GetTaskSequenceById(string taskSequenceId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "select * from SMS_TaskSequencePackage  WHERE PackageID = '" + taskSequenceId + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                SCCMTaskSequence taskSequence = null;

                foreach (IResultObject queryResult in queryResults)
                {
                    taskSequence = new SCCMTaskSequence(queryResult);
                }

                return taskSequence;
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static void DeployTaskSequenceToCollection(string collectionId, string taskSequenceId, DateTime ScheduledStartTime)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String PackageName = "New Package"; //Will update later if package name is found in query below

                string query = "select * from SMS_TaskSequencePackage where PackageID = '" + taskSequenceId + "'";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject queryResult in queryResults)
                    PackageName = queryResult["Name"].StringValue.ToString();

                //For details on the SMS_Advertisement class including HEX values for flags see: http://msdn.microsoft.com/en-us/library/hh948575.aspx
                IResultObject DeployTaskSequences = connection.CreateInstance("SMS_Advertisement");
                DeployTaskSequences["CollectionID"].StringValue = collectionId;
                DeployTaskSequences["PackageID"].StringValue = taskSequenceId;
                DeployTaskSequences["AdvertisementName"].StringValue = PackageName;
                DeployTaskSequences["ExpirationTime"].DateTimeValue = ScheduledStartTime;
                DeployTaskSequences["ProgramName"].StringValue = "*";

                DeployTaskSequences["AssignedScheduleEnabled"].BooleanValue = true;

                //Required=0 Available=2 : http://screencast.com/t/HWAm3vmwQw8
                DeployTaskSequences["OfferType"].IntegerValue = 0;

                //Flag Details
                //  For details on queries that allow you to inspect the values of these flags in the DB see: http://myitforum.com/cs2/blogs/jnelson/archive/2011/06/21/158110.aspx
                //  0x00400000 = Send wake-up packets http://screencast.com/t/HWAm3vmwQw8
                //  0x00000020 = Assign Schedule - As soon as possible : http://screencast.com/t/bvSTXY2O
                //  0x00800000 = Show task sequence progress http://screencast.com/t/qcYPLOGv
                //  0x00200000 = System Restart http://screencast.com/t/qcYPLOGv
                //  0x00080000 = Allow task sequence to run for client on the Internet (setting this sets option to false, otherwise it will be true)
                

                UInt32 obj = 0;
                //obj = 45744160; //45744160 is a magic number that creates a task sequence deployment that works.
                //But we should be writing the flag individually as below. Setting individually works now.
                obj = 0x00000020 | 0x00800000 | 0x00200000 | 0x00100000 | 0x02000000 | 0x00020000 | 0x00080000;

                DeployTaskSequences["AdvertFlags"].LongValue = obj;

                //Flag Details
                //  0x00001000 = Never rerun the program http://screencast.com/t/bvSTXY2O
                //  0x00000100 = Download content locally when needed by running task sequence http://screencast.com/t/4dJHwQALVR18
                 
                DeployTaskSequences["RemoteClientFlags"].LongValue = 0x00000100 | 0x00000020 | 0x00002000;

                //Set DateTime
                DeployTaskSequences["PresentTime"].DateTimeValue = ScheduledStartTime;

                DeployTaskSequences.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void DeployApplicationToCollection(string targetCollectionId, UInt32 applicationId, DateTime scheduledStartTime)
        {
            //Documentation on SMS_ApplicationAssignment Server WMI Class
            //http://msdn.microsoft.com/en-us/library/hh949469.aspx

            //Helpful forum post on how to build an SMS_ApplicationAssignment object
            //http://social.technet.microsoft.com/Forums/en-US/configmanagersdk/thread/a910ef16-ff58-4724-a4a8-c0205ed42c81

            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                SCCMApplication appToDeploy = GetApplicationById(applicationId);

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
                oSMS_ApplicationAssignment["DesiredConfigType"].IntegerValue = 1;
                oSMS_ApplicationAssignment["DisableMomAlerts"].BooleanValue = false;
                oSMS_ApplicationAssignment["DPLocality"].IntegerValue = 80;
                oSMS_ApplicationAssignment["Enabled"].BooleanValue = true;
                oSMS_ApplicationAssignment["EnforcementDeadline"].DateTimeValue = DateTime.Now.ToUniversalTime();
                oSMS_ApplicationAssignment["EvaluationSchedule"].StringValue = null;
                oSMS_ApplicationAssignment["ExpirationTime"].StringValue = null;
                oSMS_ApplicationAssignment["LastModificationTime"].DateTimeValue = DateTime.Now;
                oSMS_ApplicationAssignment["LastModifiedBy"].StringValue = Environment.UserName;
                oSMS_ApplicationAssignment["LocaleID"].StringValue = "1033";
                oSMS_ApplicationAssignment["LogComplianceToWinEvent"].BooleanValue = false;
                oSMS_ApplicationAssignment["NonComplianceCriticality"].StringValue = null;
                oSMS_ApplicationAssignment["NotifyUser"].BooleanValue = true;
                oSMS_ApplicationAssignment["OfferFlags"].IntegerValue = 0;
                oSMS_ApplicationAssignment["OfferTypeID"].IntegerValue = 0;
                oSMS_ApplicationAssignment["OverrideServiceWindows"].BooleanValue = false;
                oSMS_ApplicationAssignment["Priority"].IntegerValue = 1;
                oSMS_ApplicationAssignment["RaiseMomAlertsOnFailure"].BooleanValue = false;
                oSMS_ApplicationAssignment["RebootOutsideOfServiceWindows"].BooleanValue = false;
                oSMS_ApplicationAssignment["RequireApproval"].BooleanValue = false;
                oSMS_ApplicationAssignment["SendDetailedNonComplianceStatus"].BooleanValue = false;
                oSMS_ApplicationAssignment["StartTime"].DateTimeValue = DateTime.Now.ToUniversalTime();
                oSMS_ApplicationAssignment["StateMessagePriority"].IntegerValue = 5;
                oSMS_ApplicationAssignment["SuppressReboot"].IntegerValue = 0;
                oSMS_ApplicationAssignment["TargetCollectionID"].StringValue = targetCollectionId;
                oSMS_ApplicationAssignment["UpdateDeadline"].StringValue = null;
                oSMS_ApplicationAssignment["UpdateSupersedence"].BooleanValue = false;
                oSMS_ApplicationAssignment["UseGMTTimes"].BooleanValue = true;
                oSMS_ApplicationAssignment["UserUIExperience"].BooleanValue = true;
                oSMS_ApplicationAssignment["WoLEnabled"].BooleanValue = false;

                oSMS_ApplicationAssignment.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static SCCMSystem[] GetAllDevices()
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = "select * from SMS_R_System";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMSystem> resources = new List<SCCMSystem>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMSystem resource = new SCCMSystem(queryResult);

                    resources.Add(resource);
                }

                return resources.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMSystem GetDeviceDetailsByDeviceName(string DeviceName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = string.Format("select * from SMS_R_System where Name = '{0}'", DeviceName);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMSystem> resources = new List<SCCMSystem>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMSystem resource = new SCCMSystem(queryResult);

                    return resource;
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMSystem GetDeviceDetails(UInt32 resourceId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = string.Format("select * from SMS_R_System where ResourceID = '{0}'", resourceId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMSystem resource = new SCCMSystem(queryResult);

                    return resource;
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static void EditDevice(Int32 resourceId, string[] updatedMACAddress)
        {
            try
            {
                WqlConnectionManager connection = GetWqlConnection();
                String query = string.Format("select * from SMS_R_System where ResourceID = '{0}'", resourceId);
                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject queryResult in queryResults)
                {
                    queryResult["MACAddresses"].StringValue = updatedMACAddress[0];
                    queryResult.Put();
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static UInt32 CreateDevice(string netBiosName, string macAddress)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string smBiosGuid = Guid.NewGuid().ToString();

                if (netBiosName.Trim().Length == 0 || macAddress.Trim().Length == 0)
                {
                    throw new ArgumentNullException("Computer Name and Mac Address must be defined");
                }

                // Reformat macAddress to : separator.
                if (string.IsNullOrEmpty(macAddress) == false)
                {
                    macAddress = macAddress.Replace("-", ":");
                }

                // Create the computer.
                Dictionary<string, object> inParams = new Dictionary<string, object>();
                inParams.Add("NetbiosName", netBiosName);
                inParams.Add("SMBIOSGUID", smBiosGuid);
                inParams.Add("MACAddress", macAddress);
                inParams.Add("OverwriteExistingRecord", false);

                IResultObject outParams = connection.ExecuteMethod(
                    "SMS_Site",
                    "ImportMachineEntry",
                    inParams);

                connection.Close();
                connection.Dispose();

                return Convert.ToUInt32(outParams.PropertyList["ResourceID"]);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return 0;
        }

        public static void UpdateDevice(string netBiosName, string macAddress)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string smBiosGuid = Guid.NewGuid().ToString();

                if (netBiosName.Trim().Length == 0 || macAddress.Trim().Length == 0)
                {
                    throw new ArgumentNullException("Computer Name and Mac Address must be defined");
                }

                // Reformat macAddress to : separator.
                if (string.IsNullOrEmpty(macAddress) == false)
                {
                    macAddress = macAddress.Replace("-", ":");
                }

                // Create the computer.
                Dictionary<string, object> inParams = new Dictionary<string, object>();
                inParams.Add("NetbiosName", netBiosName);
                inParams.Add("MACAddress", macAddress);
                inParams.Add("OverwriteExistingRecord", true);

                IResultObject outParams = connection.ExecuteMethod(
                    "SMS_Site",
                    "ImportMachineEntry",
                    inParams);

                connection.Close();
                connection.Dispose();
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static SCCMVariable[] GetCustomVariablesForCollection(string collectionId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                #region may be able to break this out into a method so that get and set collection vars can share this code
                IResultObject collectionSettings = connection.CreateInstance("SMS_CollectionSettings");
                collectionSettings["CollectionID"].StringValue = collectionId;
                collectionSettings.Put();

                collectionSettings.Get();

                List<IResultObject> collectionVariables = collectionSettings.GetArrayItems("CollectionVariables");

                collectionSettings.Get();
                #endregion

                List<SCCMVariable> variables = new List<SCCMVariable>();

                foreach (IResultObject item in collectionVariables)
                {
                    if (item != null)
                    {
                        SCCMVariable variable = new SCCMVariable(item);

                        variables.Add(variable);
                    }
                }

                return variables.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMVariable[] GetCustomVariablesForDevice(UInt32 resourceId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String queryGetCollectionMemberByName = "select SiteCode from SMS_FullCollectionMembership  WHERE ResourceId = '" + resourceId + "'";

                IResultObject queryResults1 = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                String SourceSite = "";

                foreach (IResultObject queryResult1 in queryResults1)
                    SourceSite = queryResult1["SiteCode"].StringValue.ToString();

                IResultObject computerSettings = connection.CreateInstance("SMS_MachineSettings");
                computerSettings["ResourceID"].IntegerValue = Convert.ToInt32(resourceId);
                computerSettings["SourceSite"].StringValue = SourceSite;
                computerSettings["LocaleID"].IntegerValue = 1033; // The default locale ID is 1033, English (United States).

                computerSettings.Put();

                computerSettings.Get();

                List<IResultObject> collectionVariables = computerSettings.GetArrayItems("MachineVariables");

                computerSettings.Get();

                List<SCCMVariable> variables = new List<SCCMVariable>();

                foreach (IResultObject item in collectionVariables)
                {
                    if (item != null)
                    {
                        SCCMVariable variable = new SCCMVariable(item);

                        variables.Add(variable);
                    }
                }

                return variables.ToArray();
            }
            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static void SetCustomVariableForCollection(string collectionId, SCCMVariable variable)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                IResultObject collectionSettings = connection.CreateInstance("SMS_CollectionSettings");
                collectionSettings["CollectionID"].StringValue = collectionId;
                collectionSettings.Put();

                collectionSettings.Get();

                List<IResultObject> collectionVariables = collectionSettings.GetArrayItems("CollectionVariables");

                IResultObject collectionVariable = connection.CreateEmbeddedObjectInstance("SMS_CollectionVariable");
                collectionVariable["Name"].StringValue = variable.Name;
                collectionVariable["Value"].StringValue = variable.Value;
                collectionVariable["IsMasked"].BooleanValue = variable.IsMasked;


                collectionVariables.Add(collectionVariable);
                collectionSettings.SetArrayItems("CollectionVariables", collectionVariables);

                collectionSettings.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void SetCustomVariableForDevice(UInt32 resourceId, SCCMVariable variable)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                #region may be able to put this in a method so that get and set device variable methods can share this code
                //Commenting out because this method does not work when run against a newly created device. 
                //It takes some time for the device membership to update, so we need to get the site code directly from the device.
                //
                ///String queryGetCollectionMemberByName = "select SiteCode from SMS_FullCollectionMembership  WHERE ResourceId = '" + resourceId + "'";

                //IResultObject queryResults1 = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                //String SourceSite = "";

                //foreach (IResultObject queryResult1 in queryResults1)
                //    SourceSite = queryResult1["SiteCode"].StringValue.ToString();
                //

                //Creating empty source site. Will try setting value just below.
                string SourceSite = "";

                try
                {
                    SCCMSystem deviceDetails = GetDeviceDetails(resourceId);
                    SourceSite = deviceDetails.SMSAssignedSites[0];
                }
                catch
                {
                    throw new ArgumentNullException("Unable to find site code for this device. Without site code variables cannot be added.");
                }

                IResultObject computerSettings = connection.CreateInstance("SMS_MachineSettings");
                computerSettings["ResourceID"].IntegerValue = Convert.ToInt32(resourceId);
                computerSettings["SourceSite"].StringValue = SourceSite;
                computerSettings["LocaleID"].IntegerValue = 1033; // The default locale ID is 1033, English (United States).

                computerSettings.Put();
                computerSettings.Get();

                List<IResultObject> collectionVariables = computerSettings.GetArrayItems("MachineVariables");

                computerSettings.Get();
                #endregion

                IResultObject collectionVariable = connection.CreateEmbeddedObjectInstance("SMS_MachineVariable");
                collectionVariable["Name"].StringValue = variable.Name;
                collectionVariable["Value"].StringValue = variable.Value;
                collectionVariable["IsMasked"].BooleanValue = variable.IsMasked;

                collectionVariables.Add(collectionVariable);

                computerSettings.SetArrayItems("MachineVariables", collectionVariables);

                computerSettings.Put();

                connection.Close();
                connection.Dispose();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void DeleteCustomVariableFromDevice(UInt32 resourceId, string variableName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            string query = string.Format("select * from SMS_MachineSettings where ResourceID = '{0}'", resourceId);

            IResultObject queryResult = connection.QueryProcessor.ExecuteQuery(query);

            //Create null instance of computerSettings. We will populate this later when iterating query result
            IResultObject computerSettings = null;

            //Extract one instance of SMS_MachineSettings out of the query result
            foreach (IResultObject cs in queryResult)
            {
                computerSettings = cs;
                break;
            }

            //Get this instance of MachineSettings
            computerSettings.Get();

            //Get MachineVariables from this instance of machine settings
            List<IResultObject> machineVariables = computerSettings.GetArrayItems("MachineVariables");

            //Iterate machine variables and remove variable if we find a match with the one to remove
            foreach (IResultObject machVar in machineVariables)
            {
                if (machVar["Name"].StringValue == variableName)
                {
                    //Since we found a match remove this variable from the collection of machine variables
                    machineVariables.Remove(machVar);

                    //Update this instance of SMS_MachineSettings with the new list of variables
                    computerSettings.SetArrayItems("MachineVariables", machineVariables);
                    computerSettings.Put();

                    break;
                }
            }




            //
            ////List<IResultObject> machineVariables = new List<IResultObject>();
            //foreach (IResultObject oMS in computerSettings)
            //{
            //    machineVariables = oMS.GetArrayItems("MachineVariables");
            //}

            ////machineVariables.Get();

            //foreach (IResultObject mV in machineVariables)
            //{
            //    mV.Get();
            //    if (mV["Name"].StringValue == variableName)
            //    {
            //        mV.Delete();
            //        break;
            //    }
            //}
            //
        }

        #region Boundary Steps
        private static IResultObject GetBoundaryObjectById(UInt32 boundaryId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = string.Format("SELECT * FROM SMS_Boundary WHERE BoundaryID = {0}", boundaryId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject qR in queryResults)
                {
                    return qR;
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        private static IResultObject GetBoundaryGroupObjectById(UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = string.Format("SELECT * FROM SMS_BoundaryGroup WHERE GroupID = {0}", boundaryGroupId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject qR in queryResults)
                {
                    return qR;
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMBoundary[] GetAllBoundaries()
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "SELECT * FROM SMS_Boundary";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundary> boundaries = new List<SCCMBoundary>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundary boundary = new SCCMBoundary(queryResult);

                    boundaries.Add(boundary);
                }

                return boundaries.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMBoundary GetBoundaryByID(UInt32 boundaryId)
        {
            SCCMBoundary boundary = new SCCMBoundary(GetBoundaryObjectById(boundaryId));

            return boundary;
        }

        public static SCCMBoundary[] GetBoundariesByBoundaryGroupId(UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = string.Format("SELECT * FROM SMS_BoundaryGroupMembers WHERE GroupID = {0}", boundaryGroupId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundary> boundaries = new List<SCCMBoundary>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundary boundary = GetBoundaryByID(Convert.ToUInt32(queryResult["BoundaryID"].IntegerValue));

                    boundaries.Add(boundary);
                }

                return boundaries.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static void AddBoundariesToBoundaryGroup(UInt32[] boundaryIds, UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                methodParams.Add("BoundaryID", boundaryIds);

                IResultObject oBounaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

                oBounaryGroup.ExecuteMethod("AddBoundary", methodParams);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void RemoveBoundariesFromBoundaryGroup(UInt32[] boundaryIds, UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                methodParams.Add("BoundaryID", boundaryIds);

                IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

                oBoundaryGroup.ExecuteMethod("RemoveBoundary", methodParams);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void AddSiteServerToBoundaryGroup(string serverName, string siteCode, bool isSlowConnection, UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string serverNALPath = GetServerNalPath(serverName, siteCode);

                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                methodParams.Add("ServerNALPath", new string[1] { serverNALPath });

                //Set up connection speed flags
                //0 = Fast connection speed, 1 = Slow connection speed
                int connectionSpeedFlag = 0;
                if (isSlowConnection == true)
                {
                    connectionSpeedFlag = 1;
                }

                methodParams.Add("Flags", new int[1] { connectionSpeedFlag });

                IResultObject oBounaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

                oBounaryGroup.ExecuteMethod("AddSiteSystem", methodParams);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void RemoveSiteServerFromBoundaryGroup(string serverName, string siteCode, UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string serverNALPath = GetServerNalPath(serverName, siteCode);

                Dictionary<string, object> methodParams = new Dictionary<string, object>();
                methodParams.Add("ServerNALPath", new string[1] { serverNALPath });

                IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

                oBoundaryGroup.ExecuteMethod("RemoveSiteSystem", methodParams);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static UInt32 CreateBoundary(string boundaryValue, string boundaryDisplayName, string boundaryType)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                int type = GetBoundaryTypeIntFromTypeName(boundaryType);

                IResultObject oBoundary = connection.CreateInstance("SMS_Boundary");
                oBoundary["DisplayName"].StringValue = boundaryDisplayName;
                oBoundary["BoundaryType"].IntegerValue = type;
                oBoundary["Value"].StringValue = boundaryValue;
                oBoundary.Put();
                oBoundary.Get();

                return Convert.ToUInt32(oBoundary["BoundaryID"].IntegerValue);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return 0;
        }

        public static void EditBoundaryByID(UInt32 boundaryId, string updatedBoundaryValue, string updatedBoundaryDisplayName, string updatedBoundaryType)
        {

            IResultObject oBoundary = GetBoundaryObjectById(boundaryId);

            try
            {
                oBoundary["DisplayName"].StringValue = updatedBoundaryDisplayName;
                oBoundary["Value"].StringValue = updatedBoundaryValue;
                oBoundary["BoundaryType"].StringValue = GetBoundaryTypeIntFromTypeName(updatedBoundaryType).ToString();

                oBoundary.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void DeleteBoundary(UInt32 boundaryId)
        {
            IResultObject oBoundary = GetBoundaryObjectById(boundaryId);

            try
            {
                oBoundary.Delete();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static SCCMBoundaryGroup[] GetAllBoundaryGroups()
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = "SELECT * FROM SMS_BoundaryGroup";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundaryGroup> boundaryGroups = new List<SCCMBoundaryGroup>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundaryGroup boundaryGroup = new SCCMBoundaryGroup(queryResult);

                    boundaryGroups.Add(boundaryGroup);
                }

                return boundaryGroups.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static SCCMBoundaryGroup GetBoundaryGroupByID(UInt32 boundaryGroupId)
        {
            SCCMBoundaryGroup boundaryGroup = new SCCMBoundaryGroup(GetBoundaryGroupObjectById(boundaryGroupId));

            return boundaryGroup;
        }

        public static SCCMBoundaryGroup[] GetBoundaryGroupsByBoundaryID(UInt32 boundaryId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = string.Format("SELECT * FROM SMS_BoundaryGroupMembers WHERE BoundaryID = {0}", boundaryId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMBoundaryGroup> boundaryGroups = new List<SCCMBoundaryGroup>();

                foreach (IResultObject queryResult in queryResults)
                {
                    SCCMBoundaryGroup boundaryGroup = GetBoundaryGroupByID(Convert.ToUInt32(queryResult["GroupID"].IntegerValue));

                    boundaryGroups.Add(boundaryGroup);
                }

                return boundaryGroups.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        public static UInt32 CreateBoundaryGroup(string groupName, string groupDescription)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                IResultObject gBoundary = connection.CreateInstance("SMS_BoundaryGroup");
                gBoundary["Name"].StringValue = groupName;
                gBoundary["Description"].StringValue = groupDescription;
                gBoundary["DefaultSiteCode"].StringValue = "";
                gBoundary.Put();
                gBoundary.Get();

                return Convert.ToUInt32(gBoundary["GroupID"].IntegerValue);
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return 0;
        }

        public static void SetAssignedSiteOnBoundaryGroup(UInt32 boundaryGroupId, string siteCode)
        {
            WqlConnectionManager connection = GetWqlConnection();

            IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

            try
            {
                oBoundaryGroup["DefaultSiteCode"].StringValue = siteCode;
                oBoundaryGroup.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void RemoveAssignedSiteFromBoundaryGroup(UInt32 boundaryGroupId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

            try
            {
                oBoundaryGroup["DefaultSiteCode"].StringValue = "";
                oBoundaryGroup.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }

        public static void EditBoundaryGroupByID(UInt32 boundaryGroupId, string updatedGroupName)
        {
            IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

            try
            {
                oBoundaryGroup["Name"].StringValue = updatedGroupName;

                oBoundaryGroup.Put();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);

            }
        }

        public static void DeleteBoundaryGroup(UInt32 boundaryGroupId)
        {
            IResultObject oBoundaryGroup = GetBoundaryGroupObjectById(boundaryGroupId);

            try
            {
                oBoundaryGroup.Delete();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }
        }
      */
 
        public static string[] GetAvailableBounaryTypes()
        {
            string[] SCCMBoundaryTypes = new string[4];
            SCCMBoundaryTypes[0] = "IP Subnet";
            SCCMBoundaryTypes[1] = "Active Directory Site";
            SCCMBoundaryTypes[2] = "IPv6";
            SCCMBoundaryTypes[3] = "Ip Address Range";

            return SCCMBoundaryTypes;
        }

        public static string GetBoundaryTypeNameFromTypeInt(int SCCMTypeInt)
        {
            string[] SCCMBoundaryTypes = GetAvailableBounaryTypes();

            return SCCMBoundaryTypes[SCCMTypeInt];
        }

        public static int GetAssignedBoundaryGroupCountBySiteCode(string defaultSiteCode)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = string.Format("SELECT DefaultSiteCode FROM SMS_BoundaryGroup WHERE DefaultSiteCode = '{0}'", defaultSiteCode);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                int countOfAssignedSites = 0;

                foreach (IResultObject qR in queryResults)
                {
                    countOfAssignedSites = countOfAssignedSites + 1;
                }

                return countOfAssignedSites;
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return 0;
        }
        
        /*
        public static int GetBoundaryTypeIntFromTypeName(string SCCMTypeName)
        {
            string[] SCCMBoundaryTypes = GetAvailableBounaryTypes();

            return Array.IndexOf(SCCMBoundaryTypes, SCCMTypeName);
        }

        #endregion

        public static SCCMSite[] GetAllSites()
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = "SELECT * FROM SMS_Site";

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                List<SCCMSite> sites = new List<SCCMSite>();

                foreach (IResultObject result in queryResults)
                {
                    SCCMSite site = new SCCMSite(result);
                    sites.Add(site);
                }

                return sites.ToArray();
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;
        }

        private static string GetServerNalPath(string serverName, string siteCode)
        {
            //The follow site specifies building ServerNALPath as shown below: http://cm12sdk.net/?p=511
            //$NALPath = "[""Display=\\" +  $DistributionPoint + "." + $DomainName + "\""]MSWNET:[""SMS_SITE=" + $SiteCode + """]\\" + $DistributionPoint + "." + $DomainName + "\"

            //But we don't have distribution point or domain name so we are building it with serverName and siteCode
            string serverNALPath = string.Format(@"[""Display=\\{0}\""]MSWNET:[""SMS_SITE={1}""]\\{0}\", serverName, siteCode);

            return serverNALPath;
        }

        private static IResultObject GetCollectionObjectById(string collectionId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = string.Format("select * from SMS_Collection where CollectionID = '{0}'", collectionId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject oCollection in queryResults)
                {
                    return oCollection;
                }
            }

            catch (Exception ex)
            {
                HandleErrors(ex);
            }

            return null;

        }
*/
        private static void HandleErrors(Exception ex)
        {
            if (ex is SmsConnectionException)
            {
                SmsConnectionException scEx = (SmsConnectionException)ex;

                throw new Exception(string.Format("SmsConnectionException Details {0}", scEx.Details), scEx);
            }

            if (ex is SmsQueryException)
            {
                SmsQueryException sqEx = (SmsQueryException)ex;

                throw new Exception(string.Format("SmsQueryException Details {0}", sqEx.Details), sqEx);
            }

            else
            {
                throw ex;
            }
        }
    }
}