using LabManagement.App.Common;
using LabManagement.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using LabManagement.Api.Extensions;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection"); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServices();

builder.Services.AddDbContext<LabDbContext>(options => 
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ILabDbContext>(provider => 
    provider.GetRequiredService<LabDbContext>());

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(ILabDbContext).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(ILabDbContext).Assembly);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret не найден!");

// Настройка аутентификации и авторизации 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"], // Кто выпустил токен
        ValidAudience = jwtSettings["Audience"],     // Для кого выпущен токен
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)) 
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.AddEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 

app.Run();
