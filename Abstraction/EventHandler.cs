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
        {
            var handleState = false;
            if (CanHandle(@event))
            {
                try
                {
                    handleState = HandleAsync((T)@event, cancellationToken).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            
            return Task.FromResult(handleState);
        }
    }
}
