using CityHubCore.Application.Base;
using CityHubCore.Infrastructure.DB;
using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace CityAdminDomain.Models.DB.CityAdmin {
    public class CityAdminDapper : IApplicationDapper, IDisposable {
        //private readonly IDbConnection _connection;
        private IDbConnection _connection;

        string InterfaceBase.ImplementationName { get { return "CityAdminDapper"; } }

        public IDbConnection Connection { get { return _connection; } }

        public CityAdminDapper(string connectionString) {
            _connection = new SqlConnection(connectionString);
        }

        //public void Dispose() => _connection.Dispose();

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_connection != null) {
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}
