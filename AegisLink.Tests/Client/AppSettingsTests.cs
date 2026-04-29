namespace AegisLink.Tests.Client;

public class AppSettingsTests
{
    [Fact]
    public void Defaults_AreConfigured()
    {
        var settings = new global::AppSettings();

        Assert.False(settings.AutoDeleteEnabled);
        Assert.Equal("Never", settings.AutoDeleteDuration);
    }
}
