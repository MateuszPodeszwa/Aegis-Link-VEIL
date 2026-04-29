using AegisLink.Server.Services;

namespace AegisLink.Tests.Server;

public class AegisIdServiceTests
{
    [Fact]
    public void CreateId_UsesExpectedAlphabet()
    {
        var publicKeyBytes = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();

        var id = AegisIdService.CreateId(publicKeyBytes);

        Assert.Equal("VYQWYLXV", id);
    }

    [Fact]
    public void Verify_ReturnsTrueForMatchingId()
    {
        var publicKeyBytes = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        var publicKeyB64 = Convert.ToBase64String(publicKeyBytes);
        var id = AegisIdService.CreateId(publicKeyBytes);

        var result = AegisIdService.Verify(id, publicKeyB64);

        Assert.True(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForInvalidBase64()
    {
        var result = AegisIdService.Verify("ABCDEFGH", "not-base-64");

        Assert.False(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForMismatchedId()
    {
        var publicKeyBytes = Enumerable.Range(1, 32).Select(i => (byte)i).ToArray();
        var publicKeyB64 = Convert.ToBase64String(publicKeyBytes);

        var result = AegisIdService.Verify("ABCDEFGH", publicKeyB64);

        Assert.False(result);
    }
}
