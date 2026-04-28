# Client Services

Service classes used by the client UI. Each service wraps browser APIs or SignalR in a simple C# surface.

## ChatCryptoService

Location: `Aegis-Link-VEIL/Services/ChatCryptoService.cs`

Role: JS interop wrapper for encryption helpers.

Methods:

- `Encrypt(string json, string key)` — calls `aegisCrypto.boxEncrypt` and returns base64 ciphertext.
- `Decrypt(string payload, string key)` — calls `aegisCrypto.boxDecrypt` and returns plaintext JSON.
- `ComputeSharedKey(string secret, string publicKey)` — calls `aegisCrypto.computeSharedKey` (ECDH).

## ChatHubService

Location: `Aegis-Link-VEIL/Services/ChatHubService.cs`

Role: SignalR client wrapper with event hooks.

State:

- `_connection` — SignalR connection instance.
- `_sessionId` — room ID used for join and send.

Events:

- `OnReceiveMessage` — fired on decrypted inbound message.
- `OnMessageDeleted` — fired on delete event.
- `OnUserJoined` — fired when another connection joins the room.
- `OnStatusChanged` — fired on connection state changes.

Methods:

- `InitializeAsync(hubUrl, decryptFn, encryptFn, sharedKey, sessionId)` — builds connection, wires handlers, joins session.
- `SendMessage(sessionId, cipher)` — forwards ciphertext to hub.
- `DeleteMessage(sessionId, messageId)` — forwards delete request to hub.
- `DisposeAsync()` — disposes the SignalR connection.

## ChatStore

Location: `Aegis-Link-VEIL/Services/ChatStore.cs`

Role: session storage wrapper for chat sessions and message history.

Key elements:

- `const string StorageKey = "aegis_chats"` — session storage key.
- `List<ChatSession> Chats` — in‑memory chat list.

Methods:

- `InitializeAsync()` — lazy initialisation wrapper.
- `SaveAsync()` — serialises and saves chats to session storage.
- `GetChat(string sessionId)` — resolves a chat by session key.
- `CreateChat(sessionId, sessionName, partnerAegisId, partnerPublicKey)` — creates or returns existing chat.

## ContactService

Location: `Aegis-Link-VEIL/Services/ContactService.cs`

Role: local contact storage helpers via JS interop.

Methods:

- `GetAllAsync()` — loads contacts and sorts by nickname.
- `GetByIdAsync(string aegisId)` — resolves a contact by ID.
- `SaveAsync(Contact contact)` — adds or updates a contact.
- `DeleteAsync(string aegisId)` — removes a contact.
- `GetColourAsync(string aegisId)` — deterministic colour for avatar.
- `ExistsAsync(string aegisId)` — checks if a contact is already stored.

## SettingsService

Location: `Aegis-Link-VEIL/Services/SettingService.cs`

Role: local settings persistence via JS interop.

Methods:

- `LoadAsync()` — loads settings or returns defaults.
- `SaveAsync(AppSettings settings)` — saves settings to local storage.

## See also

- [Crypto interop](07-crypto-interop.md)
- [Client models](06-client-models.md)
- [Back to index](README.md)
