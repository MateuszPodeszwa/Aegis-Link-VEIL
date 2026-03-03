var builder = WebApplication.CreateBuilder(args);

// --- Services ---
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException(
        "No CORS origins configured. Add at least one entry under 'AllowedOrigins' in appsettings.Development.json.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers(); // This finds your new Controller
builder.Services.AddOpenApi();

var app = builder.Build();

// --- Middleware ---
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();
app.UseCors("_myAllowSpecificOrigins");
app.UseAuthorization();

app.MapControllers(); // This maps the routes defined in your Controller attributes

app.Run();