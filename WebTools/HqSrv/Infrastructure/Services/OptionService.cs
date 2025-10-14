using HqSrv.Domain.Services;
using POVWebDomain.Common;
using POVWebDomain.Models.API.StoreSrv.Common;
using POVWebDomain.Models.API.StoreSrv.Common.Options;
using POVWebDomain.Models.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HqSrv.Infrastructure.Services
{
    public class OptionService : IOptionService
    {
        public async Task<Result<dynamic>> GetSalesModeType()
        {
            SaleModeTypeReturn responseBody = new SaleModeTypeReturn
            {
                Options = new List<CheckboxOption<int>> {
                    new CheckboxOption<int>(1, "現金"),
                    new CheckboxOption<int>(2, "點數加價購")
                },
            };
            responseBody.SalesModeTypeDef = responseBody.Options.Select(t => new SelectableOption<int>
            {
                ID = t.ID,
                Checked = t.ID == 1
            }).ToList();

            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<dynamic>> GetSellingDateTime()
        {
            SellingDateTimeReturn responseBody = new SellingDateTimeReturn
            {
                Options = new List<Option<int>> {
                    new Option<int>(0, "一年"),
                    new Option<int>(1, "五年"),
                    new Option<int>(2, "無期限"),
                    new Option<int>(3, "自定結束時間")
                }
            };
            responseBody.SellingDateTime = responseBody.Options[0];

            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<dynamic>> GetShipping()
        {
            GetShippingReturn responseBody = new GetShippingReturn
            {
                Options = new List<CheckboxOption<long>>
        {
            new CheckboxOption<long> { ID = 6, Name = "宅配" },
            new CheckboxOption<long> { ID = 7, Name = "宅配冷凍" },
            new CheckboxOption<long> { ID = 8, Name = "超商取貨付款 - 全家" },
            new CheckboxOption<long> { ID = 9, Name = "超商取貨付款 - 7-11" },
            new CheckboxOption<long> { ID = 10, Name = "付款後超商取貨 - 全家" },
            new CheckboxOption<long> { ID = 11, Name = "付款後超商取貨 - 7-11" },
            new CheckboxOption<long> { ID = 12, Name = "宅配（貨到付款）" },

            // 加入您要的運送方式
        }
            };
            responseBody.ShippingTypes = responseBody.Options.Select(t => new ShippingTypes { ID = t.ID }).ToList();

            return Result<dynamic>.Success(new { responseBody });
        }

        public async Task<Result<dynamic>> GetPayment()
        {
            GetPaymentReturn responseBody = new GetPaymentReturn
            {
                Options = new List<CheckboxOption<int>>
        {
            new CheckboxOption<int> { ID = 1, Name = "信用卡" },
            new CheckboxOption<int> { ID = 2, Name = "信用卡分期付款" },
            new CheckboxOption<int> { ID = 11, Name = "全家取貨付款" },
            new CheckboxOption<int> { ID = 12, Name = "7-11取貨付款" },
            new CheckboxOption<int> { ID = 21, Name = "ATM付款" },
            new CheckboxOption<int> { ID = 22, Name = "貨到付款" },
            new CheckboxOption<int> { ID = 31, Name = "Apple Pay" },
            new CheckboxOption<int> { ID = 41, Name = "LINE Pay" },
            new CheckboxOption<int> { ID = 42, Name = "街口支付" },
            new CheckboxOption<int> { ID = 43, Name = "悠遊付" },
            new CheckboxOption<int> { ID = 44, Name = "AFTEE先享後付" },
            new CheckboxOption<int> { ID = 45, Name = "全盈+PAY" },
        }
            };
            responseBody.PayTypes = responseBody.Options.Select(t => new SelectableOption<int> { ID = t.ID }).ToList();

            return Result<dynamic>.Success(new { responseBody });
        }
    }
}