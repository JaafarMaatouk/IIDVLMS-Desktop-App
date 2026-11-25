using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsDetainedVehicleData
    {
        public static int DetainVehicle(int VehicleID, DateTime DetainDate, string DetainReason,
            decimal FineFees, int DetainedByUserID, string Notes)
        {
            int DetainID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO DetainedVehicles
                             (VehicleID, DetainDate, DetainReason, FineFees, DetainedByUserID, IsReleased, ReleaseDate, ReleasedByUserID, Notes)
                             VALUES (@VehicleID, @DetainDate, @DetainReason, @FineFees, @DetainedByUserID, 0, NULL, NULL, @Notes);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);
            command.Parameters.AddWithValue("@DetainDate", DetainDate);
            command.Parameters.AddWithValue("@DetainReason", DetainReason);
            command.Parameters.AddWithValue("@FineFees", FineFees);
            command.Parameters.AddWithValue("@DetainedByUserID", DetainedByUserID);
            command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    DetainID = insertedID;
            }
            catch { }
            finally { connection.Close(); }

            return DetainID;
        }

        public static bool ReleaseVehicle(int DetainID, int ReleasedByUserID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE DetainedVehicles
                             SET IsReleased = 1, ReleaseDate = GETDATE(), ReleasedByUserID = @ReleasedByUserID
                             WHERE DetainID = @DetainID AND IsReleased = 0";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DetainID", DetainID);
            command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch { return false; }
            finally { connection.Close(); }

            return (rowsAffected > 0);
        }

        public static bool IsVehicleDetained(int VehicleID)
        {
            bool isDetained = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT TOP 1 1 FROM DetainedVehicles WHERE VehicleID = @VehicleID AND IsReleased = 0";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isDetained = reader.HasRows;
                reader.Close();
            }
            catch { isDetained = false; }
            finally { connection.Close(); }

            return isDetained;
        }

        public static int GetActiveDetainIDByVehicleID(int VehicleID)
        {
            int detainID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT TOP 1 DetainID FROM DetainedVehicles WHERE VehicleID = @VehicleID AND IsReleased = 0 ORDER BY DetainID DESC";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                object res = command.ExecuteScalar();
                if (res != null && int.TryParse(res.ToString(), out int id))
                    detainID = id;
            }
            catch { }
            finally { connection.Close(); }

            return detainID;
        }

        public static DataTable GetDetainHistoryByVehicleID(int VehicleID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                                DV.DetainID,
                                DV.DetainDate,
                                DV.DetainReason,
                                DV.FineFees,
                                CASE WHEN DV.IsReleased = 1 THEN 'Released' ELSE 'Currently Detained' END AS Status,
                                DV.ReleaseDate,
                                U1.UserName AS DetainedBy,
                                U2.UserName AS ReleasedBy,
                                DV.Notes
                             FROM DetainedVehicles DV
                             INNER JOIN Users U1 ON DV.DetainedByUserID = U1.UserID
                             LEFT JOIN Users U2 ON DV.ReleasedByUserID = U2.UserID
                             WHERE DV.VehicleID = @VehicleID
                             ORDER BY DV.DetainDate DESC, DV.DetainID DESC";

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

        public static DataTable GetAllDetainedVehicles()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                                DV.DetainID,
                                DV.VehicleID,
                                V.PlateNumber,
                                P.FirstName + ' ' + P.SecondName + ' ' + P.LastName AS OwnerName,
                                DV.DetainDate,
                                DV.DetainReason,
                                DV.FineFees,
                                CASE WHEN DV.IsReleased = 1 THEN 'Released' ELSE 'Detained' END AS Status,
                                DV.ReleaseDate
                             FROM DetainedVehicles DV
                             INNER JOIN Vehicles V ON DV.VehicleID = V.VehicleID
                             INNER JOIN People P ON V.CurrentOwnerPersonID = P.PersonID
                             ORDER BY DV.DetainDate DESC";

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