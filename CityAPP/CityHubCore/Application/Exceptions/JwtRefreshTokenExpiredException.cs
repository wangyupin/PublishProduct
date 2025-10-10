using System;
using System.Globalization;

namespace CityHubCore.Application.Exceptions {
    // custom exception class for throwing application specific exceptions (e.g. for validation) 
    // that can be caught and handled within the application
    public class JwtRefreshTokenExpiredException : Exception {
        public JwtRefreshTokenExpiredException() : base() { }

        public JwtRefreshTokenExpiredException(string message) : base(message) { }

        public JwtRefreshTokenExpiredException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args)) {
        }
    }
}
