using AegisLink.Shared;
using Microsoft.JSInterop;

namespace Aegis_Link_VEIL.Services;

public class SettingsService
{
    private readonly IJSRuntime _js;

    public SettingsService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<AppSettings> LoadAsync()
    {
        var settings = await _js.InvokeAsync<AppSettings?>(
            "aegisCrypto.loadSettings"
        );

        return settings ?? new AppSettings();
    }

    public async Task SaveAsync(AppSettings settings)
    {
        await _js.InvokeVoidAsync(
            "aegisCrypto.saveSettings",
            settings ?? new AppSettings()
        );
    }
}
