

using System.Net.Sockets;
using System.Text;
using System.Web;

public class Request
{
    public Method Method { get; private set; }

    public string Url { get; private set; }

    public HeaderCollection Headers { get;private set; }

    public CookieCollection Cookies { get; private set; }

    public string Body { get;private set; }

    public IReadOnlyDictionary<string, string> Form { get; private set; }

    public static Request Parse(string request)
    {
        var lines = request.Split("\r\n");

        var startLine = lines.First().Split(" ");

        var method = ParseMethod(startLine[0]);

        var url = startLine[1];

        var headers = ParseHeaders(lines.Skip(1));

        var cookies = ParseCookies(headers);

        var bodyLines = lines.Skip(headers.Count + 2).ToArray();

        var body = string.Join("\r\n", bodyLines);

        var form = ParseForm(headers, body);

        return new Request
                   {
                       Method = method,
                       Url = url, 
                       Headers = headers,
                       Cookies = cookies,
                       Body = body,
                       Form = form
                   };
    }

    private static CookieCollection ParseCookies(HeaderCollection headers)
    {
        var cookieCollection = new CookieCollection();

        if (headers.Contains(Header.Cookie))
        {

            var cookieHeader = headers[Header.Cookie];

            var allCookies = cookieHeader.Split(';');

            foreach (var cookieText in allCookies)
            {
                
                var cookieParts = cookieText.Split('=');

                var cookieName = cookieParts[0];
                var cookieValue =  cookieParts[1];

                cookieCollection.Add(cookieName,cookieValue);

            }


        }
        
        return cookieCollection;

    }

    private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
    {
        var formCollection = new Dictionary<string, string>();

        if (headers.Contains(Header.ContentType) &&
            headers[Header.ContentType] == ContentType.FormUrlEncoded)
        {
            var parsedResult = ParseFormData(body);

            foreach (var (name, value) in parsedResult)
            {
                formCollection.Add(name, value);
            }
        }

        return formCollection;
    }

    private static Dictionary<string, string> ParseFormData(string bodyLines) =>
        HttpUtility.UrlDecode(bodyLines).Split('&').Select(part => part.Split('=')).Where(part => part.Length == 2)
            .ToDictionary(part => part[0], part => part[1], StringComparer.InvariantCultureIgnoreCase);

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

        var totalBytes = 0;

        var requestBuilder = new StringBuilder();

        do
        {
            var bytesRead = networkStream.Read(buffer, 0, bufferLength);

            totalBytes += bytesRead;

            if (totalBytes > 10 * 1024)
            {
                throw new InvalidOperationException("Request is too large.");
            }

            requestBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

        }
        while (networkStream.DataAvailable);

        return requestBuilder.ToString();


    }

}