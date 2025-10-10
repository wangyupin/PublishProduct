using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POVWebDomain.Models.ExternalApi
{

    public class HCTAddrCompare
    {
        public string USER { get; set; }
        public string ADDR { get; set; }
        public string ESDATE { get; set; }
        public string TEL { get; set; }
        public string TEL2 { get; set; }
        public string EPRDCL { get; set; }
        public string EMARK { get; set; }
        public string ESCSNO { get; set; }
        public string EJAMT { get; set; }
        public string EQAMT { get; set; }
        public string EQAMTTYPE { get; set; }
        public string ELAMTTYPE { get; set; }
    }
    public class HCTTransData
    {
        public string Epino { get; set; }
        public string Ercsig { get; set; }
        public string Ertel1 { get; set; }
        public string Eraddr { get; set; }
        public string Ejamt { get; set; }
        public string Eqamt { get; set; }
        public string Escsno { get; set; }
        public string Emark { get; set; }
        public string Eqmny { get; set; }
        public string Eddate { get; set; }
    }

    public class HCTAddrCompareResponse
    {
        [JsonPropertyName("NO")]
        public string NO { get; set; } //編號
        [JsonPropertyName("PUTDATA_M")]
        public string PUTDATA_M { get; set; } //站所四碼代號
        [JsonPropertyName("PUTDATANAME")]
        public string PUTDATANAME { get; set; } //站所簡碼
        [JsonPropertyName("PUTDATA_S")]
        public string PUTDATA_S { get; set; } //站所名稱
        [JsonPropertyName("PUTDATAZIP")]
        public string PUTDATAZIP { get; set; } //郵遞區號
        [JsonPropertyName("PUTOUT_AREA1")]
        public string PUTOUT_AREA1 { get; set; } //ＳＤ註區
        [JsonPropertyName("PUTOUT_AREA11")]
        public string PUTOUT_AREA11 { get; set; } //ＳＤ疊區
        [JsonPropertyName("PUTOUT_AREA2")]
        public string PUTOUT_AREA2 { get; set; } //ＤＤ註區
        [JsonPropertyName("PUTOUT_AREA21")]
        public string PUTOUT_AREA21 { get; set; } //ＤＤ疊區

        [JsonPropertyName("PUTOUT_AREA4")]
        public string PUTOUT_AREA4 { get; set; }//ＭＤ註區
        [JsonPropertyName("PUTOUT_AREA41")]
        public string PUTOUT_AREA41 { get; set; }//ＭＤ疊區
        [JsonPropertyName("PUTOUT_AREA6")]
        public string PUTOUT_AREA6 { get; set; }//低溫註區
        [JsonPropertyName("PUTOUT_AREA61")]
        public string PUTOUT_AREA61 { get; set; }//低溫疊區
        [JsonPropertyName("PUTF5STAR")]
        public string PUTF5STAR { get; set; }//聯運費用(起碼)
        [JsonPropertyName("PUTF5GWET")]
        public string PUTF5GWET { get; set; }//聯運費用(百公斤)
        [JsonPropertyName("PUTNEWRKZONE")]
        public string PUTNEWRKZONE { get; set; }//新集貨註區
        [JsonPropertyName("CODE1")]
        public string CODE1 { get; set; }//類別
        [JsonPropertyName("CODE2")]
        public string CODE2 { get; set; }//到著簡碼
        [JsonPropertyName("CODE3")]
        public string CODE3 { get; set; }//衛星區
        [JsonPropertyName("CODE4")]
        public string CODE4 { get; set; }//註區
        [JsonPropertyName("CODE5")]
        public string CODE5 { get; set; }//疊區
        [JsonPropertyName("CODE6")]
        public string CODE6 { get; set; }//QRcode
        [JsonPropertyName("AREAS")]
        public string AREAS { get; set; }//聯運區提醒(*表示有聯運區,空白表示沒有)

        [JsonPropertyName("MDCODE1")]
        public string MDCODE1 { get; set; }//MD到著碼衛星區
        [JsonPropertyName("MDCODE2")]
        public string MDCODE2 { get; set; }//MD到著碼註區
        [JsonPropertyName("MDCODE3")]
        public string MDCODE3 { get; set; }//MD到著碼疊區
        [JsonPropertyName("MSG")]
        public string MSG { get; set; }//訊息
    }

    public class HCTTransDataResponse
    {
        [JsonPropertyName("Num")]
        public string Num { get; set; }
        [JsonPropertyName("success")]
        public string Success { get; set; }
        [JsonPropertyName("edelno")]
        public string Edelno { get; set; }
        [JsonPropertyName("epino")]
        public string Epino { get; set; }
        [JsonPropertyName("erstno")]
        public string Erstno { get; set; }
        [JsonPropertyName("eqamt")]
        public string Eqamt { get; set; }
        [JsonPropertyName("escsno")]
        public string Escsno { get; set; }
        [JsonPropertyName("eqmny")]
        public string Eqmny { get; set; }
        [JsonPropertyName("ErrMsg")]
        public string ErrMsg { get; set; }



        [JsonPropertyName("NewOutArea")]
        public string NewOutArea { get; set; }
        [JsonPropertyName("CODE1")]
        public string CODE1 { get; set; }
        [JsonPropertyName("CODE2")]
        public string CODE2 { get; set; }
        [JsonPropertyName("CODE3")]
        public string CODE3 { get; set; }
        [JsonPropertyName("CODE4")]
        public string CODE4 { get; set; }
        [JsonPropertyName("CODE5")]
        public string CODE5 { get; set; }
        [JsonPropertyName("CODE7")]
        public string CODE7 { get; set; }
        [JsonPropertyName("AREAS")]
        public string AREAS { get; set; }
        [JsonPropertyName("MDCODE1")]
        public string MDCODE1 { get; set; }
        [JsonPropertyName("MDCODE2")]
        public string MDCODE2 { get; set; }
        [JsonPropertyName("MDCODE3")]
        public string MDCODE3 { get; set; }
    }

    public class HCTKey
    {
        public string Company { get; set; }
        public string Password { get; set; }
    }
    public class TCatApiKey
    {
        public string CustomerContractID { get; set; }
        public string CustomerContractCode { get; set; }
    }
    public class TCatAddresses
    {
        public string Search { get; set; }
    }
    public class TCatAddressesResItem
    {
        public string Search { get; set; }
        public string PostNumber { get; set; }

    }
    public class TCatAddressesObject
    {
        public List<TCatAddressesResItem> Addresses { get; set; }
    }
    public class TCatParsingAddress
    {
        public string CustomerId { get; set; }
        public string CustomerToken { get; set; }
        public List<TCatAddresses> Addresses { get; set; }

    }
    public class TCatParsingAddressResponse
    {
        public string SrvTranId { get; set; }
        public string IsOK { get; set; }
        public string Message { get; set; }
        public TCatAddressesObject Data { get; set; }
    }
    public class TCatOrderRequest
    {
        public string OBTNumber { get; set; }
        public string OrderId { get; set; }
        public string Thermosphere { get; set; }
        public string Spec { get; set; }
        public string ReceiptLocation { get; set; }
        //public string ReceiptStationNo { get; set; }
        public string RecipientName { get; set; }
        public string RecipientTel { get; set; }
        public string RecipientMobile { get; set; }
        public string RecipientAddress { get; set; }
        public string SenderName { get; set; }
        public string SenderTel { get; set; }
        public string SenderMobile { get; set; }
        public string SenderZipCode { get; set; }
        public string SenderAddress { get; set; }
        public string ShipmentDate { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string IsFreight { get; set; }
        public string IsCollection { get; set; }
        public int CollectionAmount { get; set; }
        public string IsSwipe { get; set; }
        //public string IsMobilePay { get; set; }
        public string IsDeclare { get; set; }
        public int DeclareAmount { get; set; }
        public string ProductTypeId { get; set; }
        public string ProductName { get; set; }
        //public string Memo { get; set; }
    }

    public class TCatPrintOBT
    {
        public string CustomerId { get; set; }
        public string CustomerToken { get; set; }
        public string PrintType { get; set; }
        public string PrintOBTType { get; set; }
        public List<TCatOrderRequest> Orders { get; set; }
    }
    public class TCatOrder
    {
        public string OBTNumber { get; set; }
        public string OrderId { get; set; }
        public string FileNo { get; set; }
    }

    public class TCatPrintObj
    {
        public string PrintDateTime { get; set; }
        public List<TCatOrder> Orders { get; set; }
    }
    public class TCatPrintOBTResponse
    {
        public string SrvTranId { get; set; }
        public string IsOK { get; set; }
        public string Message { get; set; }
        public TCatPrintObj Data { get; set; }
    }

    public class HomeDeliveryAddressCommon
    {
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ShippingAddress { get; set; }
    }

}
