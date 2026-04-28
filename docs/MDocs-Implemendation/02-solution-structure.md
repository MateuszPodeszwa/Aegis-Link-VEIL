# Solution Layout and Runtime Flow

High‑level map of the solution plus the main runtime path for a secure chat.

## Solution structure

| Project | Type | Role |
| --- | --- | --- |
| `Aegis-Link-VEIL/` | Blazor WebAssembly PWA | Client UI, crypto, session handling |
| `AegisLink.Server/` | ASP.NET Core 10 | REST API, SignalR hub, identity storage |
| `AegisLink.Shared/` | .NET class library | Shared DTOs |

Key folders and files:

- `Aegis-Link-VEIL/Pages/` — UI pages and flows.
- `Aegis-Link-VEIL/Services/` — browser storage, SignalR, crypto interop.
- `Aegis-Link-VEIL/Models/` — client data models.
- `Aegis-Link-VEIL/wwwroot/` — static assets and crypto JS.
- `AegisLink.Server/Controllers/` — REST endpoints.
- `AegisLink.Server/Hubs/` — SignalR relay hub.
- `AegisLink.Server/Data/` — EF Core context and entities.

## Runtime flow (short version)

1. **Identity creation** — browser generates keypair and derives Aegis ID from the public key hash.
2. **Identity registration** — client posts Aegis ID + public key to `/identity/register`.
3. **Partner lookup** — partner Aegis ID resolves to a public key via `/identity/lookup/{id}`.
4. **Room ID** — both sides compute a deterministic room ID from both Aegis IDs.
5. **Shared key** — ECDH creates a shared secret from own secret key + partner public key.
6. **SignalR join** — client joins the room on `/chatHub`.
7. **Message send** — plaintext → `secretbox` → base64 payload, then relayed.
8. **Message receive** — payload decrypted client‑side and stored in session storage.

## Dev ports (default)

- Client: `https://localhost:7035`
- Server: `https://localhost:7069`

## See also

- [Client app core](03-client-core.md)
- [Server API and hub](08-server-api.md)
- [Back to index](README.md)
