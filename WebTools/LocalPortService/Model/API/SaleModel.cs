using LocalPortService.Core.Helper;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json.Serialization;

namespace LocalPortService.Model.API
{
    public class SaleHeaderPrinter
    {
        [Required(ErrorMessage = "缺少銷售門市")]
        public string SellStore { get; set; }
        [Required(ErrorMessage = "缺少銷售單號")]
        public string SellID { get; set; }
        [Required(ErrorMessage = "缺少銷售日期")]
        public string SellDate { get; set; }
        public string? Remark { get; set; }
        public string TerminalID { get; set; }
        public string LogoSrc { get; set; }
        public string TemplateFileName { get; set; }
        public string FBUrl { get; set; }
        public string IGUrl { get; set; }
        public string SellPerson { get; set; }
    }

    public class SaleDetailPrinter
    {
        [Required(ErrorMessage = "缺少商品編號")]
        public string GoodID { get; set; }
        [Required(ErrorMessage = "缺少商品名稱")]
        public string GoodName { get; set; }
        public string GoodStyle { get; set; }
        [Required(ErrorMessage = "缺少商品單價")]
        public int SellPrice { get; set; }
        [Required(ErrorMessage = "缺少商品數量")]
        public int Qty { get; set; }
        [Required(ErrorMessage = "缺少銷售價格")]
        public int SellCash { get; set; }
        public string Sort { get; set; }
        public float AdvicePrice { get; set; }
        public float Discount { get; set; }
    }

    public class SplitGoodDetail
    {
        public string Line1 { get; set; }
        public string Line2 { get; set; }
    }

    public class SalePrinterJsonObj
    {
        public SaleHeaderPrinter? Head { get; set; }
        public List<SaleDetailPrinter>? Body { get; set; }
    }

    public class SalesInvoice
    {
        [Required(ErrorMessage = "缺少發票號碼")]
        public string InvoiceCode { get; set; }

        [Required(ErrorMessage = "缺少發票賣方")]
        public string Seller { get; set; }
        public string Buyer { get; set; }
        public string BuyerName { get; set; }
        [Required(ErrorMessage = "缺少發票隨機碼")]
        public string RandomCode { get; set; }
        [Required(ErrorMessage = "缺少發票金額")]
        public decimal InvoiceAmount { get; set; }
        [Required(ErrorMessage = "缺少發票日期")]
        [JsonConverter(typeof(DateTimeConverter))]

        public DateTime InvoiceDate { get; set; }

        public string InvoiceTerm { get; set; }
        [Required(ErrorMessage = "缺少發票稅金")]
        public decimal TaxAmount { get; set; }
        public string TaxType { get; set; }
        public string QrCodedata1 { get; set; }
        public string? QrCodedata2 { get; set; }
        public string SellStore { get; set; }
        public string TelPhone { get; set; }
    }

    public class DetHeader
    {
        public string SellStore {  get; set; }
        public string TelPhone { get; set; }
        public string SellID { get; set; }
        public string SellDate { get; set; }
        public string Tax { get; set; }
    }

    public class DetBody
    {
        public string SellID { get; set; }
        public string SellStore { get; set; }
        public string TelPhone { get; set; }
        public string SellDate { get; set; }
        public string GoodID { get; set; }
        public string GoodName { get; set; }
        public string GoodStyle { get; set; }
        public float SellPrice { get; set; }
        public int Qty { get; set; }
        public float SellCash { get; set; }
        public string SellMode { get; set; }
        public float Cash { get; set; }
        public float Card{ get; set; }
        public float Gift { get; set; }
    }

    public class SalesInvoiceAndDetail
    {

        public SalesInvoice Invoice { get; set; }
        public DetHeader Header { get; set; }
        public List<DetBody>? Body { get; set; }
    }

    public class DiscountOrderPrint
    {
        public SalesInvoice Invoice { get; set; }
        public DetHeader Header { get; set; }
        public List<DetBody>? Body { get; set; }
    }

}
