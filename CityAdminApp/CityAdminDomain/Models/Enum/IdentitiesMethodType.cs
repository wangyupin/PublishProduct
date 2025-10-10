namespace CityAdminDomain.Models.Enum {
    /// <summary>
    /// 
    /// </summary>
    public enum IdentitiesMethodType {
        PASSWORD,
        PBKDF2, // REF: https://cmatskas.com/-net-password-hashing-using-pbkdf2/
        TOPT,
        POVSQL, //使用旌泓POS驗證
        FB
    }
}
