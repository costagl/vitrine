using VitrineApi.Interfaces;

public class LojaMiddleware
{
    private readonly RequestDelegate _next;

    public LojaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILojaService lojaService)
    {
        var host = context.Request.Host.Host; // exemplo: loja123.vitrine.com

        if (host == "localhost")
        {
            await _next(context);
            return;
        }

        var subdomain = host.Split('.')[0];

        var loja = await lojaService.BuscarPorSubdominio(subdomain);
        if (loja == null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Loja não encontrada");
            return;
        }

        context.Items["Loja"] = loja;

        await _next(context);
    }
}