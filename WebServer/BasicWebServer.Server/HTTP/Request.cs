

using System.Net.Sockets;
using System.Text;

public class Request
{
    public Method Method { get; private set; }

    public string Url { get; private set; }

    public HeaderCollection Headers { get;private set; }

    public string Body { get;private set; }

    public static Request Parse(string request)
    {
        var lines = request.Split("\r\n");

        var startLine = lines.First().Split(" ");

        var method = ParseMethod(startLine[0]);

        var url = startLine[1];

        var headers = ParseHeaders(lines.Skip(1));

        var bodyLines = lines.Skip(headers.Count + 2).ToArray();

        var body = string.Join("\r\n", bodyLines);

        return new Request { Method = method, Url = url, Headers = headers, Body = body };
    }

    private static HeaderCollection ParseHeaders(IEnumerable<string> headerLines)
    {
        var headerCollection = new HeaderCollection();

        foreach (var headerLine in headerLines)
        {

            if (headerLine == string.Empty)
            {
                break;
            }

            var headerParts = headerLine.Split(":", 2);

            if (headerParts.Length != 2)
            {
                throw new InvalidOperationException("Request is not valid");
            }

            var headerName = headerParts[0];
            var headerValue = headerParts[1].Trim();

            headerCollection.Add(headerName,headerValue);
        }

        return headerCollection;
    }

    private static Method ParseMethod(string method)
    {
        try
        {
            return (Method)Enum.Parse(typeof(Method), method, true);
        }
        catch (Exception )
        {
            throw new InvalidOperationException($"Method '{method}' is not supported");
        }

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