namespace GWCloudPortal.EventBus.Interface
{
    public interface IEventPublisher : IDisposable
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent;
    }
}
