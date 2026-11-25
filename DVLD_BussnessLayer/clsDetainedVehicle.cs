using System;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsDetainedVehicle
    {
        public int DetainID { get; set; }
        public int VehicleID { get; set; }
        public DateTime DetainDate { get; set; }
        public string DetainReason { get; set; }
        public decimal FineFees { get; set; }
        public int DetainedByUserID { get; set; }
        public bool IsReleased { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? ReleasedByUserID { get; set; }
        public string Notes { get; set; }

        public clsDetainedVehicle()
        {
            DetainID = -1;
            VehicleID = -1;
            DetainDate = DateTime.Now;
            DetainReason = "";
            FineFees = 0;
            DetainedByUserID = -1;
            IsReleased = false;
            ReleaseDate = null;
            ReleasedByUserID = null;
            Notes = "";
        }

        public bool Detain()
        {
            if (IsVehicleDetained(VehicleID))
                return false;

            DetainID = clsDetainedVehicleData.DetainVehicle(
                VehicleID, DetainDate, DetainReason, FineFees, DetainedByUserID, Notes);
            return (DetainID != -1);
        }

        public static bool ReleaseVehicleByVehicleID(int VehicleID, int ReleasedByUserID)
        {
            int activeDetainID = clsDetainedVehicleData.GetActiveDetainIDByVehicleID(VehicleID);
            if (activeDetainID == -1)
                return false;

            return clsDetainedVehicleData.ReleaseVehicle(activeDetainID, ReleasedByUserID);
        }

        public static bool ReleaseByDetainID(int DetainID, int ReleasedByUserID) => clsDetainedVehicleData.ReleaseVehicle(DetainID, ReleasedByUserID);
        public static bool IsVehicleDetained(int VehicleID) => clsDetainedVehicleData.IsVehicleDetained(VehicleID);
        public static int GetActiveDetainID(int VehicleID) => clsDetainedVehicleData.GetActiveDetainIDByVehicleID(VehicleID);
        public static DataTable GetVehicleDetainHistory(int VehicleID) => clsDetainedVehicleData.GetDetainHistoryByVehicleID(VehicleID);
        public static DataTable GetAllDetainedVehicles() => clsDetainedVehicleData.GetAllDetainedVehicles();
    }
}