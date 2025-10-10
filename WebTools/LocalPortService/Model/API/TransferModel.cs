namespace LocalPortService.Model.API
{
    public class TransferHeader
    {
        public string? TransferID { get; set; }
        public string? TranDate { get; set; }
        public string? TranOutStore { get; set; }
        public string? TranInStore { get; set; }
        public string? Remark { get; set; }
    }

    public class TransferDetail
    {
        public string? TransferID { get; set; }
        public string? GoodID { get; set; }
        public string? SizeNo { get; set; }
        public int Num { get; set; }
    }

    public class TransferPrinterJsonObj
    {
        public TransferHeader? Head { get; set; }
        public List<TransferDetail>? Body { get; set; }
    }
}
