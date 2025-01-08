using Microsoft.OpenApi.Models;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "User Management API", 
        Version = "v1" 
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
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
        }
    });
});

var app = builder.Build();

// Middleware to catch unhandled exceptions and return consistent error responses
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An unhandled exception occurred.");

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var errorResponse = new { error = "Internal server error." };
        var errorJson = JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(errorJson);
    }
});

// Middleware to validate tokens and return 401 Unauthorized for invalid tokens
app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/swagger-ui"))
    {
        await next.Invoke();
        return;
    }

    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
    var configToken = app.Configuration["ApiToken"];

    if (string.IsNullOrEmpty(token) || token != configToken)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/json";

        var errorResponse = new { error = "Unauthorized" };
        var errorJson = JsonSerializer.Serialize(errorResponse);

        await context.Response.WriteAsync(errorJson);
        return;
    }

    await next.Invoke();
});

// Middleware to log HTTP method, request path, and response status code
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    var method = context.Request.Method;
    var path = context.Request.Path;
    
    await next.Invoke();

    var statusCode = context.Response.StatusCode;
    logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}", method, path, statusCode);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management API v1"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
