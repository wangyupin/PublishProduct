using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Application.Exceptions {
    public class SessionNotAllowException : Exception {
        public SessionNotAllowException() : base() { }

        public SessionNotAllowException(string message) : base(message) { }

        public SessionNotAllowException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }
}
