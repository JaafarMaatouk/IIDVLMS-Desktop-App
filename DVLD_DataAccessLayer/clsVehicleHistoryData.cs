using System;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsVehicleHistoryData
    {
        public static bool GetVehicleSummaryStats(int VehicleID, ref int TotalOwnersCount,
            ref int TotalViolationsCount, ref int UnpaidViolationsCount,
            ref decimal TotalUnpaidFines, ref decimal TotalPaidFines,
            ref int TotalInspectionsCount, ref DateTime? LastValidInspectionExpiry,
            ref int TotalDetentionsCount, ref bool IsCurrentlyDetained)
        {
            bool success = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"
                SELECT
                    (SELECT COUNT(*) FROM VehicleOwnershipHistory WHERE VehicleID = @VehicleID) AS TotalOwnersCount,
                    (SELECT COUNT(*) FROM TrafficViolations WHERE VehicleID = @VehicleID) AS TotalViolationsCount,
                    (SELECT COUNT(*) FROM TrafficViolations WHERE VehicleID = @VehicleID AND IsPaid = 0) AS UnpaidViolationsCount,
                    (SELECT ISNULL(SUM(FineAmount), 0) FROM TrafficViolations WHERE VehicleID = @VehicleID AND IsPaid = 0) AS TotalUnpaidFines,
                    (SELECT ISNULL(SUM(FineAmount), 0) FROM TrafficViolations WHERE VehicleID = @VehicleID AND IsPaid = 1) AS TotalPaidFines,
                    (SELECT COUNT(*) FROM TechnicalInspections WHERE VehicleID = @VehicleID) AS TotalInspectionsCount,
                    (SELECT MAX(ExpiryDate) FROM TechnicalInspections WHERE VehicleID = @VehicleID AND InspectionResult = 1) AS LastValidInspectionExpiry,
                    (SELECT COUNT(*) FROM DetainedVehicles WHERE VehicleID = @VehicleID) AS TotalDetentionsCount,
                    CASE WHEN EXISTS (SELECT 1 FROM DetainedVehicles WHERE VehicleID = @VehicleID AND IsReleased = 0)
                         THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsCurrentlyDetained";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    success = true;
                    TotalOwnersCount = Convert.ToInt32(reader["TotalOwnersCount"]);
                    TotalViolationsCount = Convert.ToInt32(reader["TotalViolationsCount"]);
                    UnpaidViolationsCount = Convert.ToInt32(reader["UnpaidViolationsCount"]);
                    TotalUnpaidFines = Convert.ToDecimal(reader["TotalUnpaidFines"]);
                    TotalPaidFines = Convert.ToDecimal(reader["TotalPaidFines"]);
                    TotalInspectionsCount = Convert.ToInt32(reader["TotalInspectionsCount"]);
                    LastValidInspectionExpiry = reader["LastValidInspectionExpiry"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["LastValidInspectionExpiry"]);
                    TotalDetentionsCount = Convert.ToInt32(reader["TotalDetentionsCount"]);
                    IsCurrentlyDetained = Convert.ToBoolean(reader["IsCurrentlyDetained"]);
                }
                reader.Close();
            }
            catch { success = false; }
            finally { connection.Close(); }

            return success;
        }
    }
}