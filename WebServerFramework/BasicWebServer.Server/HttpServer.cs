

using System.Net;
using System.Net.Sockets;
using System.Text;

public class HttpServer
{
    private readonly IPAddress ipAddress;

    private readonly int port;

    private readonly TcpListener serverListener;

    public HttpServer(string ipAdress, int port)
    {

        this.ipAddress = IPAddress.Parse(ipAdress);
        this.port = port;

        this.serverListener = new TcpListener(this.ipAddress, port);

    }

    public void Start()
    {
        this.serverListener.Start();
        while (true)
        {
            

            var connection = serverListener.AcceptTcpClient();

            var networkStream = connection.GetStream();

            WriteResponse(networkStream, "Hello from the server!");

            connection.Close();
        }

    }

    private void WriteResponse(NetworkStream networkStream, string message)
    {
      
        var contentLength = Encoding.UTF8.GetByteCount(message);

        var response = $@"HTTP/1.1 200 OK
Content-Type: text/plain; charset=UTF-8
Content-Length: {contentLength}

{message}";

        var responseBytes = Encoding.UTF8.GetBytes(response);

        networkStream.Write(responseBytes);

    }
}