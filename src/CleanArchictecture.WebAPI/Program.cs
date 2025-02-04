using CleanArchictecture.Infrastructure;
using CleanArchictecture.Application;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.OData;
using CleanArchictecture.WebAPI.Controllers;
using CleanArchictecture.WebAPI.Modules;
using CleanArhictecture_2025.WebAPI;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Microsoft loglar�n� azalt
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Debug() // Genel minimum log seviyesi
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}"
    )
    .WriteTo.Seq("http://localhost:5341") // Seq sunucu URL
    .CreateLogger();


builder.AddServiceDefaults();

builder.Services.AddApplicaton();
builder.Services.AddInfrastructure(builder.Configuration);
//Openapi kullan�lmas� i�in mutlaka cors politikas� eklemek gerekiyor.
builder.Services.AddCors();
builder.Services.AddOpenApi();

//Odatay� kullanabilmek i�in AddOData metodunu ekledik.
builder.Services.AddControllers().AddOData(opt => 
        opt
        .Select()
        .Filter()
        .Count()
        .Expand()
        .OrderBy()
        .SetMaxTop(null)
        .AddRouteComponents("odata",AppODataController.GetEdmModel()));

builder.Services.AddRateLimiter(x => // Rate Limiter servisini uygulamaya ekler.
    x.AddFixedWindowLimiter("fixed", cfg => // "Fixed Window" stratejisiyle bir rate limiting politikas� ekler.
    {
        cfg.QueueLimit = 100; // Kuyru�a al�nabilecek maksimum istek say�s�n� belirler (100 istek).
        cfg.Window = TimeSpan.FromMinutes(1); // Zaman penceresini 1 dakika olarak ayarlar.
        cfg.PermitLimit = 100; // 1 dakika i�inde izin verilen maksimum istek say�s�n� belirler (100 istek).
        cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Kuyruktaki isteklerin i�lenme s�ras�n� belirler (en eski istekler �nce i�lenir).
    }));

builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();
var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapDefaultEndpoints();

//e�er signalR ile �al��mam�z gerekiyorsa AllowCredentials izin vermemiz gerekiyor.
app.UseCors(x => x
.AllowAnyHeader()
.AllowCredentials()
.AllowAnyMethod()
.SetIsOriginAllowed(t => true));

app.MapControllers().RequireRateLimiting("fixed");

app.RegisterRoutes();

app.UseExceptionHandler();

app.Run();
