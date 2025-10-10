using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityHubCore.Application.Exceptions {
    public class LoginDuplicationException : Exception {
        public LoginDuplicationException() : base() { }

        public LoginDuplicationException(string message) : base(message) { }

        public LoginDuplicationException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }
}
