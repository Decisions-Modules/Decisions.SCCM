using DecisionsFramework.Design.Properties;
using Microsoft.ConfigurationManagement.ManagementProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SCCM_2012
{
    [DataContract]
    public class SCCMCollection
    {
        [DataMember]
        public string CollectionName { get; set; }
        [DataMember]
        public string CollectionId { get; set; }

        public SCCMCollection() { }

        public SCCMCollection(IResultObject queryResult)
        {
            if (queryResult["Name"] != null && queryResult["Name"].ObjectValue != null)
            {
                CollectionName = queryResult["Name"].StringValue;
            }
            else { CollectionName = ""; }

            if (queryResult["CollectionID"] != null && queryResult["CollectionID"].ObjectValue != null)
            {
                CollectionId = queryResult["CollectionID"].StringValue;
            }
            else
            {
                throw new Exception("SMS_R_System object is invalid. Unable to get CollectionID from IResultObject");
            }
        }
    }

    [DataContract]
    public class SCCMSystem //SMS_R_System documentation: http://msdn.microsoft.com/en-us/library/hh949186.aspx
    {
        [DataMember]
        public DateTime CreationDate { get; set; }
        [DataMember]
        public string[] IPAddresses { get; set; }
        [DataMember]
        public string[] IPSubnets { get; set; }
        [DataMember]
        public string[] IPv6Addresses { get; set; }
        [DataMember]
        public string[] IPv6Prefixes { get; set; }
        [DataMember]
        public bool IsVirtualMachine { get; set; }
        [DataMember]
        public string[] MACAddresses { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string NetbiosName { get; set; }
        [DataMember]
        public string OperatingSystemNameandVersion { get; set; }
        [DataMember]
        public string ResourceDomainORWorkgroup { get; set; }
        [DataMember]
        public UInt32 ResourceId { get; set; }
        [DataMember]
        public string SID { get; set; }
        [DataMember]
        public string SMBIOSGUID { get; set; }
        [DataMember]
        public string[] SMSAssignedSites { get; set; }

        public SCCMSystem() { }

        public SCCMSystem(IResultObject queryResult)
        {
            if (queryResult["CreationDate"] != null && queryResult["CreationDate"].ObjectValue != null)
            {
                CreationDate = queryResult["CreationDate"].DateTimeValue;
            }
            else { CreationDate = DateTime.MinValue; }

            if (queryResult["IPAddresses"] != null && queryResult["IPAddresses"].ObjectValue != null)
            {
                IPAddresses = queryResult["IPAddresses"].StringArrayValue;
            }
            else { IPAddresses = null; }

            if (queryResult["IPSubnets"] != null && queryResult["IPSubnets"].ObjectValue != null)
            {
                IPSubnets = queryResult["IPSubnets"].StringArrayValue;
            }
            else { IPSubnets = null; }

            if (queryResult["IPv6Addresses"] != null && queryResult["IPv6Addresses"].ObjectValue != null)
            {
                IPv6Addresses = queryResult["IPv6Addresses"].StringArrayValue;
            }
            else { IPv6Addresses = null; }

            if (queryResult["IPv6Prefixes"] != null && queryResult["IPv6Prefixes"].ObjectValue != null)
            {
                IPv6Prefixes = queryResult["IPv6Prefixes"].StringArrayValue;
            }
            else { IPv6Prefixes = null; }

            if (queryResult["IsVirtualMachine"] != null && queryResult["IsVirtualMachine"].ObjectValue != null)
            {
                IsVirtualMachine = queryResult["IsVirtualMachine"].BooleanValue;
            }
            else { IsVirtualMachine = false; }

            if (queryResult["MACAddresses"] != null && queryResult["MACAddresses"].ObjectValue != null)
            {
                MACAddresses = queryResult["MACAddresses"].StringArrayValue;
            }
            else { MACAddresses = null; }

            if (queryResult["Name"] != null && queryResult["Name"].ObjectValue != null)
            {
                Name = queryResult["Name"].StringValue;
            }
            else { Name = null; }

            if (queryResult["NetbiosName"] != null && queryResult["NetbiosName"].ObjectValue != null)
            {
                NetbiosName = queryResult["NetbiosName"].StringValue;
            }
            else { NetbiosName = null; }

            if (queryResult["OperatingSystemNameandVersion"] != null && queryResult["OperatingSystemNameandVersion"].ObjectValue != null)
            {
                OperatingSystemNameandVersion = queryResult["OperatingSystemNameandVersion"].StringValue;
            }
            else { OperatingSystemNameandVersion = null; }

            if (queryResult["ResourceDomainORWorkgroup"] != null && queryResult["ResourceDomainORWorkgroup"].ObjectValue != null)
            {
                ResourceDomainORWorkgroup = queryResult["ResourceDomainORWorkgroup"].StringValue;
            }
            else { ResourceDomainORWorkgroup = null; }

            if (queryResult["ResourceID"] != null && queryResult["ResourceID"].ObjectValue != null)
            {
                ResourceId = Convert.ToUInt32(queryResult["ResourceID"].StringValue);
            }
            else
            {
                throw new Exception("SMS_R_System object is invalid. Unable to get Resource Id from IResultObject");
            }

            if (queryResult["SID"] != null && queryResult["SID"].ObjectValue != null)
            {
                SID = queryResult["SID"].StringValue;
            }
            else { SID = null; }

            if (queryResult["SMBIOSGUID"] != null && queryResult["SMBIOSGUID"].ObjectValue != null)
            {
                SMBIOSGUID = queryResult["SMBIOSGUID"].StringValue;
            }
            else { SMBIOSGUID = null; }

            if (queryResult["SMSAssignedSites"] != null && queryResult["SMSAssignedSites"].ObjectValue != null)
            {
                SMSAssignedSites = queryResult["SMSAssignedSites"].StringArrayValue;
            }
            else { SMSAssignedSites = null; }
        }
    }

    [DataContract]
    public class SCCMPackage
    {
        [DataMember]
        public string PackageID { get; set; }
        [DataMember]
        public string PackageName { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Language { get; set; }

        public SCCMPackage() { }

        public SCCMPackage(IResultObject queryResult)
        {
            if (queryResult["PackageID"] != null && queryResult["PackageID"].ObjectValue != null)
            {
                PackageID = queryResult["PackageID"].StringValue;
            }
            else
            {
                throw new Exception("Package object is invalid. Unable to get PackageID from IResultObject");
            }

            if (queryResult["Name"] != null && queryResult["Name"].ObjectValue != null)
            {
                PackageName = queryResult["Name"].StringValue;
            }
            else { PackageName = ""; }

            if (queryResult["Manufacturer"] != null && queryResult["Manufacturer"].ObjectValue != null)
            {
                Manufacturer = queryResult["Manufacturer"].StringValue;
            }
            else { Manufacturer = ""; }

            if (queryResult["Version"] != null && queryResult["Version"].ObjectValue != null)
            {
                Version = queryResult["Version"].StringValue;
            }
            else { Version = ""; }

            if (queryResult["Language"] != null && queryResult["Language"].ObjectValue != null)
            {
                Language = queryResult["Language"].StringValue;
            }
            else { Language = ""; }
        }
    }

    [DataContract]
    public class SCCMApplication
    {
        [DataMember]
        public UInt32 CI_ID { get; set; }
        [DataMember]
        public String CI_UniqueID { get; set; }
        [DataMember]
        public Boolean HasContent { get; set; }
        [DataMember]
        public Boolean IsDeployable { get; set; }
        [DataMember]
        public Boolean IsDeployed { get; set; }
        [DataMember]
        public String LocalizedDescription { get; set; }
        [DataMember]
        public String LocalizedDisplayName { get; set; }
        [DataMember]
        public UInt32 LocalizedPropertyLocaleID { get; set; }
        [DataMember]
        public String Manufacturer { get; set; }
        [DataMember]
        public String ModelName { get; set; }
        [DataMember]
        public UInt32 ModelID { get; set; }
        [DataMember]
        public UInt32 NumberOfDeployments { get; set; }
        [DataMember]
        public UInt32 NumberOfDeploymentTypes { get; set; }
        [DataMember]
        public String SoftwareVersion { get; set; }
        [DataMember]
        public UInt32 SourceCIVersion { get; set; }
        [DataMember]
        public String SourceSite { get; set; }

        public SCCMApplication() { }

        public SCCMApplication(IResultObject queryResult)
        {
            if (queryResult["CI_ID"] != null && queryResult["CI_ID"].ObjectValue != null)
            {
                CI_ID = Convert.ToUInt32(queryResult["CI_ID"].StringValue);
            }
            else
            {
                throw new Exception(string.Format("Application object is invalid. Unable to get CI_ID as Int32 from IResultObject. Value found is '{0}'", queryResult["CI_ID"].StringValue));
            }

            if (queryResult["CI_UniqueID"] != null && queryResult["CI_UniqueID"].ObjectValue != null)
            {
                CI_UniqueID = queryResult["CI_UniqueID"].StringValue;
            }
            else { CI_UniqueID = ""; }

            if (queryResult["HasContent"] != null && queryResult["HasContent"].ObjectValue != null)
            {
                HasContent = queryResult["HasContent"].BooleanValue;
            }
            else { HasContent = false; }

            if (queryResult["IsDeployable"] != null && queryResult["IsDeployable"].ObjectValue != null)
            {
                IsDeployable = queryResult["IsDeployable"].BooleanValue;
            }
            else { IsDeployable = false; }

            if (queryResult["IsDeployed"] != null && queryResult["IsDeployed"].ObjectValue != null)
            {
                IsDeployed = queryResult["IsDeployed"].BooleanValue;
            }
            else { IsDeployed = false; }

            if (queryResult["LocalizedDescription"] != null && queryResult["LocalizedDescription"].ObjectValue != null)
            {
                LocalizedDescription = queryResult["LocalizedDescription"].StringValue;
            }
            else { LocalizedDescription = ""; }

            if (queryResult["LocalizedDisplayName"] != null && queryResult["LocalizedDisplayName"].ObjectValue != null)
            {
                LocalizedDisplayName = queryResult["LocalizedDisplayName"].StringValue;
            }
            else { LocalizedDisplayName = ""; }

            if (queryResult["LocalizedPropertyLocaleID"] != null && queryResult["LocalizedPropertyLocaleID"].ObjectValue != null)
            {
                LocalizedPropertyLocaleID = Convert.ToUInt32(queryResult["LocalizedPropertyLocaleID"].StringValue);
            }
            else { LocalizedPropertyLocaleID = 0; }

            if (queryResult["Manufacturer"] != null && queryResult["Manufacturer"].ObjectValue != null)
            {
                Manufacturer = queryResult["Manufacturer"].StringValue;
            }
            else { Manufacturer = ""; }

            if (queryResult["ModelName"] != null && queryResult["ModelName"].ObjectValue != null)
            {
                ModelName = queryResult["ModelName"].StringValue;
            }
            else { ModelName = ""; }

            if (queryResult["ModelID"] != null && queryResult["ModelID"].ObjectValue != null)
            {
                ModelID = Convert.ToUInt32(queryResult["ModelID"].StringValue);
            }
            else { ModelID = 0; }

            if (queryResult["NumberOfDeployments"] != null && queryResult["NumberOfDeployments"].ObjectValue != null)
            {
                NumberOfDeployments = Convert.ToUInt32(queryResult["NumberOfDeployments"].StringValue);
            }
            else { NumberOfDeployments = 0; }

            if (queryResult["NumberOfDeploymentTypes"] != null && queryResult["NumberOfDeploymentTypes"].ObjectValue != null)
            {
                NumberOfDeploymentTypes = Convert.ToUInt32(queryResult["NumberOfDeploymentTypes"].StringValue);
            }
            else { NumberOfDeploymentTypes = 0; }

            if (queryResult["SoftwareVersion"] != null && queryResult["SoftwareVersion"].ObjectValue != null)
            {
                SoftwareVersion = queryResult["SoftwareVersion"].StringValue;
            }
            else { SoftwareVersion = ""; }

            if (queryResult["SourceCIVersion"] != null && queryResult["SourceCIVersion"].ObjectValue != null)
            {
                SourceCIVersion = Convert.ToUInt32(queryResult["SourceCIVersion"].StringValue);
            }
            else { SourceCIVersion = 0; }

            if (queryResult["SourceSite"] != null && queryResult["SourceSite"].ObjectValue != null)
            {
                SourceSite = queryResult["SourceSite"].StringValue;
            }
            else { SourceSite = ""; }
        }
    }

    [DataContract]
    public class SCCMTaskSequence
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string TaskSequencePackageID { get; set; }
        [DataMember]
        public DateTime SourceDate { get; set; }

        public SCCMTaskSequence() { }

        public SCCMTaskSequence(IResultObject queryResult)
        {
            if (queryResult["Name"] != null && queryResult["Name"].ObjectValue != null)
            {
                Name = queryResult["Name"].StringValue;
            }
            else { Name = ""; }

            if (queryResult["PackageID"] != null && queryResult["PackageID"].ObjectValue != null)
            {
                TaskSequencePackageID = queryResult["PackageID"].StringValue;
            }
            else
            {
                throw new Exception("Task Sequence object is invalid. Unable to get PackageID from IResultObject");
            }

            if (queryResult["SourceDate"] != null && queryResult["SourceDate"].ObjectValue != null)
            {
                SourceDate = queryResult["SourceDate"].DateTimeValue;
            }
            else { SourceDate = DateTime.MinValue; }
        }
    }

    [DataContract]
    public class SCCMAdvertisment
    {
        [DataMember]
        public UInt32 ActionInProgress;
        [DataMember]
        public UInt32 AdvertFlags;
        [DataMember]
        public string AdvertisementID;
        [DataMember]
        public string AdvertisementName;
        [DataMember]
        public Boolean AssignedScheduleEnabled;
        [DataMember]
        public Boolean AssignedScheduleIsGMT;
        [DataMember]
        public UInt32 AssignmentID;
        [DataMember]
        public string CollectionID;
        [DataMember]
        public string Comment;
        [DataMember]
        public UInt32 DeviceFlags;
        [DataMember]
        public DateTime ExpirationTime;
        [DataMember]
        public Boolean ExpirationTimeEnabled;
        [DataMember]
        public Boolean ExpirationTimeIsGMT;
        [DataMember]
        public string HierarchyPath;
        [DataMember]
        public Boolean IncludeSubCollection;
        [DataMember]
        public UInt32 MandatoryCountdown;
        [DataMember]
        public UInt32 OfferType;
        [DataMember]
        public string PackageID;
        [DataMember]
        public DateTime PresentTime;
        [DataMember]
        public Boolean PresentTimeEnabled;
        [DataMember]
        public Boolean PresentTimeIsGMT;
        [DataMember]
        public UInt32 Priority;
        [DataMember]
        public string ProgramName;
        [DataMember]
        public UInt32 RemoteClientFlags;
        [DataMember]
        public string SourceSite;
        [DataMember]
        public UInt32 TimeFlags;

        public SCCMAdvertisment() { }

        public SCCMAdvertisment(IResultObject queryResult)
        {
            if (queryResult["ActionInProgress"] != null && queryResult["ActionInProgress"].ObjectValue != null)
            {
                ActionInProgress = Convert.ToUInt32(queryResult["ActionInProgress"].StringValue);
            }
            else { ActionInProgress = 0; }

            if (queryResult["AdvertFlags"] != null && queryResult["AdvertFlags"].ObjectValue != null)
            {
                AdvertFlags = Convert.ToUInt32(queryResult["AdvertFlags"].StringValue);
            }
            else { AdvertFlags = 0; }

            if (queryResult["AdvertisementID"] != null && queryResult["AdvertisementID"].ObjectValue != null)
            {
                AdvertisementID = queryResult["AdvertisementID"].StringValue;
            }
            else
            {
                throw new Exception("Adversisment object is invalid. Unable to get AdvertisementID from IResultObject");
            }

            if (queryResult["AdvertisementName"] != null && queryResult["AdvertisementName"].ObjectValue != null)
            {
                AdvertisementName = queryResult["AdvertisementName"].StringValue;
            }
            else
            {
                AdvertisementName = "";
            }

            if (queryResult["AssignedScheduleEnabled"] != null && queryResult["AssignedScheduleEnabled"].ObjectValue != null)
            {
                AssignedScheduleEnabled = queryResult["AssignedScheduleEnabled"].BooleanValue;
            }
            else { AssignedScheduleEnabled = false; }

            if (queryResult["AssignedScheduleIsGMT"] != null && queryResult["AssignedScheduleIsGMT"].ObjectValue != null)
            {
                AssignedScheduleIsGMT = queryResult["AssignedScheduleIsGMT"].BooleanValue;
            }
            else { AssignedScheduleIsGMT = false; }

            if (queryResult["AssignmentID"] != null && queryResult["AssignmentID"].ObjectValue != null)
            {
                AssignmentID = Convert.ToUInt32(queryResult["AssignmentID"].StringValue);
            }
            else { AssignmentID = 0; }

            if (queryResult["CollectionID"] != null && queryResult["CollectionID"].ObjectValue != null)
            {
                CollectionID = queryResult["CollectionID"].StringValue;
            }
            else { CollectionID = ""; }

            if (queryResult["Comment"] != null && queryResult["Comment"].ObjectValue != null)
            {
                Comment = queryResult["Comment"].StringValue;
            }
            else { Comment = ""; }

            if (queryResult["DeviceFlags"] != null && queryResult["DeviceFlags"].ObjectValue != null)
            {
                DeviceFlags = Convert.ToUInt32(queryResult["DeviceFlags"].StringValue);
            }
            else { DeviceFlags = 0; }

            if (queryResult["ExpirationTime"] != null && queryResult["ExpirationTime"].ObjectValue != null)
            {
                ExpirationTime = queryResult["ExpirationTime"].DateTimeValue;
            }
            else { ExpirationTime = DateTime.MinValue; }

            if (queryResult["ExpirationTimeEnabled"] != null && queryResult["ExpirationTimeEnabled"].ObjectValue != null)
            {
                ExpirationTimeEnabled = queryResult["ExpirationTimeEnabled"].BooleanValue;
            }
            else { ExpirationTimeEnabled = false; }

            if (queryResult["ExpirationTimeIsGMT"] != null && queryResult["ExpirationTimeIsGMT"].ObjectValue != null)
            {
                ExpirationTimeIsGMT = queryResult["ExpirationTimeIsGMT"].BooleanValue;
            }
            else { ExpirationTimeIsGMT = false; }

            if (queryResult["HierarchyPath"] != null && queryResult["HierarchyPath"].ObjectValue != null)
            {
                HierarchyPath = queryResult["HierarchyPath"].StringValue;
            }
            else { HierarchyPath = ""; }

            if (queryResult["IncludeSubCollection"] != null && queryResult["IncludeSubCollection"].ObjectValue != null)
            {
                IncludeSubCollection = queryResult["IncludeSubCollection"].BooleanValue;
            }
            else { IncludeSubCollection = false; }

            if (queryResult["MandatoryCountdown"] != null && queryResult["MandatoryCountdown"].ObjectValue != null)
            {
                MandatoryCountdown = Convert.ToUInt32(queryResult["MandatoryCountdown"].StringValue);
            }
            else { MandatoryCountdown = 0; }

            if (queryResult["OfferType"] != null && queryResult["OfferType"].ObjectValue != null)
            {
                ActionInProgress = Convert.ToUInt32(queryResult["OfferType"].StringValue);
            }

            if (queryResult["PackageID"] != null && queryResult["PackageID"].ObjectValue != null)
            {
                PackageID = queryResult["PackageID"].StringValue;
            }
            else { PackageID = ""; }

            if (queryResult["PresentTime"] != null && queryResult["PresentTime"].ObjectValue != null)
            {
                PresentTime = queryResult["PresentTime"].DateTimeValue;
            }
            else { PresentTime = DateTime.MinValue; }

            if (queryResult["PresentTimeEnabled"] != null && queryResult["PresentTimeEnabled"].ObjectValue != null)
            {
                PresentTimeEnabled = queryResult["PresentTimeEnabled"].BooleanValue;
            }
            else { PresentTimeEnabled = false; }

            if (queryResult["PresentTimeIsGMT"] != null && queryResult["PresentTimeIsGMT"].ObjectValue != null)
            {
                PresentTimeIsGMT = queryResult["PresentTimeIsGMT"].BooleanValue;
            }
            else { PresentTimeIsGMT = false; }

            if (queryResult["Priority"] != null && queryResult["Priority"].ObjectValue != null)
            {
                Priority = Convert.ToUInt32(queryResult["Priority"].StringValue);
            }
            else { Priority = 0; }

            if (queryResult["ProgramName"] != null && queryResult["ProgramName"].ObjectValue != null)
            {
                ProgramName = queryResult["ProgramName"].StringValue;
            }
            else { ProgramName = ""; }


            if (queryResult["RemoteClientFlags"] != null && queryResult["RemoteClientFlags"].ObjectValue != null)
            {
                RemoteClientFlags = Convert.ToUInt32(queryResult["RemoteClientFlags"].StringValue);
            }
            else { RemoteClientFlags = 0; }

            if (queryResult["SourceSite"] != null && queryResult["SourceSite"].ObjectValue != null)
            {
                SourceSite = queryResult["SourceSite"].StringValue;
            }
            else { SourceSite = ""; }

            if (queryResult["TimeFlags"] != null && queryResult["TimeFlags"].ObjectValue != null)
            {
                TimeFlags = Convert.ToUInt32(queryResult["TimeFlags"].StringValue);
            }
            else { TimeFlags = 0; }
        }
    }

    [DataContract]
    public class SCCMVariable
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool IsMasked { get; set; }

        public SCCMVariable() { }

        public SCCMVariable(IResultObject queryResult)
        {
            if (queryResult["Name"] != null && queryResult["Name"].ObjectValue != null)
            {
                Name = queryResult["Name"].StringValue;
            }
            else { Name = ""; }

            if (queryResult["Value"] != null && queryResult["Value"].ObjectValue != null)
            {
                Value = queryResult["Value"].StringValue;
            }
            else { Value = ""; }

            if (queryResult["IsMasked"] != null && queryResult["IsMasked"].ObjectValue != null)
            {
                IsMasked = queryResult["IsMasked"].BooleanValue;
            }
            else { IsMasked = false; }
        }
    }

    [DataContract]
    public class SCCMBoundary
    {
        [DataMember]
        public string BoundaryFlags { get; set; }
        [DataMember]
        public UInt32 BoundaryID { get; set; }
        [DataMember]
        public string BoundaryType { get; set; }
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string[] DefaultSiteCode { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string GroupCount { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        [DataMember]
        public string[] SiteSystems { get; set; }
        [DataMember]
        public string Value { get; set; }

        public SCCMBoundary() { }

        public SCCMBoundary(IResultObject queryObj)
        {
            if (queryObj["BoundaryFlags"] != null && queryObj["BoundaryFlags"].ObjectValue != null)
            {
                BoundaryFlags = queryObj["BoundaryFlags"].StringValue;
            }
            else { BoundaryFlags = ""; }

            if (queryObj["BoundaryID"] != null && queryObj["BoundaryID"].ObjectValue != null)
            {
                BoundaryID = Convert.ToUInt32(queryObj["BoundaryID"].IntegerValue);
            }
            else
            {
                throw new Exception("Boundary object is invalid. Unable to get BoundaryID from IResultObject");
            }

            if (queryObj["BoundaryType"] != null && queryObj["BoundaryType"].ObjectValue != null)
            {
                BoundaryType = SCCM2012Steps.GetBoundaryTypeNameFromTypeInt(queryObj["BoundaryType"].IntegerValue);
            }
            else { BoundaryType = ""; }

            if (queryObj["CreatedBy"] != null && queryObj["CreatedBy"].ObjectValue != null)
            {
                CreatedBy = queryObj["CreatedBy"].StringValue;
            }
            else { CreatedBy = ""; }

            if (queryObj["CreatedOn"] != null && queryObj["CreatedOn"].ObjectValue != null)
            {
                CreatedOn = queryObj["CreatedOn"].DateTimeValue;
            }
            else { CreatedOn = DateTime.MinValue; }

            if (queryObj["DisplayName"] != null && queryObj["DisplayName"].ObjectValue != null)
            {
                DisplayName = queryObj["DisplayName"].StringValue;
            }
            else { DisplayName = ""; }

            if (queryObj["GroupCount"] != null && queryObj["GroupCount"].ObjectValue != null)
            {
                GroupCount = queryObj["GroupCount"].StringValue;
            }
            else { GroupCount = ""; }

            if (queryObj["ModifiedBy"] != null && queryObj["ModifiedBy"].ObjectValue != null)
            {
                ModifiedBy = queryObj["ModifiedBy"].StringValue;
            }
            else { ModifiedBy = ""; }

            if (queryObj["ModifiedOn"] != null && queryObj["ModifiedOn"].ObjectValue != null)
            {
                ModifiedOn = queryObj["ModifiedOn"].DateTimeValue;
            }
            else { ModifiedOn = DateTime.MinValue; }

            if (queryObj["Value"] != null && queryObj["Value"].ObjectValue != null)
            {
                Value = queryObj["Value"].StringValue;
            }
            else { Value = ""; }
        }
    }

    [DataContract]
    public class SCCMBoundaryGroup
    {
        [DataMember]
        public string CreatedBy { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public string DefaultSiteCode { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public UInt32 GroupID { get; set; }
        [DataMember]
        public Int32 MemberCount { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; }
        [DataMember]
        public DateTime ModifiedOn { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Boolean Shared { get; set; }
        [DataMember]
        public Int32 SiteSystemCount { get; set; }

        public SCCMBoundaryGroup() { }

        public SCCMBoundaryGroup(IResultObject queryObj)
        {
            if (queryObj["CreatedBy"] != null && queryObj["CreatedBy"].ObjectValue != null)
            {
                CreatedBy = queryObj["CreatedBy"].StringValue;
            }
            else { CreatedBy = ""; }

            if (queryObj["CreatedOn"] != null && queryObj["CreatedOn"].ObjectValue != null)
            {
                CreatedOn = queryObj["CreatedOn"].DateTimeValue;
            }
            else { CreatedOn = DateTime.MinValue; }

            if (queryObj["DefaultSiteCode"] != null && queryObj["DefaultSiteCode"].ObjectValue != null)
            {
                DefaultSiteCode = queryObj["DefaultSiteCode"].StringValue;
            }
            else { DefaultSiteCode = ""; }

            if (queryObj["Description"] != null && queryObj["Description"].ObjectValue != null)
            {
                Description = queryObj["Description"].StringValue;
            }
            else { Description = ""; }

            if (queryObj["GroupID"] != null && queryObj["GroupID"].ObjectValue != null)
            {
                GroupID = Convert.ToUInt32(queryObj["GroupID"].IntegerValue);
            }
            else
            {
                throw new Exception("BoundaryGroup object is invalid. Unable to get GroupID from IResultObject");
            }

            if (queryObj["MemberCount"] != null && queryObj["MemberCount"].ObjectValue != null)
            {
                MemberCount = queryObj["MemberCount"].IntegerValue;
            }
            else { MemberCount = 0; }

            if (queryObj["ModifiedBy"] != null && queryObj["ModifiedBy"].ObjectValue != null)
            {
                ModifiedBy = queryObj["ModifiedBy"].StringValue;
            }
            else { ModifiedBy = ""; }

            if (queryObj["ModifiedOn"] != null && queryObj["ModifiedOn"].ObjectValue != null)
            {
                ModifiedOn = queryObj["ModifiedOn"].DateTimeValue;
            }
            else { ModifiedOn = DateTime.MinValue; }

            if (queryObj["Name"] != null && queryObj["Name"].ObjectValue != null)
            {
                Name = queryObj["Name"].StringValue;
            }
            else { Name = ""; }

            if (queryObj["Shared"] != null && queryObj["Shared"].ObjectValue != null)
            {
                Shared = queryObj["Shared"].BooleanValue;
            }
            else { Shared = false; }

            if (queryObj["SiteSystemCount"] != null && queryObj["SiteSystemCount"].ObjectValue != null)
            {
                SiteSystemCount = queryObj["SiteSystemCount"].IntegerValue;
            }
            else { SiteSystemCount = 0; }
        }
    }

    [DataContract]
    public class SCCMSite //SMS_Site class is documented at http://msdn.microsoft.com/en-us/library/hh949223.aspx
    {
        [DataMember]
        public UInt32 BuildNumber { get; set; }
        [DataMember]
        public string InstallDir { get; set; }
        [DataMember]
        public string ReportingSiteCode { get; set; }
        [DataMember]
        public UInt32 RequestedStatus { get; set; }
        [DataMember]
        public string ServerName { get; set; }
        [DataMember]
        public string SiteCode { get; set; }
        [DataMember]
        public string SiteName { get; set; }
        [DataMember]
        public string SiteCodeAndName { get; set; }
        [DataMember]
        public UInt32 Status { get; set; }
        [DataMember]
        public string TimeZoneInfo { get; set; }
        [DataMember]
        public UInt32 Type { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public int AssignedBoundaryGroupCount { get; set; }

        public SCCMSite() { }

        public SCCMSite(IResultObject queryObj)
        {
            if (queryObj["BuildNumber"] != null && queryObj["BuildNumber"].ObjectValue != null)
            {
                BuildNumber = Convert.ToUInt32(queryObj["BuildNumber"].IntegerValue);
            }
            else { BuildNumber = 0; }

            if (queryObj["InstallDir"] != null && queryObj["InstallDir"].ObjectValue != null)
            {
                InstallDir = queryObj["InstallDir"].StringValue;
            }
            else { InstallDir = ""; }

            if (queryObj["ReportingSiteCode"] != null && queryObj["ReportingSiteCode"].ObjectValue != null)
            {
                ReportingSiteCode = queryObj["ReportingSiteCode"].StringValue;
            }
            else { ReportingSiteCode = ""; }

            if (queryObj["RequestedStatus"] != null && queryObj["RequestedStatus"].ObjectValue != null)
            {
                RequestedStatus = Convert.ToUInt32(queryObj["RequestedStatus"].IntegerValue);
            }
            else { RequestedStatus = 0; }

            if (queryObj["ServerName"] != null && queryObj["ServerName"].ObjectValue != null)
            {
                ServerName = queryObj["ServerName"].StringValue;
            }
            else { ServerName = ""; }

            if (queryObj["SiteCode"] != null && queryObj["SiteCode"].ObjectValue != null)
            {
                SiteCode = queryObj["SiteCode"].StringValue;
            }
            else
            {
                throw new Exception("Site object is invalid. Unable to get SiteCode from IResultObject");
            }

            if (queryObj["SiteName"] != null && queryObj["SiteName"].ObjectValue != null)
            {
                SiteName = queryObj["SiteName"].StringValue;
            }
            else { SiteName = ""; }

            SiteCodeAndName = string.Format("{0} - {1}", SiteCode, SiteName);

            if (queryObj["Status"] != null && queryObj["Status"].ObjectValue != null)
            {
                Status = Convert.ToUInt32(queryObj["Status"].IntegerValue);
            }
            else { Status = 0; }

            if (queryObj["TimeZoneInfo"] != null && queryObj["TimeZoneInfo"].ObjectValue != null)
            {
                TimeZoneInfo = queryObj["TimeZoneInfo"].StringValue;
            }
            else { TimeZoneInfo = ""; }

            if (queryObj["Type"] != null && queryObj["Type"].ObjectValue != null)
            {
                Type = Convert.ToUInt32(queryObj["Type"].IntegerValue);
            }
            else { Type = 0; }

            if (queryObj["Version"] != null && queryObj["Version"].ObjectValue != null)
            {
                Version = queryObj["Version"].StringValue;
            }
            else { Version = ""; }

            try { AssignedBoundaryGroupCount = SCCM2012Steps.GetAssignedBoundaryGroupCountBySiteCode(SiteCode); }
            catch { AssignedBoundaryGroupCount = 0; }
        }
    }

    [DataContract]
    public class SCCMSiteSystem //SMS_SystemResource class is documented at http://msdn.microsoft.com/en-us/library/hh949718.aspx
    {
        [DataMember]
        public bool InternetEnabled { get; set; }
        [DataMember]
        public bool InternetShared { get; set; }
        [DataMember]
        public string NALPath { get; set; }
        [DataMember]
        public string ResourceType { get; set; }
        [DataMember]
        public string RoleName { get; set; }
        [DataMember]
        public string ServerName { get; set; }
        [DataMember]
        public string ServerRemoteName { get; set; }
        [DataMember]
        public string SiteCode { get; set; }
        [DataMember]
        public UInt32 SslState { get; set; }

        public SCCMSiteSystem() { }

        public SCCMSiteSystem(IResultObject queryObj)
        {
            if (queryObj["InternetEnabled"] != null && queryObj["InternetEnabled"].ObjectValue != null)
            {
                InternetEnabled = queryObj["InternetEnabled"].BooleanValue;
            }
            else { InternetEnabled = false; }

            if (queryObj["InternetShared"] != null && queryObj["InternetShared"].ObjectValue != null)
            {
                InternetShared = queryObj["InternetShared"].BooleanValue;
            }
            else
            { InternetShared = false; }

            if (queryObj["NALPath"] != null && queryObj["NALPath"].ObjectValue != null)
            {
                NALPath = queryObj["NALPath"].StringValue;
            }
            else
            {
                throw new Exception("SiteSystem object is invalid. Unable to get NALPath from IResultObject");
            }

            if (queryObj["ResourceType"] != null && queryObj["ResourceType"].ObjectValue != null)
            {
                ResourceType = queryObj["ResourceType"].StringValue;
            }
            else { ResourceType = ""; }

            if (queryObj["RoleName"] != null && queryObj["RoleName"].ObjectValue != null)
            {
                RoleName = queryObj["RoleName"].StringValue;
            }
            else
            {
                throw new Exception("SiteSystem object is invalid. Unable to get RoleName from IResultObject");
            }

            if (queryObj["ServerName"] != null && queryObj["ServerName"].ObjectValue != null)
            {
                ServerName = queryObj["ServerName"].StringValue;
            }
            else
            {
                throw new Exception("SiteSystem object is invalid. Unable to get ServerName from IResultObject");
            }

            if (queryObj["ServerRemoteName"] != null && queryObj["ServerRemoteName"].ObjectValue != null)
            {
                ServerRemoteName = queryObj["ServerRemoteName"].StringValue;
            }
            else { ServerRemoteName = ""; }

            if (queryObj["SiteCode"] != null && queryObj["SiteCode"].ObjectValue != null)
            {
                SiteCode = queryObj["SiteCode"].StringValue;
            }
            else
            {
                throw new Exception("SiteSystem object is invalid. Unable to get SiteCode from IResultObject");
            }

            if (queryObj["SslState"] != null && queryObj["SslState"].ObjectValue != null)
            {
                SslState = Convert.ToUInt32(queryObj["SslState"].IntegerValue);
            }
            else
            {
                SslState = 0;
            }
        }
    }

    [DataContract]
    public class SCCMDeploymentSummary //SMS_DeploymentSummary class is documented at http://msdn.microsoft.com/en-us/library/hh948731.aspx
    {
        [DataMember]
        public UInt32 AssignmentID { get; set; }
        [DataMember]
        public UInt32 CI_ID { get; set; }
        [DataMember]
        public String CollectionID { get; set; }
        [DataMember]
        public String CollectionName { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public String DeploymentID { get; set; }
        [DataMember]
        public UInt32 DeploymentIntent { get; set; }
        [DataMember]
        public DateTime DeploymentTime { get; set; }
        [DataMember]
        public Int32 DesiredConfigType { get; set; }
        [DataMember]
        public DateTime EnforcementDeadline { get; set; }
        [DataMember]
        public SCCMFeatureType FeatureType { get; set; }
        [DataMember]
        public String ModelName { get; set; }
        [DataMember]
        public DateTime ModificationTime { get; set; }
        [DataMember]
        public Int32 NumberErrors { get; set; }
        [DataMember]
        public Int32 NumberInProgress { get; set; }
        [DataMember]
        public Int32 NumberOther { get; set; }
        [DataMember]
        public Int32 NumberSuccess { get; set; }
        [DataMember]
        public Int32 NumberTargeted { get; set; }
        [DataMember]
        public Int32 NumberUnknown { get; set; }
        [DataMember]
        public String PackageID { get; set; }
        [DataMember]
        public UInt32 PolicyModelID { get; set; }
        [DataMember]
        public String ProgramName { get; set; }
        [DataMember]
        public String SoftwareName { get; set; }
        [DataMember]
        public DateTime SummarizationTime { get; set; }
        [DataMember]
        public UInt32 SummaryType { get; set; }

        public SCCMDeploymentSummary() { }

        public SCCMDeploymentSummary(IResultObject queryObj)
        {
            if (queryObj["AssignmentID"] != null && queryObj["AssignmentID"].ObjectValue != null)
            {
                AssignmentID = Convert.ToUInt32(queryObj["AssignmentID"].IntegerValue);
            }
            else { AssignmentID = 0; }

            if (queryObj["CI_ID"] != null && queryObj["CI_ID"].ObjectValue != null)
            {
                CI_ID = Convert.ToUInt32(queryObj["CI_ID"].IntegerValue);
            }
            else { CI_ID = 0; }

            if (queryObj["CollectionID"] != null && queryObj["CollectionID"].ObjectValue != null)
            {
                CollectionID = queryObj["CollectionID"].StringValue;
            }
            else { throw new Exception("DeploymentSummary object is invalid. Unable to get CollectionID from IResultObject"); }

            if (queryObj["CollectionName"] != null && queryObj["CollectionName"].ObjectValue != null)
            {
                CollectionName = queryObj["CollectionName"].StringValue;
            }
            else { throw new Exception("DeploymentSummary object is invalid. Unable to get CollectionName from IResultObject"); }

            if (queryObj["CreationTime"] != null && queryObj["CreationTime"].ObjectValue != null)
            {
                CreationTime = queryObj["CreationTime"].DateTimeValue;
            }
            else { CreationTime = DateTime.MinValue; }

            if (queryObj["DeploymentID"] != null && queryObj["DeploymentID"].ObjectValue != null)
            {
                DeploymentID = queryObj["DeploymentID"].StringValue;
            }
            else { DeploymentID = ""; }

            if (queryObj["DeploymentIntent"] != null && queryObj["DeploymentIntent"].ObjectValue != null)
            {
                DeploymentIntent = Convert.ToUInt32(queryObj["DeploymentIntent"].IntegerValue);
            }
            else { DeploymentIntent = 0; }

            if (queryObj["DeploymentTime"] != null && queryObj["DeploymentTime"].ObjectValue != null)
            {
                DeploymentTime = queryObj["DeploymentTime"].DateTimeValue;
            }
            else { DeploymentTime = DateTime.MinValue; }

            if (queryObj["DesiredConfigType"] != null && queryObj["DesiredConfigType"].ObjectValue != null)
            {
                DesiredConfigType = queryObj["DesiredConfigType"].IntegerValue;
            }
            else { DesiredConfigType = 0; }

            if (queryObj["EnforcementDeadline"] != null && queryObj["EnforcementDeadline"].ObjectValue != null)
            {
                EnforcementDeadline = queryObj["EnforcementDeadline"].DateTimeValue;
            }
            else { EnforcementDeadline = DateTime.MinValue; }

            if (queryObj["FeatureType"] != null && queryObj["FeatureType"].ObjectValue != null)
            {
                FeatureType = (SCCMFeatureType)Convert.ToUInt32(queryObj["FeatureType"].IntegerValue);
            }
            else { throw new Exception("DeploymentSummary object is invalid. Unable to get FeatureType from IResultObject"); }

            if (queryObj["ModelName"] != null && queryObj["ModelName"].ObjectValue != null)
            {
                ModelName = queryObj["ModelName"].StringValue;
            }
            else { ModelName = ""; }

            if (queryObj["ModificationTime"] != null && queryObj["ModificationTime"].ObjectValue != null)
            {
                ModificationTime = queryObj["ModificationTime"].DateTimeValue;
            }
            else { ModificationTime = DateTime.MinValue; }

            if (queryObj["NumberErrors"] != null && queryObj["NumberErrors"].ObjectValue != null)
            {
                NumberErrors = queryObj["NumberErrors"].IntegerValue;
            }
            else { NumberErrors = 0; }

            if (queryObj["NumberInProgress"] != null && queryObj["NumberInProgress"].ObjectValue != null)
            {
                NumberInProgress = queryObj["NumberInProgress"].IntegerValue;
            }
            else { NumberInProgress = 0; }

            if (queryObj["NumberOther"] != null && queryObj["NumberOther"].ObjectValue != null)
            {
                NumberOther = queryObj["NumberOther"].IntegerValue;
            }
            else { NumberOther = 0; }

            if (queryObj["NumberSuccess"] != null && queryObj["NumberSuccess"].ObjectValue != null)
            {
                NumberSuccess = queryObj["NumberSuccess"].IntegerValue;
            }
            else { NumberSuccess = 0; }

            if (queryObj["NumberTargeted"] != null && queryObj["NumberTargeted"].ObjectValue != null)
            {
                NumberTargeted = queryObj["NumberTargeted"].IntegerValue;
            }
            else { NumberTargeted = 0; }

            if (queryObj["NumberUnknown"] != null && queryObj["NumberUnknown"].ObjectValue != null)
            {
                NumberUnknown = queryObj["NumberUnknown"].IntegerValue;
            }
            else { NumberUnknown = 0; }

            if (queryObj["PackageID"] != null && queryObj["PackageID"].ObjectValue != null)
            {
                PackageID = queryObj["PackageID"].StringValue;
            }
            else { PackageID = ""; }

            if (queryObj["PolicyModelID"] != null && queryObj["PolicyModelID"].ObjectValue != null)
            {
                PolicyModelID = Convert.ToUInt32(queryObj["PolicyModelID"].IntegerValue);
            }
            else { PolicyModelID = 0; }

            if (queryObj["ProgramName"] != null && queryObj["ProgramName"].ObjectValue != null)
            {
                ProgramName = queryObj["ProgramName"].StringValue;
            }
            else { ProgramName = ""; }

            if (queryObj["SoftwareName"] != null && queryObj["SoftwareName"].ObjectValue != null)
            {
                SoftwareName = queryObj["SoftwareName"].StringValue;
            }
            else { SoftwareName = ""; }

            if (queryObj["SummarizationTime"] != null && queryObj["SummarizationTime"].ObjectValue != null)
            {
                SummarizationTime = queryObj["SummarizationTime"].DateTimeValue;
            }
            else { SummarizationTime = DateTime.MinValue; }

            if (queryObj["SummaryType"] != null && queryObj["SummaryType"].ObjectValue != null)
            {
                SummaryType = Convert.ToUInt32(queryObj["SummaryType"].IntegerValue);
            }
            else { SummaryType = 0; }
        }
    }

    public enum SiteType : uint
    {
        SECONDARY = 1,
        PRIMARY,
        CAS = 4
    }

    public enum TaskSequenceDeploymentPurpose : int
    {
        Required,
        Available = 2
    }

    public enum TaskSequenceRerunBehavior
    {
        [Description("Never Rerun Program")]
        NeverRerunProgram,
        [Description("Always Rerun Program")]
        AlwaysRerunProgram,
        [Description("Rerun If Failed Previous Attempt")]
        RerunIfFailedPreviousAttempt,
        [Description("Rerun If Succeeded On Previous Attempt")]
        RerunIfSucceededOnPreviousAttempt
    }

    public enum TaskSequenceOptions
    {
        [Description("Download Content Locally When Needed By Running Task Sequence")]
        DownloadContentLocallyWhenNeededByRunningTaskSequence,
        [Description("Download All Content Locally Before Starting Task Sequence")]
        DownloadAllContentLocallyBeforeStartingTaskSequence
    }

    public enum TaskSequenceTargetOptions
    {
        [Description("Only Configuration Manager Clients")]
        OnlyConfigurationManagerClients,
        [Description("Configuration Manager clients, media and PXE")]
        ConfigurationManagerClientsMediaAndPXE,
        [Description("Only media and PXE")]
        OnlyMediaAndPXE,
        [Description("Only media and PXE (hidden)")]
        OnlyMediaAndPXEHidden
    }

    public enum ApplicationIdType
    {
        [Description("CI ID")]
        CI_ID,
        [Description("Model ID")]
        ModelID
    }

    public enum SCCMFeatureType
    {
        Application = 1,
        MobileProgram,
        Script,
        SoftwareUpdate,
        Baseline,
        TaskSequence,
        ContentDistribution,
        DistributionPointGroup,
        DistributionPointHealth,
        ConfigurationPolicy
    }

    [DataContract]
    public class SCCMTaskSequenceFlags //Flags are documented at http://msdn.microsoft.com/en-us/library/hh948575.aspx
    {
        #region Advert Flags
        [DataMember]
        [PropertyClassification(0, "Assignment schedule: As soon as possible", "Advert Flags", "Scheduling")]
        //Hexadecimal (bit) = 0x00000020 (5)
        public bool adv_IMMEDIATE { get; set; }
        [DataMember]
        [PropertyClassification(0, "Announce the advertisement to the user on system startup", "Advert Flags")]
        //Hexadecimal (bit) = 0x00000100 (8)
        public bool adv_ONSYSTEMSTARTUP { get; set; }
        [DataMember]
        [PropertyClassification(0, "Assignment schedule: Log on", "Advert Flags", "Scheduling")]
        //Hexadecimal (bit) = 0x00000200 (9)
        public bool adv_ONUSERLOGON { get; set; }
        [DataMember]
        [PropertyClassification(0, "Assignment schedule: Log off", "Advert Flags", "Scheduling")]
        //Hexadecimal (bit) = 0x00000400 (10)
        public bool adv_ONUSERLOGOFF { get; set; }
        [DataMember]
        [PropertyClassification(0, "The advertisement is for a device client", "Advert Flags")]
        //Hexadecimal (bit) = 0x00008000 (15)
        public bool adv_WINDOWS_CE { get; set; }
        [DataMember]
        [PropertyClassification(0, "Allow clients to use a fallback source location for content", "Advert Flags", "Distribution Points")]
        //Hexadecimal (bit) = 0x00020000 (17). Note: true in the SCCM UI = flase for this flag.
        public bool adv_DONOT_FALLBACK { get; set; }
        [DataMember]
        [PropertyClassification(0, "Make available to boot media and PXE", "Advert Flags", "Deployment Settings")]
        //Hexadecimal (bit) = 0x00040000 (18)
        public bool adv_ENABLE_TS_FROM_CD_AND_PXE { get; set; }
        [DataMember]
        [PropertyClassification(0, "Allow task sequence to run for client on the Internet", "Advert Flags", "User Experience")]
        //Hexadecimal (bit) = 0x00080000 (19). NOTE: this flag is undocumented. Setting this value sets option to false, otherwise it will be true. True in the SCCM UI = False for this flag
        public bool adv_ALLOW_INTERNET_CLIENTS { get; set; }
        [DataMember]
        [PropertyClassification(0, "Allow Outside of Maintenance Window: Software Installation", "Advert Flags", "User Experience")]
        //Hexadecimal (bit) = 0x00100000 (20)
        public bool adv_OVERRIDE_SERVICE_WINDOWS { get; set; }
        [DataMember]
        [PropertyClassification(0, "Allow Outside of Maintenance Window: System restart", "Advert Flags", "User Experience")]
        //Hexadecimal (bit) = 0x00200000 (21)
        public bool adv_REBOOT_OUTSIDE_OF_SERVICE_WINDOWS { get; set; }
        [DataMember]
        [PropertyClassification(0, "Send wake-up packets", "Advert Flags", "Deployment Settings")]
        //Hexadecimal (bit) = 0x00400000 (22)
        public bool adv_WAKE_ON_LAN_ENABLED { get; set; }
        [DataMember]
        [PropertyClassification(0, "Show Task Sequence progress", "Advert Flags", "User Experience")]
        //Hexadecimal (bit) = 0x00800000 (23)
        public bool adv_SHOW_PROGRESS { get; set; }
        [DataMember]
        [PropertyClassification(0, "Allow users to run the program independently of assignments", "Advert Flags", "User Experience")]
        //Hexadecimal (bit) = 0x02000000 (25) NOTE: True in the SCCM UI = False for this flag
        public bool adv_NO_DISPLAY { get; set; }
        [DataMember]
        [PropertyClassification(0, "Assignments are mandatory over a slow network connection", "Advert Flags")]
        //Hex = 0x04000000 (26)
        public bool adv_ONSLOWNET { get; set; }
        [DataMember]
        [PropertyClassification(0, "Restrict to Only Media and PXE", "Advert Flags")]
        //Hex = 0x10000000
        public bool adv_ENABLE_TS_ONLY_MEDIA_AND_PXE { get; set; }
        [DataMember]
        [PropertyClassification(0, "Restrict to Only Media and PXE (hidden)", "Advert Flags")]
        //Hex = 0x30000000
        public bool adv_ENABLE_TS_ONLY_MEDIA_AND_PXE_HIDDEN { get; set; }
        #endregion

        #region Remote Client Flags
        [DataMember]
        [PropertyClassification(0, "Run the program from the local distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000008 (3)
        public bool rem_RUN_FROM_LOCAL_DISPPOINT { get; set; }
        [DataMember]
        [PropertyClassification(0, "Download the program from the local distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000010 (4)
        public bool rem_DOWNLOAD_FROM_LOCAL_DISPPOINT { get; set; }
        [DataMember]
        [PropertyClassification(0, "Do not run the program if there is no local distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000020 (5)
        public bool rem_DONT_RUN_NO_LOCAL_DISPPOINT { get; set; }
        [DataMember]
        [PropertyClassification(0, "Download the program from the remote distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000040 (6)
        public bool rem_DOWNLOAD_FROM_REMOTE_DISPPOINT { get; set; }
        [DataMember]
        [PropertyClassification(0, "Run the program from the remote distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000080 (7)
        public bool rem_RUN_FROM_REMOTE_DISPPOINT { get; set; }
        [DataMember]
        [PropertyClassification(0, "Download the program on demand from the local distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000100 (8)
        public bool rem_DOWNLOAD_ON_DEMAND_FROM_LOCAL_DP { get; set; }
        [DataMember]
        [PropertyClassification(0, "Download the program on demand from the remote distribution point", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000200 (9)
        public bool rem_DOWNLOAD_ON_DEMAND_FROM_REMOTE_DP { get; set; }
        [DataMember]
        [PropertyClassification(0, "Balloon reminders are required", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000400 (10)
        public bool rem_BALLOON_REMINDERS_REQUIRED { get; set; }
        [DataMember]
        [PropertyClassification(0, "Always rerun the program", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00000800 (11)
        public bool rem_RERUN_ALWAYS { get; set; }
        [DataMember]
        [PropertyClassification(0, "Never rerun the program", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00001000  (12)
        public bool rem_RERUN_NEVER { get; set; }
        [DataMember]
        [PropertyClassification(0, "Rerun the program if execution previously failed", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00002000 (13)
        public bool rem_RERUN_IF_FAILED { get; set; }
        [DataMember]
        [PropertyClassification(0, "Rerun the program if execution previously succeeded", "Remote Client Flags")]
        //Hexadecimal (bit) = 0x00004000 (14)
        public bool rem_RERUN_IF_SUCCEEDED { get; set; }
        #endregion

        #region Device Flags - Not currently in use
        /* We currently are not using these options in any method. This can be uncommented if we find a need for these options
        [DataMember]
        [PropertyClassification(0, "Always assign program to the client", "Device Flags")]
        //Hexadecimal (bit) = 0x01000000 (24)
        public bool dev_ASSIGN_PROGRAM_TO_CLIENT { get; set; }
        [DataMember]
        [PropertyClassification(0, "Assign only if the device is currently connected to a high-bandwidth connection", "Device Flags")]
        //Hexadecimal (bit) = 0x02000000  (25)
        public bool dev_LIMIT_TO_HIGH_BANDWIDTH { get; set; }
        [DataMember]
        [PropertyClassification(0, "Assign only if the device is docked, that is, it is attached to a desktop that is using ActiveSync", "Device Flags")]
        //Hexadecimal (bit) = 0x04000000  (26)
        public bool dev_ASSIGN_ONLY_DOCKED_DEVICE { get; set; }
        */
        #endregion

        public SCCMTaskSequenceFlags() { }
    }
}
