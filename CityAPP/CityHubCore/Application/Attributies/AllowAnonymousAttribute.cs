using System;

namespace CityHubCore.Application.Attributies {
    /// <summary>
    /// For Igrone Auth
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute { }
}
