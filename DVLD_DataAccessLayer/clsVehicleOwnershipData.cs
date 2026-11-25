using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsVehicleOwnershipData
    {
        public static int AddInitialOwnership(int VehicleID, int OwnerPersonID, DateTime PurchaseDate,
            decimal SalePrice, string Notes, int CreatedByUserID)
        {
            int OwnershipID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO VehicleOwnershipHistory (VehicleID, OwnerPersonID, PurchaseDate, EndDate, SalePrice, IsCurrentOwner, Notes, CreatedByUserID)
                             VALUES (@VehicleID, @OwnerPersonID, @PurchaseDate, NULL, @SalePrice, 1, @Notes, @CreatedByUserID);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@VehicleID", VehicleID);
            command.Parameters.AddWithValue("@OwnerPersonID", OwnerPersonID);
            command.Parameters.AddWithValue("@PurchaseDate", PurchaseDate);
            command.Parameters.AddWithValue("@SalePrice", SalePrice);
            command.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);
            command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    OwnershipID = insertedID;
            }
            catch { }
            finally { connection.Close(); }

            return OwnershipID;
        }

        public static bool ExecuteOwnershipTransfer(int VehicleID, int NewOwnerPersonID, decimal SalePrice,
            string Notes, int CreatedByUserID, ref int NewOwnershipID)
        {
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlTransaction transaction = null;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                string closePrevQuery = @"UPDATE VehicleOwnershipHistory
                                          SET IsCurrentOwner = 0, EndDate = GETDATE()
                                          WHERE VehicleID = @VehicleID AND IsCurrentOwner = 1";
                SqlCommand cmdClose = new SqlCommand(closePrevQuery, connection, transaction);
                cmdClose.Parameters.AddWithValue("@VehicleID", VehicleID);
                cmdClose.ExecuteNonQuery();

                string insertNewQuery = @"INSERT INTO VehicleOwnershipHistory
                                          (VehicleID, OwnerPersonID, PurchaseDate, EndDate, SalePrice, IsCurrentOwner, Notes, CreatedByUserID)
                                          VALUES (@VehicleID, @NewOwnerPersonID, GETDATE(), NULL, @SalePrice, 1, @Notes, @CreatedByUserID);
                                          SELECT SCOPE_IDENTITY();";
                SqlCommand cmdInsert = new SqlCommand(insertNewQuery, connection, transaction);
                cmdInsert.Parameters.AddWithValue("@VehicleID", VehicleID);
                cmdInsert.Parameters.AddWithValue("@NewOwnerPersonID", NewOwnerPersonID);
                cmdInsert.Parameters.AddWithValue("@SalePrice", SalePrice);
                cmdInsert.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(Notes) ? (object)DBNull.Value : Notes);
                cmdInsert.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                object res = cmdInsert.ExecuteScalar();
                if (res != null && int.TryParse(res.ToString(), out int id))
                    NewOwnershipID = id;

                string updateVehicleQuery = @"UPDATE Vehicles SET CurrentOwnerPersonID = @NewOwnerPersonID
                                              WHERE VehicleID = @VehicleID";
                SqlCommand cmdUpdate = new SqlCommand(updateVehicleQuery, connection, transaction);
                cmdUpdate.Parameters.AddWithValue("@NewOwnerPersonID", NewOwnerPersonID);
                cmdUpdate.Parameters.AddWithValue("@VehicleID", VehicleID);
                cmdUpdate.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch
            {
                if (transaction != null)
                    transaction.Rollback();
                return false;
            }
            finally { connection.Close(); }
        }

        public static bool DoesPersonHaveValidLicenseForClass(int PersonID, int RequiredLicenseClassID)
        {
            bool hasValid = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT TOP 1 1
                             FROM Licenses L
                             INNER JOIN Drivers D ON L.DriverID = D.DriverID
                             WHERE D.PersonID = @PersonID
                               AND L.LicenseClass = @RequiredLicenseClassID
                               AND L.IsActive = 1
                               AND L.ExpirationDate >= GETDATE()
                               AND NOT EXISTS (
                                   SELECT 1 FROM DetainedLicenses DL
                                   WHERE DL.LicenseID = L.LicenseID AND DL.IsReleased = 0
                               )";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@RequiredLicenseClassID", RequiredLicenseClassID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                hasValid = reader.HasRows;
                reader.Close();
            }
            catch { hasValid = false; }
            finally { connection.Close(); }

            return hasValid;
        }

        public static DataTable GetOwnershipHistoryByVehicleID(int VehicleID)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                                O.OwnershipID,
                                P.NationalNo,
                                P.FirstName + ' ' + P.SecondName + ' ' + ISNULL(P.ThirdName + ' ', '') + P.LastName AS OwnerFullName,
                                O.PurchaseDate,
                                O.EndDate,
                                O.SalePrice,
                                CASE WHEN O.IsCurrentOwner = 1 THEN 'Current Owner' ELSE 'Previous Owner' END AS OwnershipStatus,
                                O.Notes,
                                U.UserName AS RecordedBy
                             FROM VehicleOwnershipHistory O
                             INNER JOIN People P ON O.OwnerPersonID = P.PersonID
                             INNER JOIN Users U ON O.CreatedByUserID = U.UserID
                             WHERE O.VehicleID = @VehicleID
                             ORDER BY O.PurchaseDate DESC, O.OwnershipID DESC";

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