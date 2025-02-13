using GWCloudPortal.EventBus.Interface;

namespace GWCloudPortal.EventBus.Abstraction
{
    public abstract class EventHandler<T> : IEventHandler<T>
        where T : IEvent
    {
        public bool CanHandle(IEvent @event)
            => typeof(T) == @event.GetType();

        public abstract Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default);

        public Task<bool> HandleAsync(IEvent @event, CancellationToken cancellationToken = default)
            => CanHandle(@event) ? HandleAsync((T)@event, cancellationToken) : Task.FromResult(false);
    }
}
