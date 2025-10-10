using LocalPortService.BizService.Interface;
using LocalPortService.Model.CardMachine;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LocalPortService.BizService.CardMachine
{
    public class CTBCCardMachineStrategy : ICardMachineStrategy
    {
        public Task<ECR_Out_Messge_ParseObj> ProcessPayment(CreditCardPayment data)
        {
            var entity = new CTBCCardMachine();
            decimal roundedValue = decimal.Round(data.Total ?? 0, 2) * 100;
            entity.TransactionType = ((int)(data.TransactionType)).ToString();
            entity.HostID = ((int)(data.HostID)).ToString();
            entity.TransactionAmount = roundedValue.ToString("F0");
            entity.StoreID = data.StoreID;

            string Ecr_In_Message = entity.ComposeECR_In_Message();
            var errorMsg = entity.WriteToEcr_IN(Ecr_In_Message);
            if (errorMsg != null) entity.LaunchEcrExe();
            var e = (ECR_Out_Messge_ParseObj)entity.ParseResponse();

            return Task.FromResult(e);
        }

        public Task<ECR_Out_Messge_ParseObj> ProcessRefund(CreditCardRefund data)
        {
            var entity = new CTBCCardMachine();
            decimal roundedValue = decimal.Round(data.Total ?? 0, 2) * 100;
            entity.TransactionType = ((int)(data.TransactionType)).ToString();
            entity.HostID = ((int)(data.HostID)).ToString();
            entity.TransactionAmount = roundedValue.ToString("F0");
            entity.StoreID = data.StoreID;
            entity.TerminalID = data.TerminalID;
            entity.RefNo = data.RefNo;

            string Ecr_In_Message = entity.ComposeECR_In_Message();
            var errorMsg = entity.WriteToEcr_IN(Ecr_In_Message);
            if (errorMsg != null) entity.LaunchEcrExe();
            var e = (ECR_Out_Messge_ParseObj)entity.ParseResponse();

            return Task.FromResult(e);
        }
    }
}
