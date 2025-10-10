using CityAdminDomain.Models.Common;
using CityHubCore.Application.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityAdminDomain.Services.User {
    /// <summary>
    /// 處理使用者資訊、資料相關
    /// </summary>
    public interface IUser<T> : InterfaceBase where T : class {
        Task<UserBasicInfo> GetById(long Id);
        Task<IEnumerable<UserBasicInfo>> GetList();

    }
}
