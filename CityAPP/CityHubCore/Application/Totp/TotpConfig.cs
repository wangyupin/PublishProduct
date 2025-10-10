/// <summary>
/// REF from https://github.com/BrandonPotter/GoogleAuthenticator
/// Moon @ 2021/10/12
/// </summary>
namespace CityHubCore.Application.Totp {
    public class TotpConfig {
        public string Issuer { get; set; }
        public string IVforAccountSecretKey { get; set; }
        /// <summary>
        /// TimeTolerance -> Plus or minus in 180 seconds
        /// </summary>
        public int TimeToleranceForSeconds { get; set; }
    }
}
