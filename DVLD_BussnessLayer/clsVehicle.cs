using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsVehicle
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int VehicleID { get; set; }
        public string PlateNumber { get; set; }
        public string ChassisNumber { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }
        public string Color { get; set; }
        public string FuelType { get; set; }
        public int RequiredLicenseClassID { get; set; }
        public int CurrentOwnerPersonID { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserID { get; set; }

        public clsPerson OwnerInfo { get; set; }
        public clsLicenseClass LicenseClassInfo { get; set; }

        public clsVehicle()
        {
            VehicleID = -1;
            PlateNumber = "";
            ChassisNumber = "";
            Make = "";
            Model = "";
            ManufactureYear = DateTime.Now.Year;
            Color = "";
            FuelType = "Gasoline";
            RequiredLicenseClassID = 3;
            CurrentOwnerPersonID = -1;
            RegistrationDate = DateTime.Now;
            IsActive = true;
            CreatedByUserID = -1;
            Mode = enMode.AddNew;
        }

        private clsVehicle(int vehicleID, string plateNumber, string chassisNumber, string make, string model,
            int manufactureYear, string color, string fuelType, int requiredLicenseClassID,
            int currentOwnerPersonID, DateTime registrationDate, bool isActive, int createdByUserID)
        {
            VehicleID = vehicleID;
            PlateNumber = plateNumber;
            ChassisNumber = chassisNumber;
            Make = make;
            Model = model;
            ManufactureYear = manufactureYear;
            Color = color;
            FuelType = fuelType;
            RequiredLicenseClassID = requiredLicenseClassID;
            CurrentOwnerPersonID = currentOwnerPersonID;
            RegistrationDate = registrationDate;
            IsActive = isActive;
            CreatedByUserID = createdByUserID;

            OwnerInfo = clsPerson.Find(CurrentOwnerPersonID);
            LicenseClassInfo = clsLicenseClass.Find(RequiredLicenseClassID);
            Mode = enMode.Update;
        }

        public static clsVehicle Find(int VehicleID)
        {
            string plate = "", chassis = "", make = "", model = "", color = "", fuel = "";
            int year = 0, classID = -1, ownerID = -1, userID = -1;
            DateTime regDate = DateTime.Now;
            bool isActive = false;

            if (clsVehicleData.GetVehicleInfoByID(VehicleID, ref plate, ref chassis, ref make, ref model,
                ref year, ref color, ref fuel, ref classID, ref ownerID, ref regDate, ref isActive, ref userID))
            {
                return new clsVehicle(VehicleID, plate, chassis, make, model, year, color, fuel,
                    classID, ownerID, regDate, isActive, userID);
            }
            return null;
        }

        public static clsVehicle FindByPlateNumber(string PlateNumber)
        {
            int vehicleID = -1, year = 0, classID = -1, ownerID = -1, userID = -1;
            string chassis = "", make = "", model = "", color = "", fuel = "";
            DateTime regDate = DateTime.Now;
            bool isActive = false;

            if (clsVehicleData.GetVehicleInfoByPlateNumber(PlateNumber, ref vehicleID, ref chassis, ref make, ref model,
                ref year, ref color, ref fuel, ref classID, ref ownerID, ref regDate, ref isActive, ref userID))
            {
                return new clsVehicle(vehicleID, PlateNumber, chassis, make, model, year, color, fuel,
                    classID, ownerID, regDate, isActive, userID);
            }
            return null;
        }

        public static clsVehicle FindByChassisNumber(string ChassisNumber)
        {
            int vehicleID = -1, year = 0, classID = -1, ownerID = -1, userID = -1;
            string plate = "", make = "", model = "", color = "", fuel = "";
            DateTime regDate = DateTime.Now;
            bool isActive = false;

            if (clsVehicleData.GetVehicleInfoByChassisNumber(ChassisNumber, ref vehicleID, ref plate, ref make, ref model,
                ref year, ref color, ref fuel, ref classID, ref ownerID, ref regDate, ref isActive, ref userID))
            {
                return new clsVehicle(vehicleID, plate, ChassisNumber, make, model, year, color, fuel,
                    classID, ownerID, regDate, isActive, userID);
            }
            return null;
        }

        private bool _AddNewVehicle()
        {
            VehicleID = clsVehicleData.AddNewVehicle(PlateNumber, ChassisNumber, Make, Model, ManufactureYear,
                Color, FuelType, RequiredLicenseClassID, CurrentOwnerPersonID, RegistrationDate, IsActive, CreatedByUserID);

            if (VehicleID != -1)
            {
                clsVehicleOwnershipData.AddInitialOwnership(VehicleID, CurrentOwnerPersonID, RegistrationDate,
                    0, "Initial Vehicle Registration", CreatedByUserID);
                return true;
            }
            return false;
        }

        private bool _UpdateVehicle()
        {
            return clsVehicleData.UpdateVehicle(VehicleID, PlateNumber, ChassisNumber, Make, Model, ManufactureYear,
                Color, FuelType, RequiredLicenseClassID, CurrentOwnerPersonID, IsActive);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewVehicle())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;
                case enMode.Update:
                    return _UpdateVehicle();
            }
            return false;
        }

        public static bool DeleteVehicle(int VehicleID) => clsVehicleData.DeleteVehicle(VehicleID);
        public static DataTable GetAllVehicles() => clsVehicleData.GetAllVehicles();
        public static bool IsPlateNumberExist(string PlateNumber, int ExcludeVehicleID = -1) => clsVehicleData.IsVehicleExistByPlateNumber(PlateNumber, ExcludeVehicleID);
        public static bool IsChassisNumberExist(string ChassisNumber, int ExcludeVehicleID = -1) => clsVehicleData.IsVehicleExistByChassisNumber(ChassisNumber, ExcludeVehicleID);
    }
}