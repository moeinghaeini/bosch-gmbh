using IndustrialAutomation.Infrastructure;
using IndustrialAutomation.Core;
using IndustrialAutomation.Core.Interfaces;
using IndustrialAutomation.Infrastructure.Services;
using IndustrialAutomation.API.Middleware;
using Serilog;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with enhanced logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "IndustrialAutomation")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File("logs/industrial-automation-.txt", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("X-Version"),
        new QueryStringApiVersionReader("version"),
        new UrlSegmentApiVersionReader()
    );
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Industrial Automation API", Version = "v1.0" });
    c.SwaggerDoc("v2", new() { Title = "Industrial Automation API", Version = "v2.0" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Response Compression for better performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// Add Memory Cache for performance
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000; // Maximum number of cache entries
    options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
});

// Add CORS with enhanced security
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Add Authentication services
builder.Services.AddScoped<IAuthService, AuthService>();

// Add JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "IndustrialAutomation";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "IndustrialAutomation";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add Rate Limiting
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.AddSingleton<RateLimitOptions>();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("memory", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Memory is OK"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Industrial Automation API v1.0");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Industrial Automation API v2.0");
        c.RoutePrefix = "api-docs";
        c.DocumentTitle = "Industrial Automation API Documentation";
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
    });
}

// Add Response Compression Middleware
app.UseResponseCompression();

// Add Custom Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

// Add Request Logging
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// Add Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

// Add Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

try
{
    Log.Information("Starting Industrial Automation API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
