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
using CleanArchictecture.WebAPI.Middlewares;


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

builder.Services.AddRateLimiter(x => 
    x.AddFixedWindowLimiter("fixed", cfg => 
    {
        cfg.QueueLimit = 100; 
        cfg.Window = TimeSpan.FromMinutes(1); 
        cfg.PermitLimit = 100; 
        cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; 
    }));

builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();
var app = builder.Build();

app.UseExceptionHandler(); 
app.UseCors(x => x
    .AllowAnyHeader()
    .AllowCredentials()
    .AllowAnyMethod()
    .SetIsOriginAllowed(t => true));

app.UseRouting();  

app.UseAuthentication();  
app.UseAuthorization();   

app.MapControllers().RequireRateLimiting("fixed").RequireAuthorization();  

app.MapOpenApi();
app.MapScalarApiReference();
app.MapDefaultEndpoints();
app.RegisterRoutes();  

ExtensionsMiddleware.CreateFirstUser(app);

app.Run();
