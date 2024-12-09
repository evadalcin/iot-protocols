using System.Threading.Tasks;

namespace NetCoreClient.Protocols
{
    public interface IProtocolInterface
    {
        Task Send(string data, string sensor);
    }
}
