using CityHubCore.Application.Base;
using System.Data;

namespace CityHubCore.Infrastructure.DB {
    public interface IApplicationDapper : InterfaceBase {
        public IDbConnection Connection { get; }

    }
}
