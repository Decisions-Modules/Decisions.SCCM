using System;
using DecisionsFramework.Design.Flow.CoreSteps;
using DecisionsFramework;
using Microsoft.ConfigurationManagement.ManagementProvider.WqlQueryEngine;
using Microsoft.ConfigurationManagement.ManagementProvider;
using System.Net;
using System.Collections.Generic;
using DecisionsFramework.Data.DataTypes;

namespace SCCM_2012
{
    public abstract class SCCMBaseStep : BaseFlowAwareStep
    {
        const string WQLCACHEID = "wqlCacheId";

        private static TimeBoundCache<String, WqlConnectionManager> wqlConnectionCache = new TimeBoundCache<string, WqlConnectionManager>(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));

        protected WqlConnectionManager GetWqlConnection() //SQL for WQLConnection Manager is documented at http://msdn.microsoft.com/en-us/library/windows/desktop/aa394606(v=vs.85).aspx
        {
            WqlConnectionManager connection = wqlConnectionCache[WQLCACHEID];
            //WqlConnectionManager connection = null;

            if (connection == null)
            {
                SCCMConnectionInfo settings = SCCMConnectionInfo.GetSettings();

                connection = ConnectToSCCM(settings.SCCMServerAddress, settings.SCCMServerUsername, settings.SCCMServerUserPassword);

                wqlConnectionCache[WQLCACHEID] = connection;
            }

            return connection;
        }

        protected WqlConnectionManager ConnectToSCCM(string serverName, string userName, string password)
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
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject CreateCollectionBaseMethod(string collectionName, string collectionComment, UInt32 collectionType, string limitingCollectionIdOrName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                IResultObject oExistingCollection = GetCollectionObjectByName(collectionName);
                if (oExistingCollection == null)
                {
                    string limitingCollectionId;

                    //IResultObject oLimitingCollection = GetCollectionObjectById(limitToCollectionIdOrName);
                    if (GetCollectionObjectById(limitingCollectionIdOrName) == null)
                    {
                        IResultObject oLimitingCollection = GetCollectionObjectByName(limitingCollectionIdOrName);
                        if (oLimitingCollection == null)
                        {
                            throw new Exception(string.Format("Cannot add collection '{0}' because the limiting collection specified cannot be found by name or id equaling '{1}'. Please specify a valid limiting collection.", collectionName, limitingCollectionIdOrName));
                        }
                        else
                        {
                            SCCMCollection limitingCollection = new SCCMCollection(oLimitingCollection);
                            limitingCollectionId = limitingCollection.CollectionId;
                        }
                    }
                    else
                    {
                        limitingCollectionId = limitingCollectionIdOrName;
                    }

                    //The SMS_Collection object is documented at: http://msdn.microsoft.com/en-us/library/hh948939.aspx
                    IResultObject oCollection = connection.CreateInstance("SMS_Collection");
                    oCollection["Name"].StringValue = collectionName;
                    oCollection["OwnedByThisSite"].BooleanValue = true;
                    oCollection["Comment"].StringValue = collectionComment;
                    oCollection["CollectionType"].IntegerValue = Convert.ToInt32(collectionType); //Type 0 = Other, Type 1 = User, Type 2 = Device
                    oCollection["LimitToCollectionID"].StringValue = limitingCollectionId; //SMS00001 is the base all devices collection id
                    oCollection["RefreshType"].IntegerValue = 6; //1 = MANUAL, 2 = PERIODIC, 4 = CONSTANT_UPDATE, 6 = Incremental Updates
                    oCollection.Put();
                    oCollection.Get();

                    return oCollection;
                }
                else
                {
                    SCCMCollection existingCollection = new SCCMCollection(oExistingCollection);
                    throw new Exception(string.Format("Cannot add collection '{0}'. Collection with id of {1} already exists with that same name.", collectionName, existingCollection.CollectionId));
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }

        }

        protected IResultObject GetCollectionObjectById(string collectionId)
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

