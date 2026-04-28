using AegisLink.Shared;
using Microsoft.JSInterop;

namespace Aegis_Link_VEIL.Services;

/// <summary>
/// Loads and saves user app settings through browser storage.
/// </summary>
/// <remarks>
/// This keeps settings persistence in one place and gives pages a small API to call.
/// </remarks>
public class SettingsService
{
    // JS runtime bridge for settings storage calls.
    private readonly IJSRuntime _js;

    /// <summary>
    /// Creates a new settings service instance.
    /// </summary>
    /// <param name="js">JavaScript runtime used for browser interop calls.</param>
    public SettingsService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Loads app settings from storage.
    /// </summary>
    /// <returns>Saved settings, or defaults when nothing is stored yet.</returns>
    public async Task<AppSettings> LoadAsync()
    {
        // Load saved settings from browser storage.
        var settings = await _js.InvokeAsync<AppSettings?>(
            "aegisCrypto.loadSettings"
        );

        // If nothing has been saved yet, return default settings.
        return settings ?? new AppSettings();
    }

    /// <summary>
    /// Saves app settings to storage.
    /// </summary>
    /// <param name="settings">Settings model to persist.</param>
    public async Task SaveAsync(AppSettings settings)
    {
        // Guard against null so storage always receives a valid object.
        await _js.InvokeVoidAsync(
            "aegisCrypto.saveSettings",
            settings ?? new AppSettings()
        );
    }
}
