using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System; // Adicionado para DateTimeOffset

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
        // Usa o endereço IP como chave
        string clientIp = context.Connection.RemoteIpAddress.ToString();

        // Obtém ou cria o contador para o IP do cliente
        var requestCounter = _requestCounters.GetOrAdd(clientIp, new RequestCounter());

        if (requestCounter.IsRequestAllowed(_maxRequestsPerMinute, _timeWindowInSeconds))
        {
            // Se a requisição for permitida, prossiga para o próximo middleware
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

    // Objeto de lock para garantir thread-safety
    private readonly object _lock = new object();

    // O método foi corrigido para ser thread-safe
    public bool IsRequestAllowed(int maxRequests, int timeWindowInSeconds)
    {
        // Usa lock para garantir que a leitura e a escrita das variáveis sejam atômicas
        lock (_lock)
        {
            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Lógica de Janela Fixa: Se a janela expirou ou é o primeiro request, resetamos.
            // Usamos '>=' para maior robustez em comparação com apenas '>'
            if (_firstRequestTimestamp == 0 || currentTime - _firstRequestTimestamp >= timeWindowInSeconds)
            {
                _firstRequestTimestamp = currentTime;
                _requestCount = 0;
            }

            // Se o contador estiver abaixo do limite, permite e incrementa
            if (_requestCount < maxRequests)
            {
                _requestCount++;
                return true;
            }

            // Limite atingido
            return false;
        }
    }
}