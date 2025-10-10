using Microsoft.EntityFrameworkCore;
using CityHubCore.Infrastructure.DB;
using CityHubCore.Application.Base;
using System.Data;

namespace CityAdminDomain.Models.DB.CityAdmin {

    /// <summary>
    /// This is partial class of DB/CityAdmin/CityAdminDbContext.cs
    /// </summary>
    public partial class CityAdminDbContext : DbContext, IApplicationDbContext {

        public IDbConnection Connection => Database.GetDbConnection();

        string InterfaceBase.ImplementationName { get => "CityAdminDbContext"; }
    }
}
