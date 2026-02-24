var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5150") 
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