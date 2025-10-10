using Microsoft.EntityFrameworkCore;
using CityHubCore.Infrastructure.DB;
using CityHubCore.Application.Base;
using System.Data;

namespace POVWebDomain.Models.DB.POVWeb {

    /// <summary>
    /// This is partial class of DB/POVWeb/POVWebDbContext.cs
    /// </summary>
    public partial class POVWebDbContext : DbContext, IApplicationDbContext {

        public IDbConnection Connection => Database.GetDbConnection();

        string InterfaceBase.ImplementationName { get => "POVWebDbContext"; }
    }
}
