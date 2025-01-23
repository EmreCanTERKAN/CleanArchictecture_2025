using CleanArchictecture.Infrastructure;
using CleanArchictecture.Application;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.OData;
using CleanArchictecture.WebAPI.Controllers;
using CleanArchictecture.WebAPI.Modules;
using CleanArhictecture_2025.WebAPI;


var builder = WebApplication.CreateBuilder(args);

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
