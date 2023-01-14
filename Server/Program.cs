using Application.Interfaces;
using Infrastructure;
using Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Server;
using Server.Hub;
using System.Reflection;
using Serilog;
using Infrastructure.Services;
using RastrAdapter;
using FluentValidation;
using System;
using FluentValidation.AspNetCore;
using Application.Validation;
using Infrastructure.RabbitMQ;

Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("log.txt").CreateLogger();
Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CalculationResultContext>(options => options.UseNpgsql(connection), ServiceLifetime.Transient);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, @"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\bin\Debug\net6.0\Domain.xml");
    c.IncludeXmlComments(@"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\bin\Debug\net6.0\Server.xml");
});
builder.Services.AddSignalR();
builder.Services.AddScoped<ICalculationResultRepository, CalculationResultRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
var assembly = Assembly.GetAssembly(typeof(MappingProfile));
builder.Services.AddAutoMapper(assembly);
builder.Services.AddScoped<ICalculationService, CalculationService>();
builder.Services.AddScoped<IRastrSchemeInfoService, RastrSchemeInfoService>();
builder.Services.AddScoped<IProcessResultService, ProcessResultService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICalcModel, RastrCOMClient>();
builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
builder.Services.AddSingleton<RabbitMQConsumer>();
builder.Services.AddHostedService<CalculationBackgroundService>();
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CalculationSettingsRequestValidator>());
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();
app.UseSerilogRequestLogging();
app.UseCors(builder => builder.AllowAnyMethod().WithOrigins("http://localhost:3000").AllowAnyHeader().AllowCredentials());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProgressHub>("/progress");
    endpoints.MapControllers();
});
app.UseDirectoryBrowser(new DirectoryBrowserOptions()
{
    FileProvider = new PhysicalFileProvider(
            @"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\RastrFiles"),
    RequestPath = new PathString("/files")
});

app.Run();

public partial class Program { } // Для интеграционных тестов
