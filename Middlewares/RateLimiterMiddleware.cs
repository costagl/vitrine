using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    // O dicionário armazena o RequestCounter para cada IP
    private static ConcurrentDictionary<string, RequestCounter> _requestCounters = new ConcurrentDictionary<string, RequestCounter>();

    private readonly int _maxRequestsPerMinute = 200;
    private readonly int _timeWindowInSeconds = 60;

    public RateLimiterMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string clientIp = context.Connection.RemoteIpAddress.ToString();

        var requestCounter = _requestCounters.GetOrAdd(clientIp, new RequestCounter());

        if (requestCounter.IsRequestAllowed(_maxRequestsPerMinute, _timeWindowInSeconds))
        {
            await _next(context);
        }
        else
        {
            // Caso o limite seja atingido, retorna o código 429
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("Too many requests. Please try again later.");
        }
    }
}

public class RequestCounter
{
    private int _requestCount = 0;
    private long _firstRequestTimestamp = 0;

    private readonly object _lock = new object();

    public bool IsRequestAllowed(int maxRequests, int timeWindowInSeconds)
    {
        lock (_lock)
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (_firstRequestTimestamp == 0 || currentTime - _firstRequestTimestamp >= timeWindowInSeconds)
            {
                _firstRequestTimestamp = currentTime;
                _requestCount = 0;
            }

            if (_requestCount < maxRequests)
            {
                _requestCount++;
                return true;
            }

            return false;
        }
    }
}