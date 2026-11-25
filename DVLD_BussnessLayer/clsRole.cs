using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsRole
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool CanManageLicensesAndApps { get; set; }
        public bool CanManageTests { get; set; }
        public bool CanManageVehicles { get; set; }
        public bool CanManageUsersAndSettings { get; set; }

        public clsRole()
        {
            RoleID = -1;
            RoleName = "";
            RoleDescription = "";
            CanManageLicensesAndApps = false;
            CanManageTests = false;
            CanManageVehicles = false;
            CanManageUsersAndSettings = false;
        }

        private clsRole(int roleID, string roleName, string roleDescription,
            bool canManageLicensesAndApps, bool canManageTests,
            bool canManageVehicles, bool canManageUsersAndSettings)
        {
            RoleID = roleID;
            RoleName = roleName;
            RoleDescription = roleDescription;
            CanManageLicensesAndApps = canManageLicensesAndApps;
            CanManageTests = canManageTests;
            CanManageVehicles = canManageVehicles;
            CanManageUsersAndSettings = canManageUsersAndSettings;
        }

        public static clsRole Find(int RoleID)
        {
            string roleName = "", roleDescription = "";
            bool canLicenses = false, canTests = false, canVehicles = false, canUsers = false;

            if (clsRoleData.GetRoleInfoByID(RoleID, ref roleName, ref roleDescription,
                ref canLicenses, ref canTests, ref canVehicles, ref canUsers))
            {
                return new clsRole(RoleID, roleName, roleDescription, canLicenses, canTests, canVehicles, canUsers);
            }
            return null;
        }

        public static clsRole Find(string RoleName)
        {
            int roleID = -1;
            string roleDescription = "";
            bool canLicenses = false, canTests = false, canVehicles = false, canUsers = false;

            if (clsRoleData.GetRoleInfoByName(RoleName, ref roleID, ref roleDescription,
                ref canLicenses, ref canTests, ref canVehicles, ref canUsers))
            {
                return new clsRole(roleID, RoleName, roleDescription, canLicenses, canTests, canVehicles, canUsers);
            }
            return null;
        }

        public static DataTable GetAllRoles()
        {
            return clsRoleData.GetAllRoles();
        }
    
    
    
    }


}
namespace DVLD_BuisnessLayer
{
    internal class _NamespaceCompatibility { }
}