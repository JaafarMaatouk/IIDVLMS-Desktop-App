using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsTechnicalInspection
    {
        public int InspectionID { get; set; }
        public int VehicleID { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool InspectionResult { get; set; }
        public string TestCenterName { get; set; }
        public decimal PaidFees { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }

        public clsTechnicalInspection()
        {
            InspectionID = -1;
            VehicleID = -1;
            InspectionDate = DateTime.Now;
            ExpiryDate = DateTime.Now.AddYears(1);
            InspectionResult = true;
            TestCenterName = "";
            PaidFees = 0;
            Notes = "";
            CreatedByUserID = -1;
        }

        public bool AddNew()
        {
            InspectionID = clsTechnicalInspectionData.AddNewInspection(
                VehicleID, InspectionDate, ExpiryDate, InspectionResult, TestCenterName, PaidFees, Notes, CreatedByUserID);
            return (InspectionID != -1);
        }

        public static bool HasValidTechnicalInspection(int VehicleID) => clsTechnicalInspectionData.HasValidTechnicalInspection(VehicleID);
        public static DataTable GetVehicleInspections(int VehicleID) => clsTechnicalInspectionData.GetInspectionsByVehicleID(VehicleID);
    }
}