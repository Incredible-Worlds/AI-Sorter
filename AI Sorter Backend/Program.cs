using AI_Sorter_Backend.Models;
using Microsoft.EntityFrameworkCore;
using static AI_Sorter_Backend.Models.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Разрешаем CORS для localhost
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowLocalhost",
//        builder =>
//        {
//            builder.WithOrigins("https://localhost:7183", "http://localhost:5274")
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//        });
//});

// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var builder2 = WebApplication.CreateBuilder(args);

// Adding a database connection
builder2.Services.AddDbContext<YourContext>(options => options.UseNpgsql("Host=localhost;Database=YourDatabase;Username=yourusername;Password=yourpassword"));

// Adding a connection to the database Adding an entity model
builder2.Services.AddScoped<YourContext>();

var app1 = builder2.Build();

app1.Run();