using GWCloudPortal.EventBus.Abstraction;
using GWCloudPortal.EventBus.Interface;

namespace GWCloudPortal.EventBus.Default
{
    public sealed class PassThroughEventBus : IEventBus
    {
        private readonly EventQueue _eventQueue = new ();
        private readonly IEventHandlerExecutionContext _context;

        public PassThroughEventBus(IEnumerable<IEventHandler> eventHandlers, IEventHandlerExecutionContext context)
        {
            _context = context;
            _eventQueue.EventPushed += EventQueue_EventPushed;
        }

        private async void EventQueue_EventPushed(object? sender, EventProcessedEventArgs e)
            => await this._context.HandleEventAsync(e.Event);

        public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
            where TEvent : IEvent
                => Task.Factory.StartNew(() => _eventQueue.Push(@event), cancellationToken);

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            if (!this._context.HandlerRegistered<TEvent, TEventHandler>())
            {
                this._context.RegisterHandler<TEvent, TEventHandler>();
            }
        }


        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls
        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    this._eventQueue.EventPushed -= EventQueue_EventPushed;
                }

                _disposedValue = true;
            }
        }
        public void Dispose() => Dispose(true);
        #endregion
    }
}
