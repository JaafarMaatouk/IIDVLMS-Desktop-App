using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsVehicleData
    {
        public static bool GetVehicleInfoByID(int VehicleID, ref string PlateNumber, ref string ChassisNumber,
            ref string Make, ref string Model, ref int ManufactureYear, ref string Color, ref string FuelType,
            ref int RequiredLicenseClassID, ref int CurrentOwnerPersonID, ref DateTime RegistrationDate,
            ref bool IsActive, ref int CreatedByUserID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Vehicles WHERE VehicleID = @VehicleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    PlateNumber = (string)reader["PlateNumber"];
                    ChassisNumber = (string)reader["ChassisNumber"];
                    Make = (string)reader["Make"];
                    Model = (string)reader["Model"];
                    ManufactureYear = (int)reader["ManufactureYear"];
                    Color = (string)reader["Color"];
                    FuelType = (string)reader["FuelType"];
                    RequiredLicenseClassID = (int)reader["RequiredLicenseClassID"];
                    CurrentOwnerPersonID = (int)reader["CurrentOwnerPersonID"];
                    RegistrationDate = (DateTime)reader["RegistrationDate"];
                    IsActive = (bool)reader["IsActive"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                }
                reader.Close();
            }
            catch { isFound = false; }
            finally { connection.Close(); }

            return isFound;
        }

        public static bool GetVehicleInfoByPlateNumber(string PlateNumber, ref int VehicleID, ref string ChassisNumber,
            ref string Make, ref string Model, ref int ManufactureYear, ref string Color, ref string FuelType,
            ref int RequiredLicenseClassID, ref int CurrentOwnerPersonID, ref DateTime RegistrationDate,
            ref bool IsActive, ref int CreatedByUserID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Vehicles WHERE PlateNumber = @PlateNumber";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlateNumber", PlateNumber);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    VehicleID = (int)reader["VehicleID"];
                    ChassisNumber = (string)reader["ChassisNumber"];
                    Make = (string)reader["Make"];
                    Model = (string)reader["Model"];
                    ManufactureYear = (int)reader["ManufactureYear"];
                    Color = (string)reader["Color"];
                    FuelType = (string)reader["FuelType"];
                    RequiredLicenseClassID = (int)reader["RequiredLicenseClassID"];
                    CurrentOwnerPersonID = (int)reader["CurrentOwnerPersonID"];
                    RegistrationDate = (DateTime)reader["RegistrationDate"];
                    IsActive = (bool)reader["IsActive"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                }
                reader.Close();
            }
            catch { isFound = false; }
            finally { connection.Close(); }

            return isFound;
        }

        public static bool GetVehicleInfoByChassisNumber(string ChassisNumber, ref int VehicleID, ref string PlateNumber,
            ref string Make, ref string Model, ref int ManufactureYear, ref string Color, ref string FuelType,
            ref int RequiredLicenseClassID, ref int CurrentOwnerPersonID, ref DateTime RegistrationDate,
            ref bool IsActive, ref int CreatedByUserID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Vehicles WHERE ChassisNumber = @ChassisNumber";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChassisNumber", ChassisNumber);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    VehicleID = (int)reader["VehicleID"];
                    PlateNumber = (string)reader["PlateNumber"];
                    Make = (string)reader["Make"];
                    Model = (string)reader["Model"];
                    ManufactureYear = (int)reader["ManufactureYear"];
                    Color = (string)reader["Color"];
                    FuelType = (string)reader["FuelType"];
                    RequiredLicenseClassID = (int)reader["RequiredLicenseClassID"];
                    CurrentOwnerPersonID = (int)reader["CurrentOwnerPersonID"];
                    RegistrationDate = (DateTime)reader["RegistrationDate"];
                    IsActive = (bool)reader["IsActive"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                }
                reader.Close();
            }
            catch { isFound = false; }
            finally { connection.Close(); }

            return isFound;
        }

        public static int AddNewVehicle(string PlateNumber, string ChassisNumber, string Make, string Model,
            int ManufactureYear, string Color, string FuelType, int RequiredLicenseClassID,
            int CurrentOwnerPersonID, DateTime RegistrationDate, bool IsActive, int CreatedByUserID)
        {
            int VehicleID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Vehicles (PlateNumber, ChassisNumber, Make, Model, ManufactureYear, Color, FuelType,
                                                   RequiredLicenseClassID, CurrentOwnerPersonID, RegistrationDate, IsActive, CreatedByUserID)
                             VALUES (@PlateNumber, @ChassisNumber, @Make, @Model, @ManufactureYear, @Color, @FuelType,
                                     @RequiredLicenseClassID, @CurrentOwnerPersonID, @RegistrationDate, @IsActive, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlateNumber", PlateNumber);
            command.Parameters.AddWithValue("@ChassisNumber", ChassisNumber);
            command.Parameters.AddWithValue("@Make", Make);
            command.Parameters.AddWithValue("@Model", Model);
            command.Parameters.AddWithValue("@ManufactureYear", ManufactureYear);
            command.Parameters.AddWithValue("@Color", Color);
            command.Parameters.AddWithValue("@FuelType", FuelType);
            command.Parameters.AddWithValue("@RequiredLicenseClassID", RequiredLicenseClassID);
            command.Parameters.AddWithValue("@CurrentOwnerPersonID", CurrentOwnerPersonID);
            command.Parameters.AddWithValue("@RegistrationDate", RegistrationDate);
            command.Parameters.AddWithValue("@IsActive", IsActive);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    VehicleID = insertedID;
            }
            catch { }
            finally { connection.Close(); }

            return VehicleID;
        }

        public static bool UpdateVehicle(int VehicleID, string PlateNumber, string ChassisNumber, string Make, string Model,
            int ManufactureYear, string Color, string FuelType, int RequiredLicenseClassID,
            int CurrentOwnerPersonID, bool IsActive)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Vehicles SET 
                                PlateNumber = @PlateNumber,
                                ChassisNumber = @ChassisNumber,
                                Make = @Make,
                                Model = @Model,
                                ManufactureYear = @ManufactureYear,
                                Color = @Color,
                                FuelType = @FuelType,
                                RequiredLicenseClassID = @RequiredLicenseClassID,
                                CurrentOwnerPersonID = @CurrentOwnerPersonID,
                                IsActive = @IsActive
                             WHERE VehicleID = @VehicleID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);
            command.Parameters.AddWithValue("@PlateNumber", PlateNumber);
            command.Parameters.AddWithValue("@ChassisNumber", ChassisNumber);
            command.Parameters.AddWithValue("@Make", Make);
            command.Parameters.AddWithValue("@Model", Model);
            command.Parameters.AddWithValue("@ManufactureYear", ManufactureYear);
            command.Parameters.AddWithValue("@Color", Color);
            command.Parameters.AddWithValue("@FuelType", FuelType);
            command.Parameters.AddWithValue("@RequiredLicenseClassID", RequiredLicenseClassID);
            command.Parameters.AddWithValue("@CurrentOwnerPersonID", CurrentOwnerPersonID);
            command.Parameters.AddWithValue("@IsActive", IsActive);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch { return false; }
            finally { connection.Close(); }

            return (rowsAffected > 0);
        }

        public static bool DeleteVehicle(int VehicleID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "DELETE FROM Vehicles WHERE VehicleID = @VehicleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch { return false; }
            finally { connection.Close(); }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllVehicles()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Vehicles_View ORDER BY VehicleID DESC";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                    dt.Load(reader);
                reader.Close();
            }
            catch { }
            finally { connection.Close(); }

            return dt;
        }

        public static bool IsVehicleExistByPlateNumber(string PlateNumber, int ExcludeVehicleID = -1)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found = 1 FROM Vehicles WHERE PlateNumber = @PlateNumber AND VehicleID <> @ExcludeVehicleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PlateNumber", PlateNumber);
            command.Parameters.AddWithValue("@ExcludeVehicleID", ExcludeVehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch { isFound = false; }
            finally { connection.Close(); }

            return isFound;
        }

        public static bool IsVehicleExistByChassisNumber(string ChassisNumber, int ExcludeVehicleID = -1)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found = 1 FROM Vehicles WHERE ChassisNumber = @ChassisNumber AND VehicleID <> @ExcludeVehicleID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ChassisNumber", ChassisNumber);
            command.Parameters.AddWithValue("@ExcludeVehicleID", ExcludeVehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch { isFound = false; }
            finally { connection.Close(); }

            return isFound;
        }
    }
}