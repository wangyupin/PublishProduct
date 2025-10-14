using POVWebDomain.Common;
using System.Threading.Tasks;

namespace HqSrv.Domain.Services
{
    public interface IOptionService
    {
        Task<Result<dynamic>> GetSalesModeType();
        Task<Result<dynamic>> GetSellingDateTime();
        Task<Result<dynamic>> GetShipping();
        Task<Result<dynamic>> GetPayment();
    }
}