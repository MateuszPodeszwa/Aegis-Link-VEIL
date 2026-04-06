using AegisLink.Server.Data;
using AegisLink.Server.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.EnableForHttps = true;
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        [ "application/octet-stream" ]);
});

builder.Services.AddDbContext<AegisLinkDbContext>(options =>
    options.UseSqlite("Data Source=aegislink.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AegisLinkDbContext>();
    db.Database.EnsureCreated();
}

    // --- Middleware ---
    app.UseResponseCompression();
app.MapHub<SecureMessagingHub>("/chatHub"); 
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