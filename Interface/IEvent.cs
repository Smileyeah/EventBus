namespace GWCloudPortal.EventBus.Interface
{
    public interface IEvent
    {
        Guid Id { get; }

        DateTime Timestamp { get; }
    }
}
