using System.ComponentModel.DataAnnotations;

namespace LocalPortService.Model.CardMachine
{
    public interface ICardMachine
    {
        public string ComposeECR_In_Message();
        public ECR_Out_Messge_ParseObj ParseResponse();
        public string WriteToEcr_IN(string message);
        public void LaunchEcrExe();
    }
    public abstract class CardMachine : ICardMachine
    {
        public string TransactionType { get; set; }     // 1-2 bytes
        public string HostID { get; set; }   // 3-4 bytes
        public string TransactionAmount { get; set; }     // 33-44 bytes
        public string TransactionDate { get; set; } = DateTime.Now.ToString("yyMMdd");   // 45-48 bytes
        public string TransactionTime { get; set; } = DateTime.Now.ToString("hhmmss");   // 48-53 bytes
        public string StoreID { get; set; }     // 109-126 bytes

        public abstract string ComposeECR_In_Message();
        public abstract string WriteToEcr_IN(string message);
        public abstract void LaunchEcrExe();
        public abstract ECR_Out_Messge_ParseObj ParseResponse();
    }

    public abstract class ECR_Out_Messge_ParseObj
    {
        public string Msg { get; set; } = string.Empty;
    }

    public class CreditCardPayment
    {
        [Required(ErrorMessage = "請輸入交易金額")]
        [RegularExpression("^(?!0\\d)\\d+(\\.\\d+)?$", ErrorMessage = "交易金額需為正數")]
        public decimal? Total { get; set; }
        [Required(ErrorMessage = "請輸入交易類別")]
        public TransactionType? TransactionType { get; set; }
        [Required(ErrorMessage = "請輸入授權銀行編碼")]
        public HostID? HostID { get; set; }
        [Required(ErrorMessage = "請輸入櫃號")]
        public string? StoreID { get; set; }
    }

    public class CreditCardRefund
    {
        [Required(ErrorMessage = "請輸入交易金額")]
        [RegularExpression("^(?!0\\d)\\d+(\\.\\d+)?$", ErrorMessage = "交易金額需為正數")]
        public decimal? Total { get; set; }
        [Required(ErrorMessage = "請輸入交易類別")]
        public TransactionType? TransactionType { get; set; }
        [Required(ErrorMessage = "請輸入授權銀行編碼")]
        public HostID? HostID { get; set; }
        [Required(ErrorMessage = "請輸入櫃號")]
        public string? StoreID { get; set; }
        [Required(ErrorMessage = "請輸入端末機編號")]
        public string? TerminalID { get; set; }
        [Required(ErrorMessage = "請輸入序號")]
        public string? RefNo { get; set; }
    }

    public enum TransactionType
    {
        Sale = 1,
        Refund = 2,
        Offline = 3,
        Void = 30
    }

    public enum HostID
    {
        CreditCard = 1,
        RewardPointsRedemption = 2,
        InstallmentPayment = 3,
        UnionPayCard = 4
    }

    public enum RespCode
    {
        Approval = 0,
        Error = 1,
        CallBank = 2,
        TimeOut = 3
    }
}
