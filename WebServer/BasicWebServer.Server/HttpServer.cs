
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
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

        Console.WriteLine($"Server started on port {this.port}.");
        Console.WriteLine("Listening for requests...");
        while (true)
        {
            

            var connection = serverListener.AcceptTcpClient();

            var networkStream = connection.GetStream();

            var requestText = this.ReadRequest(networkStream);

            Console.WriteLine(requestText);

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

    private string ReadRequest(NetworkStream networkStream)
    {

        var bufferLength = 1024;
        var buffer = new byte[bufferLength];

        var requestBuilder = new StringBuilder();

        do
        {
            var bytesRead = networkStream.Read(buffer, 0, bufferLength);

            requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

        }
        while (networkStream.DataAvailable);

        return requestBuilder.ToString();


    }
}