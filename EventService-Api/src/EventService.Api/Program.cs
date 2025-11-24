using Asp.Versioning;
using EventService.Core.Dtos;
using EventService.Core.Interfaces;
using EventService.Core.Mappings;
using EventService.Core.Service;
using EventService.Core.Validation;
using EventService.Infrastructure.Database;
using EventService.Infrastructure.Repository;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IEventManagementService, EventManagementService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IValidator<EventCreateDto>, EventCreateDtoValidator>();
builder.Services.AddAutoMapper(typeof(EventProfile));

//RefitClient
builder.Services.AddRefitClient<IAccountApi>()
    .ConfigureHttpClient((sp, c) =>
    {
        var accessor = sp.GetRequiredService<IHttpContextAccessor>();
        var token = accessor.HttpContext?.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(token))
        {
            c.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Split(" ")[1]);
        }

        c.BaseAddress = new Uri("https://localhost:7174");// URL of Account API
    });

builder.Services.AddHttpContextAccessor();
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
builder.Services.AddDbContext<EventDbContext>(opt => opt.UseInMemoryDatabase("EventsDb"));


// JWT settings
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super-secret-key-for-event-scheduling-platform-enhanced";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AccountService";
//builder.Services.AddSingleton<IJwtTokenService>(new JwtTokenService(jwtKey, jwtIssuer));

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventDbContext>();
    db.Database.EnsureCreated(); // Forces seed data to be added
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
