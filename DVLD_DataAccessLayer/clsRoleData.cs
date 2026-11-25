using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsRoleData
    {
        public static bool GetRoleInfoByID(int RoleID, ref string RoleName, ref string RoleDescription,
            ref bool CanManageLicensesAndApps, ref bool CanManageTests,
            ref bool CanManageVehicles, ref bool CanManageUsersAndSettings)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Roles WHERE RoleID = @RoleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleID", RoleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    RoleName = (string)reader["RoleName"];
                    RoleDescription = reader["RoleDescription"] != DBNull.Value ? (string)reader["RoleDescription"] : "";
                    CanManageLicensesAndApps = (bool)reader["CanManageLicensesAndApps"];
                    CanManageTests = (bool)reader["CanManageTests"];
                    CanManageVehicles = (bool)reader["CanManageVehicles"];
                    CanManageUsersAndSettings = (bool)reader["CanManageUsersAndSettings"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static bool GetRoleInfoByName(string RoleName, ref int RoleID, ref string RoleDescription,
            ref bool CanManageLicensesAndApps, ref bool CanManageTests,
            ref bool CanManageVehicles, ref bool CanManageUsersAndSettings)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Roles WHERE RoleName = @RoleName";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@RoleName", RoleName);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    RoleID = (int)reader["RoleID"];
                    RoleDescription = reader["RoleDescription"] != DBNull.Value ? (string)reader["RoleDescription"] : "";
                    CanManageLicensesAndApps = (bool)reader["CanManageLicensesAndApps"];
                    CanManageTests = (bool)reader["CanManageTests"];
                    CanManageVehicles = (bool)reader["CanManageVehicles"];
                    CanManageUsersAndSettings = (bool)reader["CanManageUsersAndSettings"];
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }

        public static DataTable GetAllRoles()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Roles ORDER BY RoleID";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    dt.Load(reader);
                reader.Close();
            }
            catch (Exception ex) { }
            finally
            {
                connection.Close();
            }

            return dt;
        }
    }
}