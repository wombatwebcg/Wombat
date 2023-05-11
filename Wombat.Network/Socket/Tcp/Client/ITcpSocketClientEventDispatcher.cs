using System.Threading.Tasks;

namespace Wombat.Network.Socket
{
    public interface ITcpSocketClientEventDispatcher
    {
        Task OnServerConnected(TcpSocketClient client);

        Task OnServerConnecting(TcpSocketClient client);

        Task OnServerDataReceived(TcpSocketClient client, byte[] data, int offset, int count);

        Task OnServerDisconnecting(TcpSocketClient client);

        Task OnServerDisconnected(TcpSocketClient client);
    }
}
