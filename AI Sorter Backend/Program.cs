using AI_Sorter_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static AI_Sorter_Backend.Models.DbContex;
using AI_Sorter_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ��������� CORS ��� localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
       builder =>
        {
            builder.WithOrigins("http://ai-sortme.local:80","http://ai-sortme.local")
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

// Jwt keys
builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer("Bearer", options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
		};
	});

builder.Services.AddAuthorization();

builder.Services.AddScoped<SystemInfoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();