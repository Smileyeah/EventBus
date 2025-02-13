using System.Collections.Concurrent;
using GWCloudPortal.EventBus.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.CompilerServices;

namespace GWCloudPortal.EventBus.Default
{
    public class EventHandlerExecutionContext : IEventHandlerExecutionContext
    {
        private readonly IServiceCollection _registry;
        private readonly Func<IServiceCollection, IServiceProvider> _serviceProviderFactory;
        private readonly ConcurrentDictionary<Type, List<Type>> _registrations = new ConcurrentDictionary<Type, List<Type>>();

        public EventHandlerExecutionContext(IServiceCollection registry, 
            Func<IServiceCollection, IServiceProvider> serviceProviderFactory = null)
        {
            this._registry = registry;
            this._serviceProviderFactory = serviceProviderFactory ?? (sc => registry.BuildServiceProvider());
        }

        public async Task HandleEventAsync(IEvent @event, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventType = @event.GetType();
            if (this._registrations.TryGetValue(eventType, out List<Type> handlerTypes) &&
                handlerTypes?.Count > 0)
            {
                var serviceProvider = this._serviceProviderFactory(this._registry);
                using var childScope = serviceProvider.CreateScope();
                foreach(var handlerType in handlerTypes)
                {
                    var handler = (IEventHandler)childScope.ServiceProvider.GetService(handlerType);
                    if (handler.CanHandle(@event))
                    {
                        await handler.HandleAsync(@event, cancellationToken);
                    }
                }
            }
        }

        public bool HandlerRegistered<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>
            => this.HandlerRegistered(typeof(TEvent), typeof(THandler));

        public bool HandlerRegistered(Type eventType, Type handlerType)
        {
            if (this._registrations.TryGetValue(eventType, out List<Type> handlerTypeList))
            {
                return handlerTypeList != null && handlerTypeList.Contains(handlerType);
            }

            return false;
        }

        public void RegisterHandler<TEvent, THandler>()
            where TEvent : IEvent
            where THandler : IEventHandler<TEvent>
            => this.RegisterHandler(typeof(TEvent), typeof(THandler));

        public void RegisterHandler(Type eventType, Type handlerType)
        {
            ConcurrentDictionarySafeRegister(eventType, handlerType, this._registrations);
            this._registry.AddTransient(handlerType);
        }

        private static void ConcurrentDictionarySafeRegister<TKey, TValue>(TKey key, TValue value, ConcurrentDictionary<TKey, List<TValue>> registry)
        {
            if (registry.TryGetValue(key, out List<TValue> registryItem))
            {
                if (registryItem != null)
                {
                    if (!registryItem.Contains(value))
                    {
                        registry[key].Add(value);
                    }
                }
                else
                {
                    registry[key] = new List<TValue> { value };
                }
            }
            else
            {
                registry.TryAdd(key, new List<TValue> { value });
            }
        }
    }
}
