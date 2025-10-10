using POVWebDomain.Models.API.StoreSrv.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.EcommerceMgmt.EcommerceStore
{
    public class AllStore
    {
        public string StoreTag { get; set; }
    }
    public class GetEcommerceStoreDataRequest
    {
        public List<AllStore> Tag { get; set; }
    }

    public class GetSingleDataRequest
    {
        public string StoreTag { get; set; }
        public string StoreID { get; set; }
    }
    public class AddEcommerceStoreDataRequest
    {
        public string StoreTag { get; set; }
        public string StoreNumber { get; set; }
        public string ApiKey { get; set; }
        public string HostAddress { get; set; }
        public string StoreAlias { get; set; }
        public string StoreName { get; set; }
        public string ShippingWareHouse { get; set; }
        public string SellBranch { get; set; }
        public string EBackStage { get; set; }
        public string CustomerServiceMail { get; set; }
        public string CustomerServicePhone { get; set; }
        public string EOfficialAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string ZipID01 { get; set; }
        public string DiscountActivity { get; set; }
        public string HongLi { get; set; }
        public string DiscountCoupon { get; set; }
        public string ShippingCost { get; set; }
        public string AddDetail { get; set; }
        public string SevenCode { get; set; }
        public string PrintImage { get; set; }
        public string EStoreID { get; set; }
        public string TaxCategory { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
        public string DeliveryWay { get; set; }
        public string OldID { get; set; }
        public bool IsEcApiEnabled { get; set; }
    }

    public class UpdEcommerceStoreDataRequest
    {
        public string OriginalStoreID { get; set; }
        public string StoreTag { get; set; }
        public string StoreNumber { get; set; }
        public string ApiKey { get; set; }
        public string HostAddress { get; set; }
        public string StoreAlias { get; set; }
        public string StoreName { get; set; }
        public string ShippingWareHouse { get; set; }
        public string SellBranch { get; set; }
        public string EBackStage { get; set; }
        public string CustomerServiceMail { get; set; }
        public string CustomerServicePhone { get; set; }
        public string EOfficialAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string ZipID01 { get; set; }
        public string DiscountActivity { get; set; }
        public string HongLi { get; set; }
        public string DiscountCoupon { get; set; }
        public string ShippingCost { get; set; }
        public string AddDetail { get; set; }
        public string SevenCode { get; set; }
        public string PrintImage { get; set; }
        public string TaxCategory { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
        public string DeliveryWay { get; set; }
        public bool IsEcApiEnabled { get; set; }
    }

    /*public class ID
    {
        public string AnnouncementID { get; set; }
    }*/
    public class DelEcommerceStoreDataRequest
    {
        public string StoreTag { get; set; }
        public string StoreNumber { get; set; }
        public string StoreName { get; set; }
    }

    public class GetIDRequest
    {
        public string StoreID { get; set; }
    }

    public class GetEStoreImageRequest
    {
        public string EStoreName { get; set; }
        public string EStoreID { get; set; }
        public string EStoreImage {  get; set; }
    }

    public class ECStore
    {
        public string EStoreID { get; set; }
        //91app
        public bool IsRestricted_91 { get; set; }
        public string SoldOutActionType_91 { get; set; }
        public string Status_91 { get; set; }
        public bool IsShowPurchaseList_91 { get; set; }
        public bool IsShowSoldQty_91 { get; set; }
        public bool IsShowStockQty_91 { get; set; }
        //momoo
        public string GoodsType_Momo { get; set; }
        public bool IsECWarehouse_Momo { get; set; }
        public bool HasAs_Momo { get; set; }
        public bool IsCommission_Momo { get; set; }
        public bool IsAcceptTravelCard_Momo { get; set; }
        public string OutplaceSeq_Momo { get; set; }
        public string OutplaceSeqRtn_Momo { get; set; }
        public bool IsIncludeInstall_Momo { get; set; }
        public bool LiveStreamYn_Momo { get; set; }
        //yahoo
        public string ContentRating_Yahoo { get; set; }
        public string ProductWarrantlyPeriod_Yahoo { get; set; }
        public string ProductWarrantlyScope_Yahoo { get; set; }
        public string ProductWarrantlyHandler_Yahoo { get; set; }
        public string ProductWarrantlyDescription_Yahoo { get; set; }
        public bool IsInstallRequired_Yahoo { get; set; }
        public bool IsLargeVolumnProductGift_Yahoo { get; set; }
        public bool IsNeedRecycle_Yahoo { get; set; }
        public bool IsOutrightPurchase_Yahoo { get; set; }
        //shopee
        public string Condition_Shopee { get; set; }
        public string DescriptionType_Shopee { get; set; }


    }

    public class GetECStoreRequest
    {
        public string EStoreID { get; set; }
    }
    public class UpdECStoreRequest : ECStore { }

    public class GetOptionAllSettingRequest { }
}
