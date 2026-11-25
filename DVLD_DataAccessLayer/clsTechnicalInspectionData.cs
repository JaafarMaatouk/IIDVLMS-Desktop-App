using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsTechnicalInspectionData
    {
        public static int AddNewInspection(int VehicleID, DateTime InspectionDate, DateTime ExpiryDate,
            bool InspectionResult, string TestCenterName, decimal PaidFees, string Notes, int CreatedByUserID)
        {
            int InspectionID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO TechnicalInspections
                             (VehicleID, InspectionDate, ExpiryDate, InspectionResult, TestCenterName, PaidFees, Notes, CreatedByUserID)
                             VALUES (@VehicleID, @InspectionDate, @ExpiryDate, @InspectionResult, @TestCenterName, @PaidFees, @Notes, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);
            command.Parameters.AddWithValue("@InspectionDate", InspectionDate);
            command.Parameters.AddWithValue("@ExpiryDate", ExpiryDate);
            command.Parameters.AddWithValue("@InspectionResult", InspectionResult);
            command.Parameters.AddWithValue("@TestCenterName", TestCenterName);
            command.Parameters.AddWithValue("@PaidFees", PaidFees);
            command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    InspectionID = insertedID;
            }
            catch { }
            finally { connection.Close(); }

            return InspectionID;
        }

        public static bool HasValidTechnicalInspection(int VehicleID)
        {
            bool isValid = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT TOP 1 1 FROM TechnicalInspections
                             WHERE VehicleID = @VehicleID
                               AND InspectionResult = 1
                               AND ExpiryDate >= GETDATE()
                             ORDER BY ExpiryDate DESC";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isValid = reader.HasRows;
                reader.Close();
            }
            catch { isValid = false; }
            finally { connection.Close(); }

            return isValid;
        }

        public static DataTable GetInspectionsByVehicleID(int VehicleID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                                TI.InspectionID,
                                TI.InspectionDate,
                                TI.ExpiryDate,
                                CASE WHEN TI.InspectionResult = 1 THEN 'Passed' ELSE 'Failed' END AS ResultStatus,
                                CASE WHEN TI.InspectionResult = 1 AND TI.ExpiryDate >= GETDATE() THEN 'Valid' ELSE 'Expired / Invalid' END AS Validity,
                                TI.TestCenterName,
                                TI.PaidFees,
                                TI.Notes,
                                U.UserName AS RecordedBy
                             FROM TechnicalInspections TI
                             INNER JOIN Users U ON TI.CreatedByUserID = U.UserID
                             WHERE TI.VehicleID = @VehicleID
                             ORDER BY TI.InspectionDate DESC, TI.InspectionID DESC";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

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
    }
}