using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.Text;
using System.Text.Json.Serialization;
using VitrineApi.Data;
using VitrineApi.Interfaces;
using VitrineApi.Mappings;
using VitrineApi.Models;
using VitrineApi.Validators;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/usuario/login"; // qualquer rota inexistente ou customizada
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


var allowedOrigin = builder.Configuration["Jwt:Audience"];
var vercelSite = builder.Configuration["Jwt:Vercel"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigin, vercelSite, "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        //policy.AllowAnyOrigin() 
        //      .AllowAnyHeader()
        //      .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<VitrineDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VitrineDB"))
           .EnableSensitiveDataLogging()); // DESATIVAR EM PRODUÇÃO

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

//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ILojaService, LojaService>();
builder.Services.AddScoped<Cpf_CnpjValidator>();
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<RepositoryBase<Lojista>>();
builder.Services.AddScoped<RepositoryBase<Loja>>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseRouting();


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

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"erro\":\"Não autorizado\"}");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LojaMiddleware>();

app.MapControllers();

app.Run();
