using HqSrv.Domain.Events;
using HqSrv.Domain.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace HqSrv.Infrastructure.Services
{
    /// <summary>
    /// 領域事件服務實作
    /// </summary>
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> _logger;

        public DomainEventService(ILogger<DomainEventService> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync(IDomainEvent domainEvent)
        {
            try
            {
                _logger.LogInformation($"發布領域事件: {domainEvent.EventType} at {domainEvent.OccurredOn}");

                // 這裡可以整合訊息佇列、Event Bus 等
                // 目前先記錄日誌
                switch (domainEvent)
                {
                    case ProductPublishedEvent publishedEvent:
                        _logger.LogInformation($"商品 {publishedEvent.ProductId} 成功發布到平台 {publishedEvent.PlatformId}");
                        break;

                    case ProductValidationFailedEvent validationFailedEvent:
                        _logger.LogWarning($"商品 {validationFailedEvent.ProductId} 在平台 {validationFailedEvent.PlatformId} 驗證失敗: {string.Join(", ", validationFailedEvent.ValidationErrors)}");
                        break;
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"發布領域事件失敗: {domainEvent.EventType}");
            }
        }

        public async Task PublishMultipleAsync(params IDomainEvent[] domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                await PublishAsync(domainEvent);
            }
        }
    }
}