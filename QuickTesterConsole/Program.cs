using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCCM_2012;

namespace QuickTesterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string userChoice = null;

            do
            {
                if (!string.IsNullOrEmpty(userChoice))
                {
                    Console.WriteLine("Press Enter to return to main menu.");
                    Console.ReadLine();
                    Console.WriteLine();
                }

                Console.WriteLine(" 1 - GetAllCollections");
                Console.WriteLine(" 2 - GetCollectionItemsByCollectionID");
                Console.WriteLine(" 3 - GetCollectionItemsByCollectionName");
                Console.WriteLine(" 4 - GetAllApplications");
                Console.WriteLine(" 5 - BoundaryGroups Methods");
                Console.WriteLine(" 6 - Boundary Methods");
                Console.WriteLine(" 7 - Create device in dynamic collection with vars");
                Console.WriteLine(" 8 - Add site to boundary group");
                Console.WriteLine(" 9 - Remove site from broundary group");
                Console.WriteLine("10 - Add device to collection (and refresh col membership)");
                Console.WriteLine("11 - Delete variable from device");
                Console.WriteLine();
                Console.WriteLine("Enter number of method to test: ");
                userChoice = Console.ReadLine();

                switch (userChoice)
                {
                    #region case 1
                    case "1":
                        GetAndDisplayAllColections();
                        break;
                    #endregion
                    #region case 2
                    case "2":
                        GetAndDisplayAllColections();
                        Console.WriteLine();
                        Console.WriteLine("Enter collection ID:");
                        string colId = Console.ReadLine();
                        SCCMSystem[] resourcesById = SCCM2012Steps.GetCollectionItemsByCollectionID(colId);
                        DisplaySCCMResourceItems(resourcesById);
                        break;
                    #endregion
                    #region case 3
                    case "3":
                        GetAndDisplayAllColections();
                        Console.WriteLine(); ;
                        Console.WriteLine("Enter collection Name:");
                        string colName = Console.ReadLine();
                        SCCMSystem[] resourcesByName = SCCM2012Steps.GetCollectionItemsByCollectionName(colName);
                        DisplaySCCMResourceItems(resourcesByName);
                        break;
                    #endregion
                    #region case 4
                    case "4":
                        SCCMApplication[] applications = SCCM2012Steps.GetAllApplications();
                        foreach (SCCMApplication app in applications)
                        {
                            Console.WriteLine(app.CI_ID.ToString().PadRight(12) + app.CI_UniqueID.ToString().PadRight(12) + app.LocalizedDisplayName);

                        }
                        break;
                    #endregion
                    #region case 5
                    case "5":
                        SCCMBoundaryGroup[] boundaryGroups = SCCM2012Steps.GetAllBoundaryGroups();
                        SCCMBoundaryGroup bGroup = boundaryGroups[0];
                        Console.WriteLine("getting boundary group by id " + bGroup.GroupID);
                        Console.WriteLine("found boundary group " + SCCM2012Steps.GetBoundaryGroupByID(bGroup.GroupID).Name);
                        Console.WriteLine("Enter new name for this group");
                        string newName = Console.ReadLine();
                        SCCM2012Steps.EditBoundaryGroupByID(bGroup.GroupID, newName);
                        Console.WriteLine("Done! now hit ENTER to delete this group");
                        Console.ReadLine();

                        Console.WriteLine("deleting " + boundaryGroups[0].Name);
                        SCCM2012Steps.DeleteBoundaryGroup(boundaryGroups[0].GroupID);
                        Console.WriteLine("done!!");
                        break;
                    #endregion
                    #region case 6
                    case "6":
                        SCCMBoundary[] boundaries = SCCM2012Steps.GetAllBoundaries();
                        SCCMBoundary b = boundaries[0];
                        Console.WriteLine("getting boundary by id " + b.BoundaryID);
                        Console.WriteLine("found boundary " + SCCM2012Steps.GetBoundaryByID(b.BoundaryID).DisplayName);
                        Console.WriteLine("Enter new name for this boundary");
                        string newBName = Console.ReadLine();
                        SCCM2012Steps.EditBoundaryByID(b.BoundaryID, b.Value, newBName, b.BoundaryType);
                        Console.WriteLine("Done! now hit ENTER to delete this boundary");
                        Console.ReadLine();

                        Console.WriteLine("deleting " + b.DisplayName);
                        SCCM2012Steps.DeleteBoundary(b.BoundaryID);
                        Console.WriteLine("done!!");
                        break;
                    #endregion
                    #region case 7
                    case "7":
                        Console.WriteLine("Enter name for new machine");
                        string newMachineName = Console.ReadLine();
                        Console.WriteLine();
                        Console.WriteLine("Enter name for new collection");
                        string newCollectionName = Console.ReadLine();

                        Console.WriteLine();
                        Console.WriteLine("Creating Device...");
                        UInt32 newDeviceId = SCCM2012Steps.CreateDevice(newMachineName, GenerateMACAddress());
                        Console.WriteLine(string.Format("DONE. Device Id {0} created.", newDeviceId));

                        Console.WriteLine();
                        Console.WriteLine("Creating dynamic collection for device...");
                        string newCollectionId = SCCM2012Steps.CreateDynamicCollectionForDevices(newCollectionName, "dynaic collection", new UInt32[1] { newDeviceId });
                        Console.WriteLine(string.Format("DONE. Collection Id {0} created and device id {1} added.", newCollectionId, newDeviceId));

                        Console.WriteLine();
                        Console.WriteLine("Adding variable to device...");
                        SCCMVariable newVar = new SCCMVariable();
                        newVar.Name = "TestVarName";
                        newVar.Value = "TestVarValue";
                        newVar.IsMasked = false;
                        SCCM2012Steps.SetCustomVariableForDevice(newDeviceId, newVar);
                        Console.WriteLine(string.Format("DONE. Variable {0} added to device {1}", newVar.Name, newDeviceId));
                        break;
                    #endregion
                    #region case 8
                    case "8":
                        Console.WriteLine("Enter site code");
                        string siteCode = Console.ReadLine();
                        
                        Console.WriteLine();
                        Console.WriteLine("Enter server name");
                        string siteServerName = Console.ReadLine();
                        
                        Console.WriteLine();
                        Console.WriteLine("Enter boundary group id");
                        UInt32 boundaryGroupId = Convert.ToUInt32(Console.ReadLine());

                        Console.WriteLine();
                        Console.WriteLine("Adding site system to boundary group...");
                        SCCM2012Steps.AddSiteServerToBoundaryGroup(siteServerName, siteCode, false, boundaryGroupId);
                        Console.WriteLine("DONE!");
                        break;
                    #endregion
                    #region case 9
                    case "9":
                        Console.WriteLine("Enter site code");
                        string siteCodeToRemove = Console.ReadLine();
                        
                        Console.WriteLine();
                        Console.WriteLine("Enter server name");
                        string siteServerNameToRemove = Console.ReadLine();
                        
                        Console.WriteLine();
                        Console.WriteLine("Enter boundary group id");
                        UInt32 boundaryGroupIdToRemoveFrom = Convert.ToUInt32(Console.ReadLine());

                        Console.WriteLine();
                        Console.WriteLine("Removing site system from boundary group...");
                        SCCM2012Steps.RemoveSiteServerFromBoundaryGroup(siteServerNameToRemove, siteCodeToRemove, boundaryGroupIdToRemoveFrom);
                        Console.WriteLine("DONE!");
                        break;
                    #endregion
                    #region case 10
                    case "10":
                        Console.WriteLine("Enter device id");
                        uint deviceIdToAdd = Convert.ToUInt32(Console.ReadLine());

                        Console.WriteLine();
                        Console.WriteLine("Enter collection id");
                        string collectionIdToAddTo = Console.ReadLine();

                        Console.WriteLine();
                        Console.WriteLine("Adding device to collection...");
                        SCCM2012Steps.AddDevicesToExistingCollection(collectionIdToAddTo, new uint[1] { deviceIdToAdd });
                        Console.WriteLine("DONE adding device to collection");

                        Console.WriteLine();
                        Console.WriteLine("Refreshing collection memebership...");
                        SCCM2012Steps.RefreshCollectionMembership(collectionIdToAddTo);
                        Console.WriteLine("DONE refreshing collection membership.");
                        break;
                    #endregion
                    #region case 11
                    case "11":
                        Console.WriteLine("Enter device id");
                        UInt32 deviceIdToRemoveVar = Convert.ToUInt32(Console.ReadLine());

                        Console.WriteLine();
                        Console.WriteLine("Enter variable name");
                        string varibleNameToRemove = Console.ReadLine();

                        Console.WriteLine();
                        Console.WriteLine("Removing variable from device...");
                        SCCM2012Steps.DeleteCustomVariableFromDevice(deviceIdToRemoveVar, varibleNameToRemove);
                        Console.WriteLine("DONE removing variable from device.");
                        break;
                    #endregion
                    case "12":
                        Console.WriteLine("enter int.");
                        int intValue = Convert.ToInt32(Console.ReadLine());
                        string hexValue = intValue.ToString("X");
                        Console.WriteLine(string.Format("hex value = {0}", hexValue));
                        break;
                    default:
                        Console.WriteLine("This option is not handled yet");
                        break;
                }



            } while (userChoice != "exit");

        }

        private static void GetAndDisplayAllColections()
        {
            SCCMCollection[] collections = SCCM2012Steps.GetAllCollections();
            Console.WriteLine();
            Console.WriteLine("All Collections");
            Console.WriteLine("ID".PadRight(12) + "Name");
            foreach (SCCMCollection col in collections)
            {
                Console.WriteLine(string.Format("{0} {1}", col.CollectionId.PadRight(12), col.CollectionName));
            }
        }

        private static void DisplaySCCMResourceItems(SCCMSystem[] resources)
        {
            Console.WriteLine();
            Console.WriteLine("Resources");
            Console.WriteLine("ID".PadRight(12) + "Name");
            foreach (SCCMSystem res in resources)
            {
                Console.WriteLine(res.ResourceId.ToString().PadRight(12) + res.ResourceName);
            }
        }

        public static string GenerateMACAddress()
        {
            var sBuilder = new StringBuilder();
            var r = new Random();
            int number;
            byte b;
            for (int i = 0; i < 6; i++)
            {
                number = r.Next(0, 255);
                b = Convert.ToByte(number);
                if (i == 0)
                {
                    b = setBit(b, 6); //--> set locally administered
                    b = unsetBit(b, 7); // --> set unicast 
                }
                sBuilder.Append(number.ToString("X2"));
                if (i != 5)
                {
                    sBuilder.Append(":");
                }
            }
            return sBuilder.ToString().ToUpper();
        }

        private static byte setBit(byte b, int BitNumber)
        {
            if (BitNumber < 8 && BitNumber > -1)
            {
                return (byte)(b | (byte)(0x01 << BitNumber));
            }
            else
            {
                throw new InvalidOperationException(
                "Der Wert für BitNumber " + BitNumber.ToString() + " war nicht im zulässigen Bereich! (BitNumber = (min)0 - (max)7)");
            }
        }

        private static byte unsetBit(byte b, int BitNumber)
        {
            if (BitNumber < 8 && BitNumber > -1)
            {
                return (byte)(b | (byte)(0x00 << BitNumber));
            }
            else
            {
                throw new InvalidOperationException(
                "Der Wert für BitNumber " + BitNumber.ToString() + " war nicht im zulässigen Bereich! (BitNumber = (min)0 - (max)7)");
            }
        }
    }

}
