using System;
using System.Globalization;

namespace CityHubCore.Application.Exceptions {
    // custom exception class for throwing application specific exceptions (e.g. for validation) 
    // that can be caught and handled within the application
    public class JwtRefreshTokenNotFoundException : Exception {
        public JwtRefreshTokenNotFoundException() : base() { }

        public JwtRefreshTokenNotFoundException(string message) : base(message) { }

        public JwtRefreshTokenNotFoundException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }
}
