namespace POVWebDomain.Common
{
    public class Error
    {
        public string Code { get; }
        public string Message { get; }

        private Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        // 預定義的常見錯誤
        public static readonly Error None = new Error(string.Empty, string.Empty);
        public static readonly Error NullValue = new Error("NULL_VALUE", "值不可為空");
        public static readonly Error NotFound = new Error("NOT_FOUND", "找不到資源");
        public static readonly Error ValidationFailed = new Error("VALIDATION_FAILED", "驗證失敗");
        public static readonly Error ExternalApiError = new Error("EXTERNAL_API_ERROR", "外部API呼叫失敗");

        // 建立自訂錯誤
        public static Error Custom(string code, string message) => new Error(code, message);
        public static Error FromMessage(string message) => new Error("ERROR", message);
    }
}