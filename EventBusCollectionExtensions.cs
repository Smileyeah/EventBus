using GWCloudPortal.EventBus.Default;
using GWCloudPortal.EventBus.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace GWCloudPortal.EventBus;

public static class EventBusCollectionExtensions
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        var eventHandlerExecutionContext = new EventHandlerExecutionContext(services, 
            sc => sc.BuildServiceProvider());
        
        services.AddSingleton<IEventHandlerExecutionContext>(eventHandlerExecutionContext);
        services.AddSingleton<IEventBus, PassThroughEventBus>();

        return services;
    }
}