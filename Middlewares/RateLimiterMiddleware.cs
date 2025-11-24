//using Microsoft.AspNetCore.Http;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Threading.Tasks;

//public class RateLimiterMiddleware
//{
//    private readonly RequestDelegate _next;
//    private static ConcurrentDictionary<string, RequestCounter> _requestCounters = new ConcurrentDictionary<string, RequestCounter>();
//    private readonly int _maxRequestsPerMinute = 1; // Número máximo de requisições por IP por minuto
//    private readonly int _timeWindowInSeconds = 5; // Janela de tempo (60 segundos)

//    public RateLimiterMiddleware(RequestDelegate next)
//    {
//        _next = next;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        string clientIp = context.Connection.RemoteIpAddress.ToString();
//        var requestCounter = _requestCounters.GetOrAdd(clientIp, new RequestCounter());

//        if (requestCounter.IsRequestAllowed(_maxRequestsPerMinute, _timeWindowInSeconds))
//        {
//            // Se a requisição for permitida, prossiga
//            await _next(context);
//        }
//        else
//        {
//            // Caso o limite seja atingido, retorne uma resposta de erro
//            context.Response.StatusCode = 429; // Too Many Requests
//            await context.Response.WriteAsync("Too many requests. Please try again later.");
//        }
//    }
//}

//public class RequestCounter
//{
//    private int _requestCount = 0;
//    private long _firstRequestTimestamp = 0;

//    public bool IsRequestAllowed(int maxRequests, int timeWindowInSeconds)
//    {
//        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

//        // Se for o primeiro pedido ou a janela de tempo expirou, reinicia o contador
//        if (_firstRequestTimestamp == 0 || currentTime - _firstRequestTimestamp > timeWindowInSeconds)
//        {
//            _firstRequestTimestamp = currentTime;
//            _requestCount = 0;
//        }

//        // Verifica se a quantidade de requisições excedeu o limite
//        if (_requestCount < maxRequests)
//        {
//            _requestCount++;
//            return true;
//        }

//        return false;
//    }
//}
