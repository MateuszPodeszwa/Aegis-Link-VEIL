using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Aegis_Link_VEIL;
using Aegis_Link_VEIL.Services;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped<SettingsService>();
        builder.Services.AddScoped<ChatStore>();
        builder.Services.AddScoped<ContactService>();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        var webApiBaseAddress = builder.Configuration["WebApi:BaseAddress"]
            ?? throw new InvalidOperationException("WebApi:BaseAddress is not configured.");
        builder.Services.AddHttpClient("WebAPI", client =>
        {
            client.BaseAddress = new Uri(webApiBaseAddress);
        });

        await builder.Build().RunAsync();
    }
}