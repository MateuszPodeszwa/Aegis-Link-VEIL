using AegisLink.Server.Controllers;
using AegisLink.Server.Data;
using AegisLink.Server.Services;
using AegisLink.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AegisLink.Tests.Server;

public class IdentityControllerTests
{
    [Fact]
    public async Task Register_AddsNewEntry()
    {
        await using var context = CreateContext();
        var controller = new IdentityController(context);
        var publicKeyBytes = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        var publicKey = Convert.ToBase64String(publicKeyBytes);
        var aegisId = AegisIdService.CreateId(publicKeyBytes);

        var result = await controller.Register(new UserKeyReg(aegisId, publicKey));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(aegisId, GetAnonymousValue(ok.Value, "aegisId"));
        Assert.Equal(1, await context.UserKeys.CountAsync());
    }

    [Fact]
    public async Task Register_ReturnsOkForExistingMatchingKey()
    {
        await using var context = CreateContext();
        var publicKeyBytes = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        var publicKey = Convert.ToBase64String(publicKeyBytes);
        var aegisId = AegisIdService.CreateId(publicKeyBytes);
        context.UserKeys.Add(new UserKey { AegisId = aegisId, PublicKey = publicKey });
        await context.SaveChangesAsync();
        var controller = new IdentityController(context);

        var result = await controller.Register(new UserKeyReg(aegisId, publicKey));

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsConflictForDifferentKey()
    {
        await using var context = CreateContext();
        var publicKeyBytes = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        var publicKey = Convert.ToBase64String(publicKeyBytes);
        var aegisId = AegisIdService.CreateId(publicKeyBytes);
        context.UserKeys.Add(new UserKey { AegisId = aegisId, PublicKey = "DIFFERENT" });
        await context.SaveChangesAsync();
        var controller = new IdentityController(context);

        var result = await controller.Register(new UserKeyReg(aegisId, publicKey));

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsBadRequestForInvalidId()
    {
        await using var context = CreateContext();
        var controller = new IdentityController(context);

        var result = await controller.Register(new UserKeyReg("INVALID", "AQID"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Deregister_ReturnsNotImplemented()
    {
        await using var context = CreateContext();
        var controller = new IdentityController(context);

        var result = await controller.Deregister(new UserKeyReg("INVALID", "AQID"));

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(501, objectResult.StatusCode);
    }

    [Fact]
    public async Task Lookup_ReturnsBadRequestForInvalidId()
    {
        await using var context = CreateContext();
        var controller = new IdentityController(context);

        var result = await controller.Lookup("short");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Lookup_ReturnsNotFoundForMissingEntry()
    {
        await using var context = CreateContext();
        var controller = new IdentityController(context);

        var result = await controller.Lookup("ABCDEFGH");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Lookup_ReturnsResultForEntry()
    {
        await using var context = CreateContext();
        context.UserKeys.Add(new UserKey { AegisId = "ABCDEFGH", PublicKey = "AQID" });
        await context.SaveChangesAsync();
        var controller = new IdentityController(context);

        var result = await controller.Lookup("ABCDEFGH");

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<UserKeyGetResult>(ok.Value);
        Assert.Equal("ABCDEFGH", payload.AegisId);
        Assert.Equal("AQID", payload.PublicKey);
    }

    private static AegisLinkDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AegisLinkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AegisLinkDbContext(options);
    }

    private static string? GetAnonymousValue(object? value, string name)
    {
        if (value is null) return null;
        var property = value.GetType().GetProperty(name);
        return property?.GetValue(value) as string;
    }
}
