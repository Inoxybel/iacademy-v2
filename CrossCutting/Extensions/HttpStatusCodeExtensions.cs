using System.Net;

namespace CrossCutting.Extensions;

public static class HttpStatusCodeExtensions
{
    public static bool Is4XX(this HttpStatusCode statusCode) =>
        (int)statusCode >= 400 && (int)statusCode < 500;
}