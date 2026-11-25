using System;
using System.Data;
using System.Data.SqlClient;

namespace DVLD_DataAccessLayer
{
    public class clsPersonData
    {
        public static bool GetPersonInfoByID(int PersonID, ref string FirstName, ref string SecondName,
          ref string ThirdName, ref string LastName, ref string NationalNo, ref DateTime DateOfBirth,
           ref short Gendor, ref string Address, ref string Phone, ref string Email,
           ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    FirstName = (string)reader["FirstName"];
                    SecondName = (string)reader["SecondName"];
                    ThirdName = reader["ThirdName"] != DBNull.Value ? (string)reader["ThirdName"] : "";
                    LastName = (string)reader["LastName"];
                    NationalNo = (string)reader["NationalNo"];
                    DateOfBirth = (DateTime)reader["DateOfBirth"];
                    Gendor = Convert.ToInt16(reader["Gender"]);
                    Address = (string)reader["Address"];
                    Phone = (string)reader["Phone"];
                    Email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : "";
                    NationalityCountryID = (int)reader["GovernorateID"];
                    ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
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

        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
            ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
             ref short Gendor, ref string Address, ref string Phone, ref string Email,
             ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM People WHERE NationalNo = @NationalNo";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    isFound = true;
                    PersonID = (int)reader["PersonID"];
                    FirstName = (string)reader["FirstName"];
                    SecondName = (string)reader["SecondName"];
                    ThirdName = reader["ThirdName"] != DBNull.Value ? (string)reader["ThirdName"] : "";
                    LastName = (string)reader["LastName"];
                    DateOfBirth = (DateTime)reader["DateOfBirth"];
                    Gendor = Convert.ToInt16(reader["Gender"]);
                    Address = (string)reader["Address"];
                    Phone = (string)reader["Phone"];
                    Email = reader["Email"] != DBNull.Value ? (string)reader["Email"] : "";
                    NationalityCountryID = (int)reader["GovernorateID"];
                    ImagePath = reader["ImagePath"] != DBNull.Value ? (string)reader["ImagePath"] : "";
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

        public static int AddNewPerson(string FirstName, string SecondName,
           string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
           short Gendor, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {
            int PersonID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO People (FirstName, SecondName, ThirdName, LastName, NationalNo,
                                                 DateOfBirth, Gender, Address, Phone, Email, GovernorateID, ImagePath)
                             VALUES (@FirstName, @SecondName, @ThirdName, @LastName, @NationalNo,
                                     @DateOfBirth, @Gender, @Address, @Phone, @Email, @GovernorateID, @ImagePath);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gendor);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email);
            command.Parameters.AddWithValue("@GovernorateID", NationalityCountryID);
            command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    PersonID = insertedID;
            }
            catch (Exception ex) { }
            finally
            {
                connection.Close();
            }

            return PersonID;
        }

        public static bool UpdatePerson(int PersonID, string FirstName, string SecondName,
           string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
           short Gendor, string Address, string Phone, string Email,
            int NationalityCountryID, string ImagePath)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE People
                             SET FirstName = @FirstName,
                                 SecondName = @SecondName,
                                 ThirdName = @ThirdName,
                                 LastName = @LastName,
                                 NationalNo = @NationalNo,
                                 DateOfBirth = @DateOfBirth,
                                 Gender = @Gender,
                                 Address = @Address,
                                 Phone = @Phone,
                                 Email = @Email,
                                 GovernorateID = @GovernorateID,
                                 ImagePath = @ImagePath
                             WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gendor);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email);
            command.Parameters.AddWithValue("@GovernorateID", NationalityCountryID);
            command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT People.PersonID, People.NationalNo,
                             People.FirstName, People.SecondName, People.ThirdName, People.LastName,
                             People.DateOfBirth, People.Gender AS Gendor,
                             CASE WHEN People.Gender = 0 THEN 'Male' ELSE 'Female' END AS GendorCaption,
                             People.Address, People.Phone, People.Email,
                             People.GovernorateID AS NationalityCountryID,
                             Governorates.GovernorateName AS CountryName, People.ImagePath
                             FROM People
                             INNER JOIN Governorates ON People.GovernorateID = Governorates.GovernorateID
                             ORDER BY People.FirstName";

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

        public static bool DeletePerson(int PersonID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "DELETE FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { }
            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }

        public static bool IsPersonExist(int PersonID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM People WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
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

        public static bool IsPersonExist(string NationalNo)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 FROM People WHERE NationalNo = @NationalNo";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
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
    }
}