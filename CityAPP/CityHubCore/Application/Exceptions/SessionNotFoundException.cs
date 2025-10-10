using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Application.Exceptions {
    public class SessionNotFoundException : Exception {
        public SessionNotFoundException() : base() { }

        public SessionNotFoundException(string message) : base(message) { }

        public SessionNotFoundException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }
}
