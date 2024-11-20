using System.Threading.Tasks;

namespace NetCoreClient.Protocols
{
    public interface IProtocolInterface
    {
        void Send(string data, string sensor);
    }
}
