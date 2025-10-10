using LocalPortService.BizService.Interface;
using LocalPortService.Model.CardMachine;

namespace LocalPortService.BizService.CardMachine
{
    public class CardPaymentService
    {
        private readonly ICardMachineStrategy _cardMachineStrategy;

        public CardPaymentService(IConfiguration config)
        {
            // 讀取配置中的 CardMachineProvider 值
            string provider = config["CardMachineProvider"];

            // 根據配置值選擇不同的策略
            _cardMachineStrategy = provider switch
            {
                "CTBC" => new CTBCCardMachineStrategy(),
                _ => throw new Exception("Unsupported card machine provider")
            };
        }

        public Task<ECR_Out_Messge_ParseObj> ProcessPayment(CreditCardPayment req)
        {
            return _cardMachineStrategy.ProcessPayment(req);
        }

        public Task<ECR_Out_Messge_ParseObj> ProcessRefund(CreditCardRefund req)
        {
            return _cardMachineStrategy.ProcessRefund(req);
        }
    }
}
