using DBRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// Add services to the container.

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connection));
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors(builder => builder.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
app.MapGet("/a", (RepositoryContext db) => db.Calculations.ToList());


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
