using AI_Sorter_Backend.Models;
using Microsoft.EntityFrameworkCore;
using static AI_Sorter_Backend.Models.DbContex;

var builder = WebApplication.CreateBuilder(args);

// Разрешаем CORS для localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
       builder =>
        {
            builder.WithOrigins("http://localhost:80","http://localhost")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Any, 5001);
});

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adding a database connection
builder.Services.AddDbContext<ApplicationDbContext>(options => 
        options.UseNpgsql("Host=db; Database=postgres; Username=postgres; Password=BlazorApp"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

app.Run();