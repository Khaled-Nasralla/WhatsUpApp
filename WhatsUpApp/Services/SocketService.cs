using Microsoft.Maui.Controls;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class SocketService : IDisposable
{
    private static SocketService _instance;
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _networkStream;
    private static readonly object _lock = new object();

    // Private constructor to prevent instantiation from outside
    private SocketService(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _networkStream = _tcpClient.GetStream();
    }

    // Public method to get the instance of SocketService (Singleton)
    public static async Task<SocketService> InstanceAsync(string serverAddress = "127.0.0.1", int port = 5000)
    {
        if (_instance == null)
        {
            await InitializeInstanceAsync(serverAddress, port);
        }
        return _instance;
    }

    // Method to initialize the singleton instance
    private static async Task InitializeInstanceAsync(string serverAddress, int port)
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(serverAddress, port);
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = new SocketService(tcpClient);
            }
        }
    }

    // Method to send requests and receive responses
    public async Task<string> SendRequestAsync(string request)
    {
        var buffer = new byte[1024];
        try
        {
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            await _networkStream.WriteAsync(requestBytes, 0, requestBytes.Length);

            int bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        catch (Exception ex)
        {
            throw new Exception($"Socket error: {ex.Message}", ex);
        }
    }

    // Dispose the socket service and close the connection
    public void Dispose()
    {
        _networkStream?.Dispose();
        _tcpClient?.Close();
        _instance = null;  // Clear the instance when disposed
    }


}
