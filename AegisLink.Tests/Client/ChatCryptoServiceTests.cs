using AegisLink.Tests.TestUtilities;

namespace AegisLink.Tests.Client;

public class ChatCryptoServiceTests
{
    [Fact]
    public async Task Encrypt_InvokesJsInterop()
    {
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.boxEncrypt" ? "encrypted" : null);
        var service = new ChatCryptoService(js);

        var result = await service.Encrypt("{}", "key");

        Assert.Equal("encrypted", result);
        Assert.Equal("aegisCrypto.boxEncrypt", js.Invocations[0].Identifier);
    }

    [Fact]
    public async Task Decrypt_InvokesJsInterop()
    {
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.boxDecrypt" ? "decrypted" : null);
        var service = new ChatCryptoService(js);

        var result = await service.Decrypt("payload", "key");

        Assert.Equal("decrypted", result);
        Assert.Equal("aegisCrypto.boxDecrypt", js.Invocations[0].Identifier);
    }

    [Fact]
    public async Task ComputeSharedKey_InvokesJsInterop()
    {
        var js = new TestJsRuntime((identifier, _) =>
            identifier == "aegisCrypto.computeSharedKey" ? "shared" : null);
        var service = new ChatCryptoService(js);

        var result = await service.ComputeSharedKey("secret", "public");

        Assert.Equal("shared", result);
        Assert.Equal("aegisCrypto.computeSharedKey", js.Invocations[0].Identifier);
    }
}
