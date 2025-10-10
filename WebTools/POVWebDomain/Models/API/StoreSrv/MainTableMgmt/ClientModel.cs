using POVWebDomain.Models.API.StoreSrv.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POVWebDomain.Models.API.StoreSrv.MainTableMgmt.Client
{
    public class RequireWhenClientIDAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var newClient = (AddClientRequest)validationContext.ObjectInstance;
            string clientID = newClient.ClientID;
            string storeID = newClient.StoreID;
            if (clientID.Length >= 1 && clientID[0] == '0' && storeID.Length < 1)
                return new ValidationResult("請輸入分店代號");
            else
                return ValidationResult.Success;

        }
    }
    public class AddClientRequest
    {

        private string _clientID;
        [Required(ErrorMessage = "請輸入客戶代號")]
        public string ClientID { get { return _clientID; } set { _clientID = value?.Trim(); } }

        private string _storeID;
        [RequireWhenClientID]
        public string StoreID { get { return _storeID; } set { _storeID = value?.Trim(); } }
        public string CatenationID { get; set; }
        public string ClientName { get; set; }
        public string ClientShort { get; set; }
        public string AccountAddr04 { get; set; }
        public string SendAddress04 { get; set; }
        public string Class { get; set; }
        public string UniteID { get; set; }
        public string InvoiceName { get; set; }
        public string Principal { get; set; }
        public string ContactPerson { get; set; }
        public string TelPhon01 { get; set; }
        public string TelPhon02 { get; set; }
        public string TelPhon03 { get; set; }
        public string Fax { get; set; }

        public string InvoicePost { get; set; }
        public string InvoiceAddr01 { get; set; }
        public string AccountPost { get; set; }
        public string AccountAddr01 { get; set; }
        public string SendPost { get; set; }
        public string SendAddress01 { get; set; }
        public string Email { get; set; }
        public float AreaNum { get; set; }
        public float Discount { get; set; }
        public float PersonNum { get; set; }
        public float CreditLimit { get; set; }
        public string Display { get; set; }
        public float TaxRate { get; set; }
        public string OpenDate { get; set; }
        public string Remark01 { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
        public string Operation { get; set; }
    }
    public class DelClientRequest
    {
        [Required(ErrorMessage = "請輸入客戶代號")]
        public string ClientID { get; set; }
    }
    public class GetClientCheckRequest
    {
        public string ClientID { get; set; }
    }

    public class GetClientCheckResponse
    {
        public string ClientID { get; set; }
        public string ClientShort { get; set; }
    }
    public class GetClientDetailByIDRequest
    {
        [Required(ErrorMessage = "請輸入客戶代號")]
        public string ClientID { get; set; }
    }
    public class GetClientHelpRequest
    {
        public string ClientID_ClientShort_ClientName_Like { get; set; }
        public string ClientID_NotDisplay { get; set; }
        public string OrderBy { get; set; }
    }

    public class GetClientHelpResponse
    {
        public string ClientID { get; set; }
        public string ClientShort { get; set; }
        public string ClientName { get; set; }
        public string StoreID { get; set; }
    }

    public class GetCatenationIDHelpRequest
    {
        private string _orderBy = "CatenationID";
        public string OrderBy { get { return _orderBy; } set { _orderBy = (value == "") ? "CatenationID" : value.Trim(); } }
    }
    public class GetClientListRequest
    {
        public string ClientID_Like { get; set; }
        public string ClientID_From { get; set; }
        public string ClientID_To { get; set; }
        public string StoreID_Like { get; set; }
        public string StoreID_From { get; set; }
        public string StoreID_To { get; set; }
        public string CatenationID_Like { get; set; }
        public string CatenationID_From { get; set; }
        public string CatenationID_To { get; set; }
        public string ClientName_Like { get; set; }
        public string ClientShort_Like { get; set; }
        public string InvoiceName_Like { get; set; }
        public string Class_Like { get; set; }
        public string Class_From { get; set; }
        public string Class_To { get; set; }
        public string UniteID_Like { get; set; }
        public string Principal_Like { get; set; }
        public string ContactPerson_Like { get; set; }
        public string Telphon01_Like { get; set; }
        public string Telphon01_From { get; set; }
        public string Telphon01_To { get; set; }
        public string Fax_Like { get; set; }
        public string Fax_From { get; set; }
        public string Fax_To { get; set; }
        public string Email_Like { get; set; }
        public string InvoicePost_Like { get; set; }
        public string InvoicePost_From { get; set; }
        public string InvoicePost_To { get; set; }
        public string InvoiceAddr01_Like { get; set; }
        public string AccountPost_Like { get; set; }
        public string AccountPost_From { get; set; }
        public string AccountPost_To { get; set; }
        public string AccountAddr01_Like { get; set; }
        public string SendPost_Like { get; set; }
        public string SendPost_From { get; set; }
        public string SendPost_To { get; set; }
        public string SendAddress01_Like { get; set; }
        public string OpenDate_Like { get; set; }
        public string OpenDate_From { get; set; }
        public string OpenDate_To { get; set; }

        public string OrderBy { get; set; }
        //新加的
        public string TelPhon02_Like { get; set; }
        public string TelPhon02_From { get; set; }
        public string TelPhon02_To { get; set; }
        public string TelPhon03_Like { get; set; }
        public string TelPhon03_From { get; set; }
        public string TelPhon03_To { get; set; }
        public string AreaNum_Like { get; set; }
        public string AreaNum_From { get; set; }
        public string AreaNum_To { get; set; }
        public string Discount_Like { get; set; }
        public string Discount_From { get; set; }
        public string Discount_To { get; set; }
        public string PersonNum_Like { get; set; }
        public string PersonNum_From { get; set; }
        public string PersonNum_To { get; set; }
        public string CreditLimit_Like { get; set; }
        public string CreditLimit_From { get; set; }
        public string CreditLimit_To { get; set; }
        public string Display_Like { get; set; }
        public string Display_From { get; set; }
        public string Display_To { get; set; }
        public string TaxRate_Like { get; set; }
        public string TaxRate_From { get; set; }
        public string TaxRate_To { get; set; }
        public string AccountAddr04_Like { get; set; }
        public string AccountAddr04_From { get; set; }
        public string AccountAddr04_To { get; set; }
        public string SendAddress04_Like { get; set; }
        public string SendAddress04_From { get; set; }
        public string SendAddress04_To { get; set; }
        public string Remark01_Like { get; set; }
        public string Remark01_From { get; set; }
        public string Remark01_To { get; set; }
        public string ChangeDate_Like { get; set; }
        public string ChangeDate_From { get; set; }
        public string ChangeDate_To { get; set; }
        public string ChangePerson_Like { get; set; }
        public string ChangePerson_From { get; set; }
        public string ChangePerson_To { get; set; }
        //原本少的
        public string ClientName_From { get; set; }
        public string ClientName_To { get; set; }
        public string ClientShort_From { get; set; }
        public string ClientShort_To { get; set; }
        public string InvoiceName_From { get; set; }
        public string InvoiceName_To { get; set; }
        public string UniteID_From { get; set; }
        public string UniteID_To { get; set; }

        public string Principal_From { get; set; }
        public string Principal_To { get; set; }
        public string ContactPerson_From { get; set; }
        public string ContactPerson_To { get; set; }
        public string Email_From { get; set; }
        public string Email_To { get; set; }
        public string InvoiceAddr01_From { get; set; }
        public string InvoiceAddr01_To { get; set; }
        public string AccountAddr01_From { get; set; }
        public string AccountAddr01_To { get; set; }
        public string SendAddress01_From { get; set; }
        public string SendAddress01_To { get; set; }
    }
    public class UpdClientRequest
    {
        private string _clientID;
        [Required(ErrorMessage = "請輸入客戶代號")]
        public string ClientID { get { return _clientID; } set { _clientID = value?.Trim(); } }

        private string _storeID;
        public string StoreID { get { return _storeID; } set { _storeID = value?.Trim(); } }
        public string CatenationID { get; set; }
        public string ClientName { get; set; }
        public string ClientShort { get; set; }
        public string AccountAddr04 { get; set; }
        public string SendAddress04 { get; set; }
        public string Class { get; set; }
        public string UniteID { get; set; }
        public string InvoiceName { get; set; }
        public string Principal { get; set; }
        public string ContactPerson { get; set; }
        public string TelPhon01 { get; set; }
        public string TelPhon02 { get; set; }
        public string TelPhon03 { get; set; }
        public string Fax { get; set; }

        public string InvoicePost { get; set; }
        public string InvoiceAddr01 { get; set; }
        public string AccountPost { get; set; }
        public string AccountAddr01 { get; set; }
        public string SendPost { get; set; }
        public string SendAddress01 { get; set; }
        public string Email { get; set; }
        public float AreaNum { get; set; }
        public float Discount { get; set; }
        public float PersonNum { get; set; }
        public float CreditLimit { get; set; }
        public string Display { get; set; }
        public float TaxRate { get; set; }
        public string OpenDate { get; set; }
        public string Remark01 { get; set; }
        public string ChangePerson { get; set; }
        public string ChangeDate { get; set; }
        public string Operation { get; set; }
    }
    public class GetClientStoreRequest
    {
        public string UserID { get; set; }
        public string SellBranch { get; set; }
    }
    public class GetClientOptionsExcludeMainWHRequest
    {
        public string StoreID { get; set; } = string.Empty;
    }
    public class GetClientOptionsOnlyMainWHRequest
    {
        public string StoreID { get; set; } = string.Empty;
    }
    public class GetBillCustOptionsRequest
    {
        public string CustID { get; set; } = string.Empty;
    }

    public class GetClientMeStoreRequest
    {
        public string SellBranch { get; set; }
    }

    public class ClientDataLength
    {
        public int Length { get; set; }
    }
    public class GetDataRequest
    {

        public string Q { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Sort { get; set; }
        public string SortColumn { get; set; }
        public string Mode { get; set; }
        public AdvanceSearchRequest AdvanceRequest { get; set; }
        public SearchTerm SearchTerm { get; set; }
    }

    public class ClientData
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string InvoiceName { get; set; }
        public string ClientShort { get; set; }
        public string UniteID { get; set; }
        public string StoreID { get; set; }
        public string Principal { get; set; }
        public string ContactPerson { get; set; }
        public string TelPhon01 { get; set; }
        public string TelPhon02 { get; set; }
        public string Fax { get; set; }
        public string Operation { get; set; }
        public string InvoiceAddr01 { get; set; }
        public string AccountAddr01 { get; set; }
        public string SendAddress01 { get; set; }
        public float TaxRate { get; set; }
        public float Discount { get; set; }
        public string CatenationID { get; set; }
        public string Remark01 { get; set; }
        public string ChangeDate { get; set; }
        public float FirstAccount { get; set; }
        public float BeforeAccount { get; set; }
        public string OpenDate { get; set; }
        public string ChangePerson { get; set; }
        public float CreditLimit { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string BankName { get; set; }
        public bool Display { get; set; }
        public string InvoicePost { get; set; }
        public string AccountPost { get; set; }
        public string SendPost { get; set; }
        public string AccountAddr04 { get; set; }
        public string SendAddress04 { get; set; }
        public string Class { get; set; }
        public string Remark02 { get; set; }
        public string SendAddress02 { get; set; }
        public string OldDate { get; set; }
        public string Unit { get; set; }
        public string ClassNew { get; set; }
        public string Company { get; set; }
    }

    public class GetSingleDataRequest
    {
        public string ClientID { get; set; }
    }

    public class AddClientDataRequest
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }
        public string InvoiceName { get; set; }
        public string ClientShort { get; set; }
        public string UniteID { get; set; }
        public string StoreID { get; set; }
        public string Principal { get; set; }
        public string ContactPerson { get; set; }
        public string TelPhon01 { get; set; }
        public string TelPhon02 { get; set; }
        public string Fax { get; set; }
        public string Operation { get; set; }
        public string InvoiceAddr01 { get; set; }
        public string AccountAddr01 { get; set; }
        public string SendAddress01 { get; set; }
        public float TaxRate { get; set; }
        public float Discount { get; set; }
        public string CatenationID { get; set; }
        public string Remark01 { get; set; }
        public string ChangeDate { get; set; }
        public float FirstAccount { get; set; }
        public float BeforeAccount { get; set; }
        public string OpenDate { get; set; }
        public string ChangePerson { get; set; }
        public float CreditLimit { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string BankName { get; set; }
        public bool Display { get; set; }
        public string InvoicePost { get; set; }
        public string AccountPost { get; set; }
        public string SendPost { get; set; }
        public string AccountAddr04 { get; set; }
        public string SendAddress04 { get; set; }
        public string Class { get; set; }
        public string Remark02 { get; set; }
        public string SendAddress02 { get; set; }
        public string OldDate { get; set; }
        public string Unit { get; set; }
        public string ClassNew { get; set; }
        public string Company { get; set; }
    }
    public class ID
    {
        public string ClientID { get; set; }
    }

    public class DelClientDataRequest
    {
        public List<ID> DelList { get; set; }
    }

    public class UpdClientDataRequest
    {
        public string OriginalClientID { get; set; }
        public string ClientName { get; set; }
        public string InvoiceName { get; set; }
        public string ClientShort { get; set; }
        public string UniteID { get; set; }
        public string StoreID { get; set; }
        public string Principal { get; set; }
        public string ContactPerson { get; set; }
        public string TelPhon01 { get; set; }
        public string TelPhon02 { get; set; }
        public string Fax { get; set; }
        public string Operation { get; set; }
        public string InvoiceAddr01 { get; set; }
        public string AccountAddr01 { get; set; }
        public string SendAddress01 { get; set; }
        public float TaxRate { get; set; }
        public float Discount { get; set; }
        public string CatenationID { get; set; }
        public string Remark01 { get; set; }
        public string ChangeDate { get; set; }
        public float FirstAccount { get; set; }
        public float BeforeAccount { get; set; }
        public string OpenDate { get; set; }
        public string ChangePerson { get; set; }
        public float CreditLimit { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string BankName { get; set; }
        public bool Display { get; set; }
        public string InvoicePost { get; set; }
        public string AccountPost { get; set; }
        public string SendPost { get; set; }
        public string AccountAddr04 { get; set; }
        public string SendAddress04 { get; set; }
        public string Class { get; set; }
        public string Remark02 { get; set; }
        public string SendAddress02 { get; set; }
        public string OldDate { get; set; }
        public string Unit { get; set; }
        public string ClassNew { get; set; }
        public string Company { get; set; }


    }

    public class ClientDB
    {
        public string ClientID { get; set; }
        public string StoreID { get; set; }
        public string CatenationID { get; set; }
        public string ClientName { get; set; }
        public string ClientShort { get; set; }
        public string Class { get; set; }
        public float PersonNum { get; set; }
        public float AreaNum { get; set; }
        public string UniteID { get; set; }
        public string InvoiceName { get; set; }
        public string Principal { get; set; }
        public string ContactPerson { get; set; }
        public string TelPhon01 { get; set; }
        public string TelPhon02 { get; set; }
        public string TelPhon03 { get; set; }
        public string Fax { get; set; }
        public string InvoicePost { get; set; }
        public string InvoiceAddr01 { get; set; }
        public string AccountPost { get; set; }
        public string AccountAddr01 { get; set; }
        public string SendPost { get; set; }
        public string SendAddress01 { get; set; }
        public float TaxRate { get; set; }
        public float Discount { get; set; }
        public string OpenDate { get; set; }
        public float CreditLimit { get; set; }
        public string Email { get; set; }
        public string Remark01 { get; set; }
        public string AccountAddr04 { get; set; }
        public string SendAddress04 { get; set; }
        public string Operation { get; set; }
        public string ChangeDate { get; set; }
        public string ChangePerson { get; set; }
        public string Remark02 { get; set; }
        public string OldDate { get; set; }
        public string Unit { get; set; }
        public string ClassNew { get; set; }
        public string Company { get; set; }
    }

}
