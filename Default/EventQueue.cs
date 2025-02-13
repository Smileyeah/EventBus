using GWCloudPortal.EventBus.Abstraction;
using GWCloudPortal.EventBus.Interface;

namespace GWCloudPortal.EventBus.Default
{
    internal sealed class EventQueue
    {
        public event System.EventHandler<EventProcessedEventArgs> EventPushed;

        public EventQueue() { }

        public void Push(IEvent @event)
        {
            OnMessagePushed(new EventProcessedEventArgs(@event));
        }

        private void OnMessagePushed(EventProcessedEventArgs e) => this.EventPushed?.Invoke(this, e);
    }
}
