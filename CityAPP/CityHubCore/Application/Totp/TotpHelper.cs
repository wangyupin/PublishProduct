using Google.Authenticator;
using Microsoft.Extensions.Options;

namespace CityHubCore.Application.Totp {
    public class TotpHelper {
        private readonly TotpConfig _toptConfig;

        public TotpHelper(IOptions<TotpConfig> toptConfig) {

            _toptConfig = toptConfig.Value;

        }

        public SetupCode GenerateSetupCode(string companyId, string userId, int QRPixelsPerModule = 3) {
            companyId = companyId.ToUpper();
            userId = userId.ToUpper();

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();

            var setupInfo = twoFactor.GenerateSetupCode(_toptConfig.Issuer, $"{userId}@{companyId}", GetTwoFactorKey(companyId, userId), false, QRPixelsPerModule);

            return new() {
                Account = setupInfo.Account,
                ManualEntryKey = setupInfo.ManualEntryKey,
                QrCodeSetupImageUrl = setupInfo.QrCodeSetupImageUrl
            };
        }

        public bool ValidatePIN(string companyId, string userId, string twoFactorCodeFromClient) {
            companyId = companyId.ToUpper();
            userId = userId.ToUpper();

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(GetTwoFactorKey(companyId, userId), 
                twoFactorCodeFromClient,
                new System.TimeSpan(0, 0, _toptConfig.TimeToleranceForSeconds/2));

            return isValid;
        }


        private string GetTwoFactorKey(string companyId, string userId) {
            companyId = companyId.ToUpper();
            userId = userId.ToUpper();

            return $"{_toptConfig.Issuer}{_toptConfig.IVforAccountSecretKey}{companyId}{userId}";
        }
    }
}