                return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetAdvertisementObjectById(string advertisementId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = string.Format("select * from SMS_Advertisement where AdvertisementID = '{0}'", advertisementId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject oAdvert in queryResults)
                {
                    return oAdvert;
                }

                return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetCollectionObjectByName(string collectionName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                                string queryFromWhere = string.Format("FROM SMS_Collection WHERE Name = '{0}'", collectionName);
                string querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
                string querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

                if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
                {
                    throw new Exception(string.Format("More than one collection was found by collection name {0}", collectionName));
                }

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(querySelectStar);
                //string query = string.Format("select * from SMS_Collection where CollectionID = '{0}'", collectionId);

                //IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject oCollection in queryResults)
                {
                    return oCollection;
                }

                return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected SCCMSystem[] GetSCCMSystemsByCollectionIdBaseMethod(string collectionId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String queryGetCollectionMemberByName = string.Format("select ResourceID, Name from SMS_FullCollectionMembership  WHERE CollectionID = '{0}'", collectionId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(queryGetCollectionMemberByName);

                List<SCCMSystem> systems = new List<SCCMSystem>();

                foreach (IResultObject queryResult in queryResults)
                {
                    UInt32 resouceId = Convert.ToUInt32(queryResult["ResourceID"].IntegerValue);

                    IResultObject oSystem = GetDeviceObjectById(resouceId);

                    if (oSystem != null)
                    {
                        SCCMSystem system = new SCCMSystem(oSystem);
                        systems.Add(system);
                    }
                }

                return systems.ToArray();
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetApplicationObjectById(UInt32 applicationId, ApplicationIdType applicationIdField)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = "";

                switch (applicationIdField)
                {
                    case ApplicationIdType.CI_ID:
                        query = "SELECT * FROM SMS_Application WHERE CI_ID = '" + applicationId + "'";
                        break;
                    case ApplicationIdType.ModelID:
                        query = string.Format("SELECT * FROM SMS_Application WHERE ModelId = '{0}' AND IsLatest = 1", applicationId);
                        break;
                    default:
                        throw new Exception(string.Format("Getting application by id field of {0} is not handled at this time", Enum.GetName(typeof(ApplicationIdType), applicationIdField)));
                }                
                
                /*
                if (applicationIdField == applicationIdType.CI_ID)
                {
                    query = "SELECT * FROM SMS_Application WHERE CI_ID = '" + applicationId + "'";
                }

                if (applicationIdField == applicationIdType.ModelID)
                {
                    query = string.Format("SELECT * FROM SMS_Application WHERE ModelId = '{0}' AND IsLatest = 1", applicationId);
                }

                else
                {
                    throw new Exception(string.Format("Getting application by id field of {0} is not handled at this time", Enum.GetName(typeof(applicationIdType), applicationIdField)));
                }
                 */

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject aApplication in queryResults)
                {
                    return aApplication;
                }
                return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetDeviceObjectById(UInt32 resourceId)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                string query = string.Format("select * from SMS_R_System where ResourceID = '{0}'", resourceId);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject oDevice in queryResults)
                {
                    return oDevice;
                }
                return null;
            }
            
            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetBoundaryObjectById(UInt32 boundaryId)
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
            return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetBoundaryObjectByName(string boundaryName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String query = string.Format("SELECT * FROM SMS_Boundary WHERE DisplayName = '{0}'", boundaryName);

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(query);

                foreach (IResultObject qR in queryResults)
                {
                    return qR;
                }
                return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetBoundaryGroupObjectById(UInt32 boundaryGroupId)
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
            return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected IResultObject GetBoundaryGroupObjectByName(string boundaryGroupName)
        {
            WqlConnectionManager connection = GetWqlConnection();

            try
            {
                String queryFromWhere = string.Format("FROM SMS_BoundaryGroup WHERE Name = '{0}'", boundaryGroupName);
                String querySelectStar = string.Format("SELECT * {0}", queryFromWhere);
                String querySelectCount = string.Format("SELECT COUNT(*) {0}", queryFromWhere);

                if (GetCountOfSelectCountFromWmiClassQuery(querySelectCount) > 1)
                {
                    throw new Exception(string.Format("More than one boundary group was found by name {0}", boundaryGroupName));
                }

                IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(querySelectStar);

                foreach (IResultObject qR in queryResults)
                {
                    return qR;
                }
                return null;
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message, HandleErrors(ex));
            }
        }

        protected int GetCountOfSelectCountFromWmiClassQuery(string selectCountQuery)
        {
            WqlConnectionManager connection = GetWqlConnection();

            IResultObject queryResults = connection.QueryProcessor.ExecuteQuery(selectCountQuery);

            foreach (IResultObject qR in queryResults)
            {
                return qR["Count"].IntegerValue;

            }
            return 0;
        }

        protected string GetServerNalPath(string serverName, string siteCode)
        {
            //The follow site specifies building ServerNALPath as shown below: http://cm12sdk.net/?p=511
            //$NALPath = "[""Display=\\" +  $DistributionPoint + "." + $DomainName + "\""]MSWNET:[""SMS_SITE=" + $SiteCode + """]\\" + $DistributionPoint + "." + $DomainName + "\"

            //But we don't have distribution point or domain name so we are building it with serverName and siteCode
            string serverNALPath = string.Format(@"[""Display=\\{0}\""]MSWNET:[""SMS_SITE={1}""]\\{0}\", serverName, siteCode);

            return serverNALPath;
        }

        protected string[] GetAvailableBounaryTypes()
        {
            string[] SCCMBoundaryTypes = new string[4];
            SCCMBoundaryTypes[0] = "IP Subnet";
            SCCMBoundaryTypes[1] = "Active Directory Site";
            SCCMBoundaryTypes[2] = "IPv6";
            SCCMBoundaryTypes[3] = "Ip Address Range";

            return SCCMBoundaryTypes;
        }

        protected string GetBoundaryTypeNameFromTypeInt(int SCCMTypeInt)
        {
            string[] SCCMBoundaryTypes = GetAvailableBounaryTypes();

            return SCCMBoundaryTypes[SCCMTypeInt];
        }

        protected int GetBoundaryTypeIntFromTypeName(string SCCMTypeName)
        {
            string[] SCCMBoundaryTypes = GetAvailableBounaryTypes();

            return Array.IndexOf(SCCMBoundaryTypes, SCCMTypeName);
        }

        protected UInt32 GetAdvertFlagsIntValueFromTaskSeqFlags(SCCMTaskSequenceFlags options)
        {
            UInt32 intValueAdvertFlags = 0;

            if (options.adv_IMMEDIATE)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00000020;
            }
            if (options.adv_ONSYSTEMSTARTUP)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00000100;
            }
            if (options.adv_ONUSERLOGON)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00000200;
            }
            if (options.adv_ONUSERLOGOFF)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00000400;
            }
            if (options.adv_WINDOWS_CE)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00008000;
            }
            if (!options.adv_DONOT_FALLBACK)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00020000;
            }
            if (options.adv_ENABLE_TS_FROM_CD_AND_PXE)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00040000;
            }
            if (!options.adv_ALLOW_INTERNET_CLIENTS)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00080000;
            }
            if (options.adv_OVERRIDE_SERVICE_WINDOWS)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00100000;
            }
            if (options.adv_REBOOT_OUTSIDE_OF_SERVICE_WINDOWS)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00200000;
            }
            if (options.adv_WAKE_ON_LAN_ENABLED)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00400000;
            }
            if (options.adv_SHOW_PROGRESS)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x00800000;
            }
            if (!options.adv_NO_DISPLAY)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x02000000;
            }
            if (options.adv_ONSLOWNET)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x04000000;
            }
            if (options.adv_ENABLE_TS_ONLY_MEDIA_AND_PXE)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x10000000;
            }
            if (options.adv_ENABLE_TS_ONLY_MEDIA_AND_PXE_HIDDEN)
            {
                intValueAdvertFlags = intValueAdvertFlags | 0x30000000;
            }

            return intValueAdvertFlags;
        }

        protected UInt32 GetRemoteClientFlagsIntValueFromTaskSeqFlags(SCCMTaskSequenceFlags options)
        {
            UInt32 intValueRemoteClientFlags = 0;

            if (options.rem_RUN_FROM_LOCAL_DISPPOINT)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000008;
            }
            if (options.rem_DOWNLOAD_FROM_LOCAL_DISPPOINT)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000010;
            }
            if (options.rem_DONT_RUN_NO_LOCAL_DISPPOINT)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000020;
            }
            if (options.rem_DOWNLOAD_FROM_REMOTE_DISPPOINT)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000040;
            }
            if (options.rem_RUN_FROM_REMOTE_DISPPOINT)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000080;
            }
            if (options.rem_DOWNLOAD_ON_DEMAND_FROM_LOCAL_DP)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000100;
            }
            if (options.rem_DOWNLOAD_ON_DEMAND_FROM_REMOTE_DP)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000200;
            }
            if (options.rem_BALLOON_REMINDERS_REQUIRED)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000400;
            }
            if (options.rem_RERUN_ALWAYS)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00000800;
            }
            if (options.rem_RERUN_NEVER)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00001000;
            }
            if (options.rem_RERUN_IF_FAILED)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00002000;
            }
            if (options.rem_RERUN_IF_SUCCEEDED)
            {
                intValueRemoteClientFlags = intValueRemoteClientFlags | 0x00004000;
            }

            return intValueRemoteClientFlags;
        }

        internal static Exception HandleErrors(Exception ex)
        {
            if (ex is SmsConnectionException)
            {
                SmsConnectionException scEx = (SmsConnectionException)ex;

                return new Exception(string.Format("SmsConnectionException Details {0}", scEx.Details), scEx);
            }

            if (ex is SmsQueryException)
            {
                SmsQueryException sqEx = (SmsQueryException)ex;

                return new Exception(string.Format("SmsQueryException Details {0}", sqEx.Details), sqEx);

            }

            else
            {
                return ex;
            }
        }
    }
}
