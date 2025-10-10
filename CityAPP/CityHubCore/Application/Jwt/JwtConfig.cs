namespace CityHubCore.Application.Jwt {
    public class JwtConfig {
        public string HeaderName { get; set; }
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiresInMinutes { get; set; }
        public string RefreshTokenSecret { get; set; }
        public int RefreshTokenExpiresInHours { get; set; }
        public int RefreshTokenQueueLimit { get; set; }
        public int SessionIDExpiresInHours { get; set; }
    }
}
