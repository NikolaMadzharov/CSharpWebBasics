﻿

public class Response
{
    public Response(StatusCode statusCode)
    {
        this.StatusCode = statusCode;

        this.Headers.Add("Server", "My web Server");
        this.Headers.Add("Date", $"{DateTime.UtcNow:r}");
    }

    public StatusCode StatusCode { get; init; }

    public HeaderCollection Headers { get; } = new HeaderCollection();

    public string Body { get; set; }

}