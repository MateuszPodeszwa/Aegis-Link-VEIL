using Microsoft.JSInterop;

public class ChatCryptoService
{
    private readonly IJSRuntime _js;

    public ChatCryptoService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string> Encrypt(string json, string key)
    {
        return await _js.InvokeAsync<string>(
            "aegisCrypto.boxEncrypt",
            json,
            key);
    }

    public async Task<string> Decrypt(string payload, string key)
    {
        return await _js.InvokeAsync<string>(
            "aegisCrypto.boxDecrypt",
            payload,
            key);
    }

    public async Task<string> ComputeSharedKey(string secret, string publicKey)
    {
        return await _js.InvokeAsync<string>(
            "aegisCrypto.computeSharedKey",
            secret,
            publicKey);
    }
}