using System.Threading.Tasks;

namespace SecureLoginApp1.Services
{
    /// <summary>
    /// Reacts to a single domain event type. Register one implementation per (event, concern) pair
    /// so PageModels stay free of side-effect logic like activity logging.
    /// </summary>
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task HandleAsync(TEvent domainEvent);
    }
}
