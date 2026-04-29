using AegisLink.Shared;

namespace AegisLink.Tests.Shared;

public class SharedModelTests
{
    [Fact]
    public void Contact_Defaults_AreEmpty()
    {
        var contact = new Contact();

        Assert.Equal(string.Empty, contact.AegisId);
        Assert.Equal(string.Empty, contact.PublicKey);
        Assert.Equal(string.Empty, contact.Nickname);
        Assert.Equal(string.Empty, contact.Note);
        Assert.Equal(string.Empty, contact.Colour);
    }

    [Fact]
    public void UserKeyReg_PreservesValues()
    {
        var reg = new UserKeyReg("ABCDEFGH", "AQID");

        Assert.Equal("ABCDEFGH", reg.AegisId);
        Assert.Equal("AQID", reg.PublicKey);
    }

    [Fact]
    public void UserKeyGetResult_PreservesValues()
    {
        var result = new UserKeyGetResult("ABCDEFGH", "AQID");

        Assert.Equal("ABCDEFGH", result.AegisId);
        Assert.Equal("AQID", result.PublicKey);
    }
}
