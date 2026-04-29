using AegisLink.Shared;
using AegisLink.Tests.TestUtilities;
using Aegis_Link_VEIL.Services;

namespace AegisLink.Tests.Client;

public class ContactServiceTests
{
    [Fact]
    public async Task GetAllAsync_SortsByNickname()
    {
        var contacts = new List<Contact>
        {
            new() { Nickname = "Zed", AegisId = "ZED" },
            new() { Nickname = "Amy", AegisId = "AMY" }
        };
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.loadContacts" ? contacts : null);
        var service = new ContactService(js);

        var result = await service.GetAllAsync();

        Assert.Equal(new[] { "Amy", "Zed" }, result.Select(c => c.Nickname));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsContact()
    {
        var contacts = new List<Contact>
        {
            new() { Nickname = "Zed", AegisId = "ZED" }
        };
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.loadContacts" ? contacts : null);
        var service = new ContactService(js);

        var result = await service.GetByIdAsync("ZED");

        Assert.NotNull(result);
        Assert.Equal("Zed", result!.Nickname);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueWhenFound()
    {
        var contacts = new List<Contact>
        {
            new() { Nickname = "Zed", AegisId = "ZED" }
        };
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.loadContacts" ? contacts : null);
        var service = new ContactService(js);

        var exists = await service.ExistsAsync("ZED");

        Assert.True(exists);
    }

    [Fact]
    public async Task SaveAsync_CallsInterop()
    {
        string? called = null;
        var js = new TestJsRuntime((identifier, _) =>
        {
            called = identifier;
            return null;
        });
        var service = new ContactService(js);

        await service.SaveAsync(new Contact());

        Assert.Equal("aegisCrypto.saveContact", called);
    }

    [Fact]
    public async Task DeleteAsync_CallsInterop()
    {
        string? called = null;
        var js = new TestJsRuntime((identifier, _) =>
        {
            called = identifier;
            return null;
        });
        var service = new ContactService(js);

        await service.DeleteAsync("ZED");

        Assert.Equal("aegisCrypto.deleteContact", called);
    }

    [Fact]
    public async Task GetColourAsync_ReturnsColour()
    {
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.colourForId" ? "#abc" : null);
        var service = new ContactService(js);

        var colour = await service.GetColourAsync("ZED");

        Assert.Equal("#abc", colour);
    }
}
