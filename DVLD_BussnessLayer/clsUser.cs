using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { set; get; }
        public int PersonID { set; get; }
        public clsPerson PersonInfo;
        public int RoleID { set; get; }
        public clsRole RoleInfo;
        public string UserName { set; get; }
        public string Password { set; get; }
        public bool IsActive { set; get; }

        public clsUser()
        {
            this.UserID = -1;
            this.PersonID = -1;
            this.RoleID = 2;
            this.UserName = "";
            this.Password = "";
            this.IsActive = true;
            Mode = enMode.AddNew;
        }

        private clsUser(int UserID, int PersonID, int RoleID, string Username, string Password, bool IsActive)
        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.RoleID = RoleID;
            this.RoleInfo = clsRole.Find(RoleID);
            this.UserName = Username;
            this.Password = Password;
            this.IsActive = IsActive;
            Mode = enMode.Update;
        }

        private bool _AddNewUser()
        {
            this.UserID = clsUserData.AddNewUser(this.PersonID, this.RoleID, this.UserName, this.Password, this.IsActive);
            if (this.UserID != -1)
                this.RoleInfo = clsRole.Find(this.RoleID);
            return (this.UserID != -1);
        }

        private bool _UpdateUser()
        {
            bool updated = clsUserData.UpdateUser(this.UserID, this.PersonID, this.RoleID, this.UserName, this.Password, this.IsActive);
            if (updated)
                this.RoleInfo = clsRole.Find(this.RoleID);
            return updated;
        }

        public static clsUser FindByUserID(int UserID)
        {
            int PersonID = -1, RoleID = -1;
            string UserName = "", Password = "";
            bool IsActive = false;

            if (clsUserData.GetUserInfoByUserID(UserID, ref PersonID, ref RoleID, ref UserName, ref Password, ref IsActive))
                return new clsUser(UserID, PersonID, RoleID, UserName, Password, IsActive);
            else
                return null;
        }

        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1, RoleID = -1;
            string UserName = "", Password = "";
            bool IsActive = false;

            if (clsUserData.GetUserInfoByPersonID(PersonID, ref UserID, ref RoleID, ref UserName, ref Password, ref IsActive))
                return new clsUser(UserID, PersonID, RoleID, UserName, Password, IsActive);
            else
                return null;
        }

        public static clsUser FindByUsernameAndPassword(string UserName, string Password)
        {
            int UserID = -1, PersonID = -1, RoleID = -1;
            bool IsActive = false;

            if (clsUserData.GetUserInfoByUsernameAndPassword(UserName, Password, ref UserID, ref PersonID, ref RoleID, ref IsActive))
                return new clsUser(UserID, PersonID, RoleID, UserName, Password, IsActive);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateUser();
            }
            return false;
        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }

        public static bool DeleteUser(int UserID)
        {
            return clsUserData.DeleteUser(UserID);
        }

        public static bool isUserExist(int UserID)
        {
            return clsUserData.IsUserExist(UserID);
        }

        public static bool isUserExist(string UserName)
        {
            return clsUserData.IsUserExist(UserName);
        }

        public static bool isUserExistForPersonID(int PersonID)
        {
            return clsUserData.IsUserExistForPersonID(PersonID);
        }

        public bool ChangePassword(string NewPassword)
        {
            return clsUserData.ChangePassword(this.UserID, NewPassword);
        }
    }
}