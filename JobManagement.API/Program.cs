using JobManagement.Application.Interfaces;
using JobManagement.Application.Services;
using JobManagement.Infrastructure.Configuration;
using JobManagement.Persistence.Data;
using JobManagement.Persistence.Repositories;
using JobManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog();
builder.Services.AddControllers();

// Add services to the container
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddValidationConfiguration();

// Bind JwtSettings from appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidAudience = jwtSettings?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "")),
            ClockSkew = TimeSpan.Zero
        };
    });
        builder.Services.AddAuthorization();

// Database
        builder.Services.AddDbContext<JobManagementDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("JobManagement.Persistence")
            ));

// CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<JobApplicationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPhoneValidationService, PhoneValidationService>();
builder.Services.AddHttpClient<PhoneValidationService>();

var app = builder.Build();

// Middleware pipeline
app.UseSwaggerConfiguration();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();