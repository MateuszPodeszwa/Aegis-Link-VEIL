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
        builder.Services.AddScoped<ChatHubService>();
        builder.Services.AddScoped<ChatCryptoService>();
        builder.Services.AddScoped<ChatStore>();
        builder.Services.AddScoped<ContactService>();
        builder.Services.AddScoped<SettingsService>();
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        var configuredBaseAddress = builder.Configuration["WebApi:BaseAddress"];
        var webApiBaseAddress = ResolveWebApiBaseAddress(configuredBaseAddress, builder.HostEnvironment.BaseAddress);
        builder.Services.AddHttpClient("WebAPI", client =>
        {
            client.BaseAddress = webApiBaseAddress;
        });

        await builder.Build().RunAsync();
    }

    private static Uri ResolveWebApiBaseAddress(string? configuredBaseAddress, string hostBaseAddress)
    {
        var hostBaseUri = new Uri(hostBaseAddress);

        if (string.IsNullOrWhiteSpace(configuredBaseAddress))
            return hostBaseUri;

        if (Uri.TryCreate(configuredBaseAddress, UriKind.Absolute, out var absolute))
        {
            // Guard against stale cached development config in deployed environments.
            if (absolute.IsLoopback && !hostBaseUri.IsLoopback)
                return hostBaseUri;

            return absolute;
        }

        return new Uri(hostBaseUri, configuredBaseAddress);
    }
}
