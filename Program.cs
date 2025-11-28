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

// Configuração de CORS
var allowedOrigin = builder.Configuration["Jwt:Audience"]; // O domínio do front-end

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        // Política CORS restrita ao domínio do front-end
        policy.WithOrigins(allowedOrigin) // Permite apenas o domínio configurado no appsettings.json
              .AllowAnyHeader()            // Permite qualquer cabeçalho
              .AllowAnyMethod()            // Permite qualquer método HTTP
              .AllowCredentials();         // Permite enviar cookies, caso necessário
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
                token = token.Replace("\\", "").Replace("\"", "");
                context.Request.Headers["Authorization"] = token;
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"erro\":\"Token inválido ou ausente\"}");
            }
        };
    })
    .AddCookie();

// Configuração de cookies de autenticação
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

// Middleware para encaminhamento de cabeçalhos
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configuração do pipeline de requisições
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aplicando o CORS antes de qualquer outro middleware
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Middleware de erro global
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"erro\":\"{ex.Message}\"}}");
    }
});

app.UseRouting();

// Aplicação do middleware de RateLimiter
app.UseMiddleware<RateLimiterMiddleware>();

app.MapControllers();

app.Run();
