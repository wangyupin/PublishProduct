using LocalPortService.Model.CardMachine;

namespace LocalPortService.BizService.Interface
{
    public interface ICardMachineStrategy
    {
        public Task<ECR_Out_Messge_ParseObj> ProcessPayment(CreditCardPayment data);
        public Task<ECR_Out_Messge_ParseObj> ProcessRefund(CreditCardRefund data);
    }
}
