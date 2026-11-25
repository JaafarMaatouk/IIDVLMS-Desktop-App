using System;
using System.Collections.Generic;
using System.Data;
using DVLD_DataAccessLayer;

namespace DVLD_BussnessLayer
{
    public class clsVehicleOwnership
    {
        public static List<string> ValidateOwnershipTransfer(int VehicleID, int NewOwnerPersonID)
        {
            List<string> errors = new List<string>();
            clsVehicle vehicle = clsVehicle.Find(VehicleID);

            if (vehicle == null)
            {
                errors.Add("المركبة المطلوبة غير موجودة في النظام.");
                return errors;
            }

            // القيد 1: ألا يكون المشتري هو المالك الحالي نفسه
            if (vehicle.CurrentOwnerPersonID == NewOwnerPersonID)
                errors.Add("القيد الأول: المشتري المختار هو المالك الحالي للمركبة بالفعل.");

            // القيد 2: ألا تكون المركبة محجوزة أو موقوفة
            if (!vehicle.IsActive)
                errors.Add("القيد الثاني: سجل المركبة غير فعال حالياً.");
            if (clsDetainedVehicle.IsVehicleDetained(VehicleID))
                errors.Add("القيد الثاني: المركبة محجوزة حالياً ولا يمكن نقل ملكيتها قبل فك الحجز.");

            // القيد 3: خلو المركبة من المخالفات المرورية غير المسددة
            if (clsTrafficViolation.HasUnpaidViolations(VehicleID))
                errors.Add("القيد الثالث: توجد مخالفات مرورية غير مسددة على المركبة؛ يجب تسديدها أولاً.");

            // القيد 4: وجود فحص فني دوري ساري المفعول
            if (!clsTechnicalInspection.HasValidTechnicalInspection(VehicleID))
                errors.Add("القيد الرابع: الفحص الفني الدوري للمركبة منتهي الصلاحية أو غير موجود.");

            // القيد 5: امتلاك المشتري رخصة قيادة سارية المفعول مطابقة لفئة المركبة
            if (!clsVehicleOwnershipData.DoesPersonHaveValidLicenseForClass(NewOwnerPersonID, vehicle.RequiredLicenseClassID))
                errors.Add("القيد الخامس: المشتري لا يمتلك رخصة قيادة فعالة وسارية المفعول من الفئة المطابقة لنوع المركبة.");

            return errors;
        }

        public static bool TransferOwnership(int VehicleID, int NewOwnerPersonID, decimal SalePrice,
            string Notes, int CreatedByUserID, out string ValidationMessage)
        {
            List<string> errors = ValidateOwnershipTransfer(VehicleID, NewOwnerPersonID);
            if (errors.Count > 0)
            {
                ValidationMessage = string.Join(Environment.NewLine, errors);
                return false;
            }

            int newOwnershipID = -1;
            bool transferred = clsVehicleOwnershipData.ExecuteOwnershipTransfer(
                VehicleID, NewOwnerPersonID, SalePrice, Notes, CreatedByUserID, ref newOwnershipID);

            ValidationMessage = transferred
                ? "تمت عملية نقل الملكية وتحديث سجل الملاك بنجاح."
                : "حدث خطأ أثناء تنفيذ عملية نقل الملكية في قاعدة البيانات.";

            return transferred;
        }

        public static DataTable GetOwnershipHistory(int VehicleID) => clsVehicleOwnershipData.GetOwnershipHistoryByVehicleID(VehicleID);
    }
}