using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsTrafficViolationData
    {
        public static int AddNewViolation(int VehicleID, int? DriverID, DateTime ViolationDate,
            string ViolationType, string Location, decimal FineAmount, string Notes, int CreatedByUserID)
        {
            int ViolationID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO TrafficViolations
                             (VehicleID, DriverID, ViolationDate, ViolationType, Location, FineAmount, IsPaid, PaymentDate, Notes, CreatedByUserID)
                             VALUES (@VehicleID, @DriverID, @ViolationDate, @ViolationType, @Location, @FineAmount, 0, NULL, @Notes, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);
            command.Parameters.AddWithValue("@DriverID", DriverID.HasValue ? (object)DriverID.Value : DBNull.Value);
            command.Parameters.AddWithValue("@ViolationDate", ViolationDate);
            command.Parameters.AddWithValue("@ViolationType", ViolationType);
            command.Parameters.AddWithValue("@Location", Location);
            command.Parameters.AddWithValue("@FineAmount", FineAmount);
            command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    ViolationID = insertedID;
            }
            catch { }
            finally { connection.Close(); }

            return ViolationID;
        }

        public static bool PayViolation(int ViolationID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE TrafficViolations
                             SET IsPaid = 1, PaymentDate = GETDATE()
                             WHERE ViolationID = @ViolationID AND IsPaid = 0";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ViolationID", ViolationID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch { return false; }
            finally { connection.Close(); }

            return (rowsAffected > 0);
        }

        public static bool HasUnpaidViolations(int VehicleID)
        {
            bool hasUnpaid = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT TOP 1 1 FROM TrafficViolations WHERE VehicleID = @VehicleID AND IsPaid = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                hasUnpaid = reader.HasRows;
                reader.Close();
            }
            catch { hasUnpaid = false; }
            finally { connection.Close(); }

            return hasUnpaid;
        }

        public static DataTable GetViolationsByVehicleID(int VehicleID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                                TV.ViolationID,
                                V.PlateNumber,
                                TV.ViolationDate,
                                TV.ViolationType,
                                TV.Location,
                                TV.FineAmount,
                                CASE WHEN TV.IsPaid = 1 THEN 'Paid' ELSE 'Unpaid' END AS PaymentStatus,
                                TV.PaymentDate,
                                TV.Notes
                             FROM TrafficViolations TV
                             INNER JOIN Vehicles V ON TV.VehicleID = V.VehicleID
                             WHERE TV.VehicleID = @VehicleID
                             ORDER BY TV.ViolationDate DESC, TV.ViolationID DESC";

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

        public static DataTable GetAllViolations()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                                TV.ViolationID,
                                V.PlateNumber,
                                P.FirstName + ' ' + P.SecondName + ' ' + P.LastName AS OwnerName,
                                TV.ViolationDate,
                                TV.ViolationType,
                                TV.Location,
                                TV.FineAmount,
                                CASE WHEN TV.IsPaid = 1 THEN 'Paid' ELSE 'Unpaid' END AS PaymentStatus,
                                TV.PaymentDate
                             FROM TrafficViolations TV
                             INNER JOIN Vehicles V ON TV.VehicleID = V.VehicleID
                             INNER JOIN People P ON V.CurrentOwnerPersonID = P.PersonID
                             ORDER BY TV.ViolationDate DESC";

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
    }
}