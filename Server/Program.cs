using DBRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Server.Hub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Add services to the container.

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connection));
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

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
