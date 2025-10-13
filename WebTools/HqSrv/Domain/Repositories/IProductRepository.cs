using HqSrv.Domain.Entities;
using POVWebDomain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HqSrv.Domain.Repositories
{
    /// <summary>
    /// 商品資料庫抽象介面 - 由 Infrastructure 層實作
    /// </summary>
    public interface IProductRepository
    {
        Task<Result<Product>> GetByParentIdAsync(string parentId);
        Task<Result<List<Product>>> GetByIdsAsync(List<string> parentIds);
        Task<Result<Product>> SaveAsync(Product product);
        Task<Result<bool>> ExistsAsync(string parentId);
        Task<Result<List<Product>>> GetPublishedProductsAsync(string platformId);
    }
}