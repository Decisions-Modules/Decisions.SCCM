using DecisionsFramework.Data.ORMapper;
using System.Runtime.Serialization;
using DecisionsFramework;
using DecisionsFramework.ServiceLayer;
using DecisionsFramework.Design.Properties;
using System.Collections.Generic;
using DecisionsFramework.ServiceLayer.Actions;
using DecisionsFramework.ServiceLayer.Services.Accounts;
using DecisionsFramework.ServiceLayer.Actions.Common;
using DecisionsFramework.ServiceLayer.Services.Portal;
using System.Reflection;
using System.IO;
using System;
using DecisionsFramework.Design.Properties.Attributes;
using SCCM_2012;
using DecisionsFramework.ServiceLayer.Utilities;
namespace SCCMModule
{

    [ORMEntity]
    [DataContract]
    [ValidationRules]
    public class SCCMIntegrationSettings : AbstractModuleSettings, IInitializable
    {
        private static readonly Log _log = new Log("SCCM Module Settings");

        public SCCMIntegrationSettings()
        {
            base.EntityName = "SCCM Integration Settings";
        }

        #region properties

        [ORMField]
        private string sccmServerAddress = "";

        [DataMember]
        [PropertyClassification(0, "SCCM Server Address", "SCCM Connection")]
        public string SCCMServerAddress
        {
            get
            {
                return sccmServerAddress;
            }
            set
            {
                sccmServerAddress = value;
            }
        }

        [ORMField]
        private string sccmServerUserName;

        [DataMember]
        [PropertyClassification(1, "SCCM Server User Name", "SCCM Connection")]
        public string SCCMServerUsername
        {
            get
            {
                return sccmServerUserName;
            }
            set
            {
                sccmServerUserName = value;
            }
        }

        [ORMField(typeof(EncryptedConverter))]
        private string sccmServerUserPassword;

        [DataMember]
        [PasswordText]
        [ExcludeInDescription]
        [PropertyClassification(2, "SCCM Server User Password", "SCCM Connection")]
        public string SCCMServerUserPassword
        {
            get
            {
                return sccmServerUserPassword;
            }
            set
            {
                sccmServerUserPassword = value;
            }
        }

        #endregion

        public void Initialize() {
            SCCMIntegrationSettings.GetSettings();

            PortalSettings portalSettings = ModuleSettingsAccessor<PortalSettings>.GetSettings();
            //portalSettings.SloganText = "SCCM Management";
            //portalSettings.ShowDefaultLogo = false;
            
            // Commenting out for SKT
            //portalSettings.ShowHomeFolder = false;

            //portalSettings.ServiceCatalogNodeName = "SCCM Services";
            portalSettings.ShowServiceCatalogNode = true;
            portalSettings.ShowKnowledgeBaseNode = false;
            portalSettings.ShowFavoritesNode = false;
            portalSettings.ShowRecentNode = false;

            /*Assembly assem = GetType().Assembly;
            byte[] imageContents = null;
            _log.LogInfo(this, "Getting default logo");
            try
            {
                using (Stream stream = assem.GetManifestResourceStream("ProteusService.logo.png"))
                {
                    if (stream != null)
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            imageContents = reader.ReadBytes((int)stream.Length);

                            reader.Close();
                            stream.Close();

                        }
                }
            }
            catch (Exception ex)
            {
                _log.LogException(this, ex);
                _log.LogError(this, "Could not load the image from the assembly");
            }

            if (imageContents != null)
            {
                portalSettings.Logo = imageContents;
            }

            _log.LogInfo(this, "Saving portal settings.");
             */
            ModuleSettingsAccessor<PortalSettings>.SaveSettings();
            
        }
        
        #region HelperMethods
        public static SCCMIntegrationSettings GetSettings()
        {
            SCCMIntegrationSettings coreSettings = ModuleSettingsAccessor<SCCMIntegrationSettings>.GetSettings();
            // Setup the holder used by all steps.
            if (SCCMConnectionInfo.SETTINGS == null) {
                SCCMConnectionInfo.SETTINGS = new SCCMConnectionInfo();
            }

            SCCMConnectionInfo.SETTINGS.SCCMServerAddress = coreSettings.SCCMServerAddress;
            SCCMConnectionInfo.SETTINGS.SCCMServerUsername = coreSettings.SCCMServerUsername;
            SCCMConnectionInfo.SETTINGS.SCCMServerUserPassword = coreSettings.SCCMServerUserPassword;
            // Done setting up.
            return coreSettings;
        }
        public static void SaveSettings()
        {
            ModuleSettingsAccessor<SCCMIntegrationSettings>.SaveSettings();
            if (SCCMConnectionInfo.SETTINGS == null)
            {
                SCCMConnectionInfo.SETTINGS = new SCCMConnectionInfo();
            }

            // Refetch and reset the holder.... HSO this stuff is required for proper operation.
            SCCMIntegrationSettings coreSettings = ModuleSettingsAccessor<SCCMIntegrationSettings>.GetSettings();
            SCCMConnectionInfo.SETTINGS.SCCMServerAddress = coreSettings.SCCMServerAddress;
            SCCMConnectionInfo.SETTINGS.SCCMServerUsername = coreSettings.SCCMServerUsername;
            SCCMConnectionInfo.SETTINGS.SCCMServerUserPassword = coreSettings.SCCMServerUserPassword;
        }
        #endregion

        public override BaseActionType[] GetActions(DecisionsFramework.ServiceLayer.Utilities.AbstractUserContext userContext, DecisionsFramework.ServiceLayer.Actions.EntityActionType[] types)
        {
            List<BaseActionType> actionTypes = new List<BaseActionType>();
            Account account = userContext.GetAccount();

            actionTypes.Add(new EditEntityAction(typeof(SCCMIntegrationSettings), "Edit Settings", "Edit Integration Settings"));
           
            return actionTypes.ToArray();
        }
    }
}