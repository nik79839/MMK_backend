using Application.Interfaces;
using Infrastructure;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Server;
using Server.Hub;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CalculationResultContext>(options => options.UseNpgsql(connection), ServiceLifetime.Transient);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, @"C:\Users\otrok\source\repos\������_��_�������������\Server\bin\Debug\net6.0\Domain.xml");
    c.IncludeXmlComments(@"C:\Users\otrok\source\repos\������_��_�������������\Server\bin\Debug\net6.0\Server.xml");
});
builder.Services.AddSignalR();
builder.Services.AddScoped<ICalculationResultRepository, CalculationResultRepository>();
var assembly = Assembly.GetAssembly(typeof(MappingProfile));
builder.Services.AddAutoMapper(assembly);
builder.Services.AddScoped<ICalculationService, CalculationService>();
builder.Services.AddScoped<IRastrSchemeInfoService, RastrSchemeInfoService>();

var app = builder.Build();
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
            @"C:\Users\otrok\source\repos\������_��_�������������\Server\RastrFiles"),
    RequestPath = new PathString("/files")
});

app.Run();
