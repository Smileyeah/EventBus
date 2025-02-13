using GWCloudPortal.EventBus.Interface;

namespace GWCloudPortal.EventBus.Abstraction
{
    public class EventProcessedEventArgs : EventArgs
    {
        public EventProcessedEventArgs(IEvent @event)
        {
            this.Event = @event;
        }

        public IEvent Event { get; }

    }
}
