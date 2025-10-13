using HqSrv.Domain.Events;
using System.Threading.Tasks;

namespace HqSrv.Domain.Services
{
    /// <summary>
    /// 領域事件服務
    /// </summary>
    public interface IDomainEventService
    {
        Task PublishAsync(IDomainEvent domainEvent);
        Task PublishMultipleAsync(params IDomainEvent[] domainEvents);
    }
}