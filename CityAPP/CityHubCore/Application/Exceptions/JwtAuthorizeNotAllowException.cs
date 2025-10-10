using System;
using System.Globalization;

namespace CityHubCore.Application.Exceptions {
    // custom exception class for throwing application specific exceptions (e.g. for validation) 
    // that can be caught and handled within the application
    public class JwtAuthorizeNotAllowException : Exception {
        public JwtAuthorizeNotAllowException() : base() { }

        public JwtAuthorizeNotAllowException(string message) : base(message) { }

        public JwtAuthorizeNotAllowException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }
}
