
namespace POVWebDomain.Models.API.StoreSrv.SettingMgmt.SystemSetting
{
    public class SystemSettings
    {
        public bool EnableForeignCurrency { get; set; }
        public int DecimalPlaces { get; set; }
        public string DualScreenDisplay { get; set; }
        public bool SaleMemberLoadOption { get; set; }
        public int PointsPerDollar { get; set; }
        public decimal MinimumSpend { get; set; }
        public int MaxRedemptionPercentage { get; set; }
        public bool RefundRestoreUsedPoints { get; set; }
        public bool RefundDeductEarnedPoints { get; set; }

        public bool LocalStoreOnly { get; set; }

        // �O�_�n��ƶq
        public bool CountGeneralProduct { get; set; }
        public bool CountGiftCertificate { get; set; }
        public bool CountPoints { get; set; }
        public bool CountStoredValue { get; set; }
        public bool CountPackagingMaterial { get; set; }
        public bool CountDiscountCoupon { get; set; }
        
        public bool AllowPriceEdit { get; set; }
        public bool StockCheck { get; set; }

        // �Τ�����v��
        public bool CostPermission { get; set; }

    }

    public class SystemSettingsEditable
    {
        public string DualScreenDisplay { get; set; }
        public bool SaleMemberLoadOption { get; set; }
        public int PointsPerDollar { get; set; }
        public decimal MinimumSpend { get; set; }
        public int MaxRedemptionPercentage { get; set; }
        public bool RefundRestoreUsedPoints { get; set; }
        public bool CountPackagingMaterial { get; set; }
        public bool EnableForeignCurrency { get; set; }
        public bool LocalStoreOnly { get; set; }
        public string UserID { get; set; }
    }

    public class GetSettingsRequest
    {
        public string UserID { get; set; }
    }

}
