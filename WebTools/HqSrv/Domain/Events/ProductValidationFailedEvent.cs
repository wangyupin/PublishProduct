using System;
using System.Collections.Generic;

namespace HqSrv.Domain.Events
{
    /// <summary>
    /// 商品驗證失敗事件
    /// </summary>
    public class ProductValidationFailedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; }
        public string EventType => nameof(ProductValidationFailedEvent);

        public string ProductId { get; private set; }
        public string PlatformId { get; private set; }
        public List<string> ValidationErrors { get; private set; }

        private ProductValidationFailedEvent() { }

        public static ProductValidationFailedEvent Create(string productId, string platformId, List<string> errors)
        {
            return new ProductValidationFailedEvent
            {
                OccurredOn = DateTime.UtcNow,
                ProductId = productId,
                PlatformId = platformId,
                ValidationErrors = errors ?? new List<string>()
            };
        }
    }
}