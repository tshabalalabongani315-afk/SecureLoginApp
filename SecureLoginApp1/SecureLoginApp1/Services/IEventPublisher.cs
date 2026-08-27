using System.Threading.Tasks;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Publishes a domain event to every registered <see cref="IEventHandler{TEvent}"/>.
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent) where TEvent : class;
    }
}
