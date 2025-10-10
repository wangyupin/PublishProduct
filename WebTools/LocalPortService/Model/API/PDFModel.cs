using System.ComponentModel.DataAnnotations;

namespace LocalPortService.Model.API
{
    public class PDFExample
    {
        [Required(ErrorMessage = "請輸入公司名稱")]
        public string CompanyName { get; set; }
        public string? DocType { get; set; }
        [Required(ErrorMessage = "表頭資料不為空")]
        public PDFExampleHead PDFExampleHead {get; set;}
        [Required(ErrorMessage = "表身資料不為空")]
        public PDFExampleBody PDFExampleBody { get; set; }

    }

    public class PDFExampleHead
    {
        [Required(ErrorMessage = "表頭細項不為空")]
        public List<PDFExampleHeadItem> headItems { get; set; } 
    }

    public class PDFExampleHeadItem
    {
        [Required(ErrorMessage = "表頭細項名稱不為空")]
        public string ItemName { get; set; }
        public string? ItemValue { get; set; }
    }

    public class PDFExampleBody
    {
        [Required(ErrorMessage = "表身欄位資料不為空")]
        public List<PDFExampleBodyColumn> Column { get; set; }

        public List<List<PDFExampleBodyColumn>> Columns { get; set; }
        [Required(ErrorMessage = "表身RowData不為空")]
        public List<dynamic> Rows { get; set; }
        public List<ColumnTotal> Totals { get; set; }
        public decimal Tax { get; set; } = 0;
        public decimal Amount { get; set; } = 0;
        public bool TaxOption { get; set; } = false;
        public bool DecimalPoint { get; set; } = false;
        public string TaxMode { get; set; }
        public string SummaryColumn { get; set; }
        public string? Remark { get; set; }
    }

    public class PDFExampleBodyColumn
    {
        [Required(ErrorMessage = "表身欄位名稱不為空")]
        public string ColumnName { get; set; }
        [Required(ErrorMessage = "表身欄位長度不為空")]
        public double Width { get; set; }
        public string Align { get; set; } = "left";
    }

    public class ColumnTotal
    {
        [Required(ErrorMessage = "表身總計名稱不為空")]
        public string ColumnName { get; set; }
        [Required(ErrorMessage = "表身總計金額不為空")]
        public dynamic Total { get; set; }
    }

    public class ECPickingListPDFModel
    {
        [Required(ErrorMessage = "請輸入公司名稱")]
        public string CompanyName { get; set; }
        public string? DocType { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyTel { get; set; }
        [Required(ErrorMessage = "表頭資料不為空")]
        public ECPickingListHead ECPickingListHead { get; set; }
        [Required(ErrorMessage = "表身資料不為空")]
        public ECPickingListBody ECPickingListBody { get; set; }

    }

    public class ECPickingListHead
    {
        [Required(ErrorMessage = "收件人不為空")]
        public string Recipient { get; set; }
        public string? RecipientAddress { get; set; }
        public string? TelPhone { get; set; }
        public string? Mobile { get; set; }
        public string? ShippingMethod { get; set; }
        public string PrintDate { get; set; }
        public string BarCode { get; set; }
        public string? ClientID { get; set; }
        public string? ClientName { get; set; }
        public string PlatformName { get; set; }



    }

    public class ECPickingListBody
    {
        public List<PDFExampleBodyColumn> Column { get; set; }
        public List<ECPickingListBodyRow> Rows { get; set; }
        public string? Remark { get; set; }
        public int TotalNum { get; set; } = 0;

        public decimal Amount { get; set; } = 0;
        public bool MoreOption { get; set; } = true;
        public bool DecimalPoint { get; set; } = false;
        public decimal ShippingFee { get; set; } = 0;
        public decimal Discount { get; set; } = 0;

    }

    public class ECPickingListBodyRow
    {
        public string ParentSKU { get; set; }
        public string GoodID { get; set; }
        public string GoodName { get; set; }

        public int Quantity { get; set; }
        public decimal SellPrice { get; set; }
        public decimal SellAmount { get; set; }

    }

    public class ECPickingListMultiPDFModel
    {
        [Required(ErrorMessage = "請輸入公司名稱")]
        public string CompanyName { get; set; }
        public string? DocType { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyTel { get; set; }
        public List<ECPickingListPDFModel> Examples { get; set; }

    }

    public class ECMasterPickingListPDFModel
    {
        public string? DocType { get; set; }
        public ECMasterPickingListHead ECMasterPickingListHead { get; set; }
        [Required(ErrorMessage = "表身資料不為空")]
        public ECMasterPickingListBody ECMasterPickingListBody { get; set; }

    }
    public class ECMasterPickingListHead
    {
        public string PrintDate { get; set; } = DateTime.Now.ToString("yyyyMMss");

    }
    public class ECMasterPickingListBody
    {
        public List<PDFExampleBodyColumn> Column { get; set; }
        public List<ECMasterPickingListBodyRow> Rows { get; set; }
        public int TotalNum { get; set; } = 0;

        public bool MoreOption { get; set; } = true;
       

    }

    public class ECMasterPickingListBodyRow
    {
        public string GoodID { get; set; }

        public int Quantity { get; set; }

    }


}
