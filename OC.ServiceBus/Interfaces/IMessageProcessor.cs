using System.Threading.Tasks;

namespace OC.ServiceBus.Interfaces
{
    public interface IMessageProcessor
    {
        Task ProcessMessageAsync(string message);
    }
}
