using AegisLink.Server.Data;
using AegisLink.Server.Hubs;
using AegisLink.Server.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
var hasAllowedOrigins = allowedOrigins.Length > 0;

if (!hasAllowedOrigins && builder.Environment.IsDevelopment())
{
    throw new InvalidOperationException(
        "No CORS origins configured. Add at least one entry under 'AllowedOrigins' in appsettings.Development.json.");
}

if (hasAllowedOrigins)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("_myAllowSpecificOrigins", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSignalR();
builder.Services.AddSingleton<HubSessionTracker>();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        [ "application/octet-stream" ]);
});

var sqliteConnectionString =
    builder.Configuration.GetConnectionString("AegisLink")
    ?? "Data Source=aegislink.db";

builder.Services.AddDbContext<AegisLinkDbContext>(options => options.UseSqlite(sqliteConnectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AegisLinkDbContext>();
    db.Database.EnsureCreated();
}

// --- Middleware ---
app.UseResponseCompression();
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseRouting();
if (hasAllowedOrigins)
{
    app.UseCors("_myAllowSpecificOrigins");
}
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<SecureMessagingHub>("/chatHub");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
