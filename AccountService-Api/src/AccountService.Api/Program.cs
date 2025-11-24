using AccountService.Core.DbModels;
using AccountService.Core.Dtos;
using AccountService.Core.Interfaces;
using AccountService.Core.Services;
using AccountService.Core.Validation;
using AccountService.Data;
using AccountService.Infrastructure.Repository;
using Asp.Versioning;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IValidator<RegisterRequestDto>, RegisterDtoValidator>();
builder.Services.AddAutoMapper(typeof(RegisterProfile));
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


//Read Serilog configuration from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // If no version specified, use v1
    options.AssumeDefaultVersionWhenUnspecified = true; // Apply default version automatically
    options.ReportApiVersions = true; // Shows available versions in Response Headers
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formats group name: v1, v2, v3
    options.SubstituteApiVersionInUrl = true; // Replace version placeholder in routes
});

// In-memory EF Core
builder.Services.AddDbContext<AccountDbContext>(opt => opt.UseInMemoryDatabase("AccountsDb"));

// JWT settings
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super-secret-key-for-event-scheduling-platform-enhanced";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AccountService";
builder.Services.AddSingleton<IJwtTokenService>(new JwtTokenService(jwtKey, jwtIssuer));

//JWTToken Authorisation
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});
});


var app = builder.Build();

// Seed a default admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    if (!db.Users.Any())
    {
        db.Users.Add(new User { UserId = Guid.NewGuid(), UserName = "admin", PasswordHash = "UtRa5nECyqzKQt94ssKiAg==.GiUL5v6TjEcWXvFDRla2LaPLdgjnskef6jW4RpMikW8=.100000", Role = "Admin",Email="admin@admin.com" });
        db.Users.Add(new User { UserId = Guid.NewGuid(), UserName = "suheeluser", PasswordHash = "4vFRQeANvMO1/w3kIJDhLA==.V3g14J5FJfqXnNFLmlGe/0/ZuPlHl89qkgzDIZ2Kops=.100000", Role = "User",Email="suheeluser@suheeluser.com" });
        db.SaveChanges();
    }
}

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
