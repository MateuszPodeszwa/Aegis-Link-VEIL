# Copilot Instructions for Aegis-Link-VEIL

## Build, test, and run commands

- `dotnet restore Aegis-Link-VEIL.sln`
- `dotnet build Aegis-Link-VEIL.sln`
- `dotnet test Aegis-Link-VEIL.sln` (currently no test projects/targets are defined)
- `dotnet run --project AegisLink.Server`
- `dotnet run --project Aegis-Link-VEIL`

## Linting

No dedicated lint command is currently configured in this repository.

## High-level architecture

The solution is split into three projects:

- `Aegis-Link-VEIL` (Blazor WebAssembly client): UI, session/chat state, and crypto interop calls.
- `AegisLink.Server` (ASP.NET Core): identity API, SignalR hub, and SQLite-backed public-key mapping.
- `AegisLink.Shared` (class library): shared DTOs used by client and server.

End-to-end flow:

1. Client generates keys in browser JS (`wwwroot/crypto-interop.js`) and derives an 8-char Aegis ID from the public key.
2. Client registers `(AegisId, PublicKey)` with `POST /identity/register`.
3. For a chat, client resolves partner key via `GET /identity/lookup/{id}`.
4. Client computes ECDH shared key (`nacl.box.before`) and encrypts messages client-side (`nacl.secretbox`).
5. Ciphertext is sent through `/chatHub`; the server relays payloads only.

Server behavior is intentionally blind-relay: hub methods validate room/session membership and format but never decrypt payloads.

## Key codebase conventions

- Keep cryptographic implementation in `wwwroot/crypto-interop.js`; C# crypto services should only call JS interop wrappers.
- Session/room IDs are deterministic and must be derived via `aegisCrypto.computeRoomId(myAegisId, partnerAegisId)` (normalized + sorted IDs, SHA-256 prefix hex).
- Storage keys are part of the app contract:
  - `aegis_identity` in `localStorage`
  - `aegis_chats` in `sessionStorage`
  - `aegis_contacts` and `aegis_settings` in `localStorage`
- Client services follow the `Aegis_Link_VEIL.Services` namespace and are registered in `Aegis-Link-VEIL/Program.cs`.
- Use the named HTTP client `"WebAPI"` for server API calls; its base URL must match client `wwwroot/appsettings.json` (`WebApi:BaseAddress`) and server CORS `AllowedOrigins`.
- Server startup requires configured `AllowedOrigins` in development; missing values throw at startup.

## Security-critical expectations

- Never transmit or persist private keys on the server.
- Never add server-side plaintext message logging or decryption paths.
- Preserve API boundary checks (identity verification and session validation) when modifying hub/controller logic.
