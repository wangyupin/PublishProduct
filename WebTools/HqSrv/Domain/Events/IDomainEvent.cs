using System;

namespace HqSrv.Domain.Events
{
    /// <summary>
    /// 領域事件基底介面
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
        string EventType { get; }
    }
}
