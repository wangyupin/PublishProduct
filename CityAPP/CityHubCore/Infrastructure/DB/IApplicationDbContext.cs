using CityHubCore.Application.Base;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;

namespace CityHubCore.Infrastructure.DB {
    public interface IApplicationDbContext : InterfaceBase {
        public IDbConnection Connection { get; }
        DatabaseFacade Database { get; }
    }
}
