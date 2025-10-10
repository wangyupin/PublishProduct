using CityHubCore.Application.Base;
using CityHubCore.Infrastructure.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CityAdminDomain.Models.DB.CityAdmin {
    public class CityAdminDbContextDapper : IApplicationDapper {
        private readonly IApplicationDbContext _context;

        string InterfaceBase.ImplementationName { get => "CityAdminDbContextDapper"; }

        public IDbConnection Connection { get { return _context.Connection; } }

        public CityAdminDbContextDapper(IEnumerable<IApplicationDbContext> contexts) {
            _context = contexts.SingleOrDefault(m => m.ImplementationName == "CityAdminDbContext") ??
                throw new NotImplementedException("CityAdminDbContext not found in IServiceCollection! Maybe check Startup.cs or ConfigureApplicationServices.cs ");
        }
    }
}
