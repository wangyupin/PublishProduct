using CityHubCore.Application.Base;
using CityHubCore.Infrastructure.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace POVWebDomain.Models.DB.POVWeb {
    public class POVWebDbContextDapper : IApplicationDapper {
        private readonly IApplicationDbContext _context;

        string InterfaceBase.ImplementationName { get => "POVWebDbContextDapper"; }

        public IDbConnection Connection { get { return _context.Connection; } }

        public POVWebDbContextDapper(IEnumerable<IApplicationDbContext> contexts) {
            _context = contexts.SingleOrDefault(m => m.ImplementationName == "POVWebDbContext") ??
                throw new NotImplementedException("POVWebDbContext not found in IServiceCollection! Maybe check Startup.cs or ConfigureApplicationServices.cs ");
        }
    }
}
