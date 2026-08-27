using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Resolves and invokes registered <see cref="IEventHandler{TEvent}"/> instances synchronously.
    /// No message broker needed for a portfolio project, but the <see cref="IEventPublisher"/>
    /// interface leaves room for swapping in one later.
    /// </summary>
    public class InMemoryEventPublisher : IEventPublisher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryEventPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class
        {
            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}
