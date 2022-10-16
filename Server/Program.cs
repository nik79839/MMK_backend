using Application.Interfaces;
using Infrastructure.Persistance;
using Infrastructure.Persistance.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Server;
using Server.Hub;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Add services to the container.



string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CalculationResultContext>(options => { options.UseNpgsql(connection);}, ServiceLifetime.Singleton);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProgressHub>("/progress");
    endpoints.MapDefaultControllerRoute();
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions()
{
    FileProvider = new PhysicalFileProvider(
            @"C:\Users\otrok\source\repos\Диплом_УР_Автоматизация\Server\RastrFiles"),
    RequestPath = new PathString("/files")
});

app.Run();
