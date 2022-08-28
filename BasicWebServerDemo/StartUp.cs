using System.Text;
using System.Web;

public class Startup
{
    private const string HtmlForm = @"<form action='/HTML' method='POST'>
   Name: <input type='text' name='Name'/>
   Age: <input type='number' name ='Age'/>
<input type='submit' value ='Save' />
</form>";




    private const string DownloadForm = @"<form action='/Content' method='POST'>
   <input type='submit' value ='Download Sites Content' /> 
</form>";

    private const string FileName = "content.txt";


    private const string LoginForm = @"<form action='/Login' method='POST'>
   Username: <input type='text' name='Username'/>
   Password: <input type='text' name='Password'/>
   <input type='submit' value ='Log In' /> 
</form>";

    private const string Username = "user";

    private const string Password = "user123";

  


    public static async Task Main()
    {
        var server = new HttpServer(
            routes => routes.MapGet("/", new TextResponse("Hello from the server!"))
                .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                .MapGet("/HTML", new HtmlResponse(HtmlForm))
                .MapPost("/HTML", new TextResponse(string.Empty, AddFormDataAction))
                .MapGet("/Content", new HtmlResponse(DownloadForm)).MapPost("/Content", new TextFileResponse(FileName))
                .MapGet("/Cookies", new HtmlResponse("", Startup.AddCookiesAction))
                .MapGet("/Cookies", new HtmlResponse("<h1>Cookies set</h1>")).MapGet(
                    "/Session",
                    new TextResponse("", Startup.DisplayInfoAction)).MapGet(
                    "/Login",
                    new HtmlResponse(Startup.LoginForm))
                .MapPost("/Login", new HtmlResponse("", Startup.LoginAction))
      await server.Start();
    }

    private static void LoginAction(Request request, Response response)
    {
        request.Session.Clear();

        var bodyText = "";

        var usernameMatches = request.Form["Username"] == Startup.Username;
        var passwordMatches = request.Form["Password"] == Startup.Password;

        if (usernameMatches && passwordMatches)
        {
            request.Session[Session.SessionUserKey] = "MyUserId";
            response.Cookies.Add(Session.SessionCookieName, request.Session.Id);

            bodyText = "<h3>Logged successfully!</h3>";
        }
    }

    private static void AddFormDataAction(Request request, Response response)
    {
        response.Body = string.Empty;

        foreach (var (key, value) in request.Form)
        {
            response.Body += $"{key} - {value}";
            response.Body += Environment.NewLine;
        }
    }

    private static async Task<string> DownloadWebSiteContent(string url)
    {
        var httpClient = new HttpClient();
        using (httpClient)
        {
            var response = await httpClient.GetAsync(url);

            var html = await response.Content.ReadAsStringAsync();

            return html.Substring(0, 2000);
        }
    }

    //private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
    //{
    //    var downloads = new List<Task<string>>();

    //    foreach (var url in downloads)
    //        downloads.Add(DownloadWebSiteContent(url.ToString()));

    //    var responses = await Task.WhenAll(downloads);

    //    var responsesString = string.Join(Environment.NewLine + new string('-', 100), responses);

    //    await File.WriteAllBytesAsync(fileName, responsesString);
    //}

    private static void AddCookiesAction(Request request, Response response)
    {
        var requestHasCookies = request.Cookies.Any(c => c.Name != Session.SessionCookieName);

        var bodyText = "";

        if (requestHasCookies)
        {
            var cookieText = new StringBuilder();
            cookieText.AppendLine("<h1>Cookies</h1>");

            cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

            foreach (var cookie in request.Cookies)
            {
                cookieText.Append("<tr>");
                cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</tr>");
            }

            cookieText.Append("</table>");

            bodyText = cookieText.ToString();
        }
        else
        {
            bodyText = "<h1>Cookies set!</h1>";
        }

        if (!requestHasCookies)
        {
            response.Cookies.Add("My-Cookie", "My-Value");
            response.Cookies.Add("My-Second-Cookie", "My-Second-Value");
        }

    }

    private static void DisplayInfoAction(Request request, Response response)
    {
        var sessionExists = request.Session.ContainsKey(Session.SessionCurrentDateKey);

        var bodyText = "";

        if (sessionExists)
        {
            var currentDate = request.Session[Session.SessionCurrentDateKey];
            bodyText = $"Stored date: {currentDate}";
        }
        else
        {
            bodyText = $"Current date stored";
        }

        response.Body = "";
        response.Body += bodyText;
    }
}