using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using TodoListManager.API.Middleware;
using TodoListManager.Infrastructure;
using TodoListManager.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controllers with JSON String Enum Conversion
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// 2. Configure Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TodoList Manager API",
        Version = "v1",
        Description = "Clean Architecture RESTful Web API for managing tasks, priorities, categories, and deadlines."
    });
});

// 3. Configure CORS for Frontend Integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 4. Register Infrastructure & Application Services (DbContext, Repositories, Services)
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// 5. Automatic Database Initialization & Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.InitializeAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing and seeding the database.");
    }
}

// 6. Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// 7. Enable Swagger & Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoList Manager API v1");
    c.RoutePrefix = "swagger";
});

// Redirect root to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// 8. CORS & Routing Middleware Pipeline
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
