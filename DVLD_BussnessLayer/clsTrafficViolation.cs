using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsTrafficViolation
    {
        public int ViolationID { get; set; }
        public int VehicleID { get; set; }
        public int? DriverID { get; set; }
        public DateTime ViolationDate { get; set; }
        public string ViolationType { get; set; }
        public string Location { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }

        public clsTrafficViolation()
        {
            ViolationID = -1;
            VehicleID = -1;
            DriverID = null;
            ViolationDate = DateTime.Now;
            ViolationType = "";
            Location = "";
            FineAmount = 0;
            IsPaid = false;
            PaymentDate = null;
            Notes = "";
            CreatedByUserID = -1;
        }

        public bool AddNew()
        {
            ViolationID = clsTrafficViolationData.AddNewViolation(
                VehicleID, DriverID, ViolationDate, ViolationType, Location, FineAmount, Notes, CreatedByUserID);
            return (ViolationID != -1);
        }

        public static bool PayViolation(int ViolationID) => clsTrafficViolationData.PayViolation(ViolationID);
        public static bool HasUnpaidViolations(int VehicleID) => clsTrafficViolationData.HasUnpaidViolations(VehicleID);
        public static DataTable GetVehicleViolations(int VehicleID) => clsTrafficViolationData.GetViolationsByVehicleID(VehicleID);
        public static DataTable GetAllViolations() => clsTrafficViolationData.GetAllViolations();
    }
}