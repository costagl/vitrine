using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using VitrineApi.Data;
using VitrineApi.Helpers;
using VitrineApi.Interfaces;
using VitrineApi.Mappings;
using VitrineApi.Models;
using VitrineApi.Services;
using VitrineApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. CONFIGURAÇÃO DOS SERVIÇOS (DI Container)
// ============================================

// Configuração para o Nginx (Proxy Reverso)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Limpar redes conhecidas é crucial para funcionar em VPS/Docker/Debian onde o IP interno muda
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Configuração de CORS
var allowedOrigin = builder.Configuration["Jwt:Audience"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configuração de autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = builder.Configuration;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    token = token.Replace("\\", "").Replace("\"", "");
                    context.Request.Headers["Authorization"] = token;
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"erro\":\"Por favor, faça login novamente.\"}");
            }
        };
    })
    .AddCookie();

// Configuração de cookies de Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/usuario/login";
    options.AccessDeniedPath = "/usuario/login";
    options.SlidingExpiration = true;

    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"erro\":\"Não autorizado - login requerido\"}");
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync("{\"erro\":\"Acesso negado\"}");
    };
});

// Configuração do DbContext
builder.Services.AddDbContext<VitrineDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VitrineDB")));

// Configuração do Identity
builder.Services.AddIdentity<LojistaAuth, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddPasswordValidator<CustomPasswordValidator>()
    .AddEntityFrameworkStores<VitrineDBContext>()
    .AddDefaultTokenProviders();

// Adicionar Controllers e configurar JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Adicionar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Injeção de dependência
builder.Services.AddScoped<DbEsgotado>();
builder.Services.AddScoped<ILojaService, LojaService>();
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<Cpf_CnpjValidator>();
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<RepositoryBase<Lojista>>();
builder.Services.AddScoped<RepositoryBase<Loja>>();

// AutoMapper
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

var app = builder.Build();

// ===============================
// 2. PIPELINE DE MIDDLEWARES
// ===============================

// 1. Processar headers do Nginx
app.UseForwardedHeaders();

// 2. Tratamento Global de Erros
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro Interno: {ex.Message}");

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var mensagemErro = app.Environment.IsDevelopment() ? ex.Message : "Ocorreu um erro interno no servidor.";
        await context.Response.WriteAsync($"{{\"erro\":\"{mensagemErro}\"}}");
    }
});

// 3. Swagger (Apenas desenvolvimento)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. Redirecionamento HTTPS
app.UseHttpsRedirection();

// 5. Roteamento
app.UseRouting();

// 6. CORS
app.UseCors("CorsPolicy");

// 7. Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();

// 8. Rate Limiter
app.UseMiddleware<RateLimiterMiddleware>();

// 9. Mapeamento dos Endpoints
app.MapControllers();

app.Run();