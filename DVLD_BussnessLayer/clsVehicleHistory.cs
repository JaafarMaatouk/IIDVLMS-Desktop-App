using System;
using System.Collections.Generic;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsVehicleHistory
    {
        public clsVehicle VehicleInfo { get; set; }

        // ملخص الملاك وعدد مرات البيع
        public int TotalOwnersCount { get; set; }
        public int TotalSalesCount => TotalOwnersCount > 0 ? TotalOwnersCount - 1 : 0;

        // ملخص المخالفات المرورية والغرامات
        public int TotalViolationsCount { get; set; }
        public int UnpaidViolationsCount { get; set; }
        public int PaidViolationsCount => TotalViolationsCount - UnpaidViolationsCount;
        public decimal TotalUnpaidFines { get; set; }
        public decimal TotalPaidFines { get; set; }

        // ملخص الفحص الفني الدوري
        public int TotalInspectionsCount { get; set; }
        public DateTime? LastValidInspectionExpiry { get; set; }
        public bool HasValidInspection => LastValidInspectionExpiry.HasValue && LastValidInspectionExpiry.Value >= DateTime.Now;

        // ملخص الحجز المروري
        public int TotalDetentionsCount { get; set; }
        public bool IsCurrentlyDetained { get; set; }

        // تقييم الجاهزية القانونية للبيع
        public List<string> BlockingIssues { get; set; } = new List<string>();
        public bool IsReadyForSale => BlockingIssues.Count == 0;

        // الجداول التفصيلية الأربعة لتاريخ المركبة
        public DataTable OwnershipHistoryTable { get; set; }
        public DataTable ViolationsHistoryTable { get; set; }
        public DataTable InspectionsHistoryTable { get; set; }
        public DataTable DetentionsHistoryTable { get; set; }

        public static clsVehicleHistory GetFullHistoryByVehicleID(int VehicleID)
        {
            clsVehicle vehicle = clsVehicle.Find(VehicleID);
            if (vehicle == null)
                return null;

            int ownersCount = 0, violationsCount = 0, unpaidCount = 0, inspectionsCount = 0, detentionsCount = 0;
            decimal unpaidFines = 0, paidFines = 0;
            DateTime? lastExpiry = null;
            bool isDetained = false;

            clsVehicleHistoryData.GetVehicleSummaryStats(VehicleID, ref ownersCount, ref violationsCount,
                ref unpaidCount, ref unpaidFines, ref paidFines, ref inspectionsCount,
                ref lastExpiry, ref detentionsCount, ref isDetained);

            clsVehicleHistory history = new clsVehicleHistory
            {
                VehicleInfo = vehicle,
                TotalOwnersCount = ownersCount,
                TotalViolationsCount = violationsCount,
                UnpaidViolationsCount = unpaidCount,
                TotalUnpaidFines = unpaidFines,
                TotalPaidFines = paidFines,
                TotalInspectionsCount = inspectionsCount,
                LastValidInspectionExpiry = lastExpiry,
                TotalDetentionsCount = detentionsCount,
                IsCurrentlyDetained = isDetained,

                OwnershipHistoryTable = clsVehicleOwnership.GetOwnershipHistory(VehicleID),
                ViolationsHistoryTable = clsTrafficViolation.GetVehicleViolations(VehicleID),
                InspectionsHistoryTable = clsTechnicalInspection.GetVehicleInspections(VehicleID),
                DetentionsHistoryTable = clsDetainedVehicle.GetVehicleDetainHistory(VehicleID)
            };

            if (!vehicle.IsActive)
                history.BlockingIssues.Add("المركبة موقوفة (غير فعالة).");
            if (history.IsCurrentlyDetained)
                history.BlockingIssues.Add("المركبة محجوزة حالياً.");
            if (history.UnpaidViolationsCount > 0)
                history.BlockingIssues.Add($"يوجد {history.UnpaidViolationsCount} مخالفة غير مسددة بقيمة {history.TotalUnpaidFines:N2}.");
            if (!history.HasValidInspection)
                history.BlockingIssues.Add("الفحص الفني الدوري منتهي أو غير مسجل.");

            return history;
        }

        public static clsVehicleHistory GetFullHistoryByPlateNumber(string PlateNumber)
        {
            clsVehicle vehicle = clsVehicle.FindByPlateNumber(PlateNumber);
            if (vehicle == null)
                return null;

            return GetFullHistoryByVehicleID(vehicle.VehicleID);
        }
    }
}