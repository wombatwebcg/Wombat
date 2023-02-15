using System.Threading.Tasks;

namespace Wombat.Sockets
{
    public interface ITcpSocketSaeaClientEventDispatcher
    {
        Task OnServerConnected(TcpSocketSaeaClient client);
        Task OnServerDataReceived(TcpSocketSaeaClient client, byte[] data, int offset, int count);
        Task OnServerDisconnected(TcpSocketSaeaClient client);
    }
}
