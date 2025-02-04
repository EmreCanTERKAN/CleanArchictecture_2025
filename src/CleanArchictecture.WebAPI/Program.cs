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
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Microsoft loglarýný azalt
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
//Openapi kullanýlmasý için mutlaka cors politikasý eklemek gerekiyor.
builder.Services.AddCors();
builder.Services.AddOpenApi();

//Odatayý kullanabilmek için AddOData metodunu ekledik.
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
    x.AddFixedWindowLimiter("fixed", cfg => // "Fixed Window" stratejisiyle bir rate limiting politikasý ekler.
    {
        cfg.QueueLimit = 100; // Kuyruða alýnabilecek maksimum istek sayýsýný belirler (100 istek).
        cfg.Window = TimeSpan.FromMinutes(1); // Zaman penceresini 1 dakika olarak ayarlar.
        cfg.PermitLimit = 100; // 1 dakika içinde izin verilen maksimum istek sayýsýný belirler (100 istek).
        cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Kuyruktaki isteklerin iþlenme sýrasýný belirler (en eski istekler önce iþlenir).
    }));

builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();
var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapDefaultEndpoints();

//eðer signalR ile çalýþmamýz gerekiyorsa AllowCredentials izin vermemiz gerekiyor.
app.UseCors(x => x
.AllowAnyHeader()
.AllowCredentials()
.AllowAnyMethod()
.SetIsOriginAllowed(t => true));

app.MapControllers().RequireRateLimiting("fixed");

app.RegisterRoutes();

app.UseExceptionHandler();

app.Run();
