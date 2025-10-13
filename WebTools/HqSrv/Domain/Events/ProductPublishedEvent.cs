using HqSrv.Domain.Entities;
using System;

namespace HqSrv.Domain.Events
{
    /// <summary>
    /// 商品發布成功事件
    /// </summary>
    public class ProductPublishedEvent : IDomainEvent
    {
        public DateTime OccurredOn { get; private set; }
        public string EventType => nameof(ProductPublishedEvent);

        public string ProductId { get; private set; }
        public string PlatformId { get; private set; }
        public string PublishResult { get; private set; }

        private ProductPublishedEvent() { }

        public static ProductPublishedEvent Create(string productId, string platformId, string publishResult)
        {
            return new ProductPublishedEvent
            {
                OccurredOn = DateTime.UtcNow,
                ProductId = productId,
                PlatformId = platformId,
                PublishResult = publishResult
            };
        }
    }
}