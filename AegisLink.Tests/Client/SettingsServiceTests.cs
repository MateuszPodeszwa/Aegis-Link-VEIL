using AegisLink.Tests.TestUtilities;
using Aegis_Link_VEIL.Services;

namespace AegisLink.Tests.Client;

public class SettingsServiceTests
{
    [Fact]
    public async Task LoadAsync_ReturnsDefaultsWhenMissing()
    {
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.loadSettings" ? null : null);
        var service = new SettingsService(js);

        var settings = await service.LoadAsync();

        Assert.False(settings.AutoDeleteEnabled);
        Assert.Equal("Never", settings.AutoDeleteDuration);
    }

    [Fact]
    public async Task SaveAsync_UsesDefaultWhenNull()
    {
        object?[]? args = null;
        var js = new TestJsRuntime((identifier, callArgs) =>
        {
            if (identifier == "aegisCrypto.saveSettings")
            {
                args = callArgs;
            }

            return null;
        });
        var service = new SettingsService(js);

        await service.SaveAsync(null!);

        Assert.NotNull(args);
        Assert.NotNull(args![0]);
        var saved = Assert.IsType<AppSettings>(args![0]);
        Assert.Equal("Never", saved.AutoDeleteDuration);
    }
}
