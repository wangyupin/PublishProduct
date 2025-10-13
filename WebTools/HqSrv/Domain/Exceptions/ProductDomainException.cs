using System;

namespace HqSrv.Domain.Exceptions
{
    /// <summary>
    /// 商品領域異常
    /// </summary>
    public class ProductDomainException : Exception
    {
        public ProductDomainException(string message) : base(message) { }
        public ProductDomainException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}