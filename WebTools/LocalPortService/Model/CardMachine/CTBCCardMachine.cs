using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace LocalPortService.Model.CardMachine
{
    public class CTBCCardMachine : CardMachine
    {
        public string? InvoiceNo { get; set; }   // 5-10 bytes

        public string? CardNo { get; set; }   // 11-29 bytes
        public string? CardExpDate { get; set; }          // 30-33 bytes
        public string? ApprovalCode { get; set; }     // 54-62 bytes
        public string? Amount1 { get; set; }     // 63-74 bytes
        public string? RespCode { get; set; }     // 75-78 bytes
        public string? TerminalID { get; set; }     // 79-86 bytes
        public string? RefNo { get; set; }     // 86-97 bytes
        public string? Amount2 { get; set; }     // 98-109 bytes
        public string? Amount3 { get; set; }     // 127-138 bytes
        public string? Amount4 { get; set; }     // 139-150 bytes
        public string? InquiryType { get; set; }     // 151-152 bytes
        public string? ProductCode { get; set; }     // 152-153 bytes
        public string? SHACardNo { get; set; }     // 154-203 bytes

        public override string ComposeECR_In_Message()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.TransactionType.PadLeft(2, '0'));
            sb.Append(base.HostID.PadLeft(2, '0'));
            sb.Append((InvoiceNo ?? string.Empty).PadLeft(6,' '));
            sb.Append((CardNo ?? string.Empty).PadLeft(19, ' '));
            sb.Append((CardExpDate ?? string.Empty).PadLeft(4, ' '));
            sb.Append(base.TransactionAmount.PadLeft(12, '0'));
            sb.Append(base.TransactionDate);
            sb.Append(base.TransactionTime);
            sb.Append((ApprovalCode ?? string.Empty).PadLeft(9, ' '));
            sb.Append((Amount1 ?? string.Empty).PadLeft(12, ' '));
            sb.Append((RespCode ?? string.Empty).PadLeft(4, ' '));
            sb.Append((TerminalID ?? string.Empty).PadLeft(8, ' '));
            sb.Append((RefNo ?? string.Empty).PadLeft(12, ' '));
            sb.Append((Amount2 ?? string.Empty).PadLeft(12, ' '));
            sb.Append(base.StoreID.PadRight(18, ' '));
            sb.Append((Amount3 ?? string.Empty).PadLeft(12, ' '));
            sb.Append((Amount4 ?? string.Empty).PadLeft(12, ' '));
            sb.Append((InquiryType ?? string.Empty).PadLeft(2, ' '));
            sb.Append((ProductCode ?? string.Empty).PadLeft(2, ' '));
            sb.Append((SHACardNo ?? string.Empty).PadLeft(50, ' '));

            return sb.ToString().PadRight(400, ' ');
            
        }
        
        public override string WriteToEcr_IN(string message)
        {
            string executableLocation = AppContext.BaseDirectory;
            string datPath = @"ECR822_4001/in.dat";
            string filePath = Path.Combine(executableLocation, datPath);

            if (File.Exists(filePath))
            {
                try
                {
                    File.WriteAllTextAsync(filePath, message);
                }
                catch (Exception ex)
                {
                    return $"寫入電文時出錯: {ex.Message}";
                }
                return string.Empty;
            }
            else
            {
                return $"ECR路徑出錯";
            }
        }
     
        public override void LaunchEcrExe()
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string exePath = @"ECR822_4001/ECR.exe";
            string outdat = @"ECR822_4001/out.dat";
            string exefilePath = Path.Combine(executableLocation, exePath);
            string outdatPath = Path.Combine(executableLocation, outdat);
            string exeDir = Path.Combine(executableLocation, "ECR822_4001");
            if (File.Exists(exefilePath))
            {
                File.Delete(outdatPath);

                ProcessStartInfo info = new ProcessStartInfo();
                info.WorkingDirectory = exeDir;
                info.FileName = "ECR.exe";
                info.UseShellExecute = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);
            }
        }
        public override CTBCECRResponse ParseResponse()
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string datPath = @"ECR822_4001/out.dat";
            string filePath = Path.Combine(executableLocation, datPath);
            var res = new CTBCECRResponse();

            while (!File.Exists(filePath)) 
            {
                Thread.Sleep(1000);
            }
            try
            {
                Task<string> outDatTxt =  File.ReadAllTextAsync(filePath);
                string txt = outDatTxt.Result;
                string respCode = txt.Substring(78, 4);
                string terminalID = txt.Substring(82, 8);
                string refNo = txt.Substring(90, 12);
                res.TerminalID = terminalID;
                res.RefNo = refNo;
                int.TryParse(respCode, out int result);
                res.RespCode = (RespCode)result;

                File.Delete(filePath);

            }
            catch (Exception ex)
            {
                res.Msg = ex.Message;
            }
           
            return res;
        }
      
    }

    public class CTBCECRResponse : ECR_Out_Messge_ParseObj
    {
        public RespCode RespCode {  get; set; }
        public string TerminalID { get; set; }
        public string RefNo { get; set; }
    }
}
