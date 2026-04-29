using AegisLink.Server.Data;

namespace AegisLink.Tests.Server;

public class UserKeyTests
{
    [Fact]
    public void Defaults_AreEmptyStrings()
    {
        var userKey = new UserKey();

        Assert.Equal(string.Empty, userKey.AegisId);
        Assert.Equal(string.Empty, userKey.PublicKey);
    }
}
