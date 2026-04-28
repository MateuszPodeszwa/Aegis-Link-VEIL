using Microsoft.JSInterop;

/// <summary>
/// Handles chat cryptography calls by forwarding them to the JavaScript crypto layer.
/// </summary>
/// <remarks>
/// This service keeps C# pages/components simple and puts all crypto implementation details in one place.
/// </remarks>
public class ChatCryptoService
{
    // JS runtime bridge used to call functions in crypto-interop.js.
    private readonly IJSRuntime _js;

    /// <summary>
    /// Creates a new crypto service instance.
    /// </summary>
    /// <param name="js">JavaScript runtime used for browser interop calls.</param>
    public ChatCryptoService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Encrypts a JSON message using a shared key.
    /// </summary>
    /// <param name="json">Plain JSON text to encrypt.</param>
    /// <param name="key">Shared encryption key.</param>
    /// <returns>The encrypted payload string.</returns>
    public async Task<string> Encrypt(string json, string key)
    {
        // Encrypt plain JSON before sending it over SignalR.
        return await _js.InvokeAsync<string>(
            "aegisCrypto.boxEncrypt",
            json,
            key);
    }

    /// <summary>
    /// Decrypts an encrypted payload back into JSON text.
    /// </summary>
    /// <param name="payload">Encrypted payload text from the chat hub.</param>
    /// <param name="key">Shared decryption key.</param>
    /// <returns>Decrypted JSON text.</returns>
    public async Task<string> Decrypt(string payload, string key)
    {
        // Decrypt incoming payload back into JSON text.
        return await _js.InvokeAsync<string>(
            "aegisCrypto.boxDecrypt",
            payload,
            key);
    }

    /// <summary>
    /// Computes a shared key from a private secret and the other user's public key.
    /// </summary>
    /// <param name="secret">Our local secret/private key.</param>
    /// <param name="publicKey">Partner public key.</param>
    /// <returns>Derived shared key used for message encryption/decryption.</returns>
    public async Task<string> ComputeSharedKey(string secret, string publicKey)
    {
        // Derive one shared key from our secret key and partner public key.
        return await _js.InvokeAsync<string>(
            "aegisCrypto.computeSharedKey",
            secret,
            publicKey);
    }
}
