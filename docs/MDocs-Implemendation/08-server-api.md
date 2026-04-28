# Server API, SignalR Hub, and Data Layer

Server‑side components live in `AegisLink.Server/` and provide identity storage plus a blind relay hub.

## Program.cs

Location: `AegisLink.Server/Program.cs`

Key behaviour:

- Loads CORS origins from configuration and fails fast in development if none are set.
- Registers controllers and OpenAPI (dev only).
- Registers SignalR and `HubSessionTracker`.
- Enables response compression for binary payloads.
- Configures EF Core with SQLite (`Data Source=aegislink.db`).
- Ensures the database is created on start.
- Maps `/chatHub` and controller routes.

## IdentityController

Location: `AegisLink.Server/Controllers/IdentityController.cs`

Endpoints:

- `POST /identity/register` — validates Aegis ID ↔ public key match, inserts if free, returns the ID.
- `POST /identity/deregister` — currently disabled and returns 501 until secure authorisation exists.
- `GET /identity/lookup/{id}` — returns public key by Aegis ID, or 404 if missing.

## SecureMessagingHub

Location: `AegisLink.Server/Hubs/SecureMessagingHub.cs`

Role: blind relay for ciphertext in a room identified by a deterministic session ID.

Methods:

- `JoinSession(string sessionId)` — validates session ID and adds connection to the group.
- `SendMessage(string sessionId, string payload)` — relays ciphertext to all other members.
- `DeleteMessage(string sessionId, Guid messageId)` — relays delete events.
- `OnDisconnectedAsync(Exception?)` — removes connection tracking.

Validation:

- Session ID must match `^[0-9a-f]{32}$` (first 16 bytes of SHA‑256 as hex).
- Relay only happens if the connection previously joined the session.

## AegisIdService

Location: `AegisLink.Server/Services/AegisIdService.cs`

Methods:

- `CreateId(byte[] publicKeyBytes)` — SHA‑256 → first 40 bits → Base32 → 8‑char ID.
- `Verify(string claimedId, string publicKeyB64)` — recomputes ID and compares to claim.

## HubSessionTracker

Location: `AegisLink.Server/Services/HubSessionTracker.cs`

Role: in‑memory map of connection IDs to joined sessions.

Methods:

- `Join(string connectionId, string sessionId)` — adds a session to the connection set.
- `IsMember(string connectionId, string sessionId)` — membership check.
- `Remove(string connectionId)` — clears the connection record.

## Data layer

### AegisLinkDbContext

Location: `AegisLink.Server/Data/AegisLinkDbContext.cs`

- Exposes `DbSet<UserKey>`.
- Configures `AegisId` as primary key.
- Enforces unique public keys.

### UserKey

Location: `AegisLink.Server/Data/UserKey.cs`

Fields:

- `AegisId` — 8‑char ID (primary key).
- `PublicKey` — base64 public key (unique).

### Migrations

Location: `AegisLink.Server/Migrations/`

- `InitialCreate` builds the `UserKeys` table and unique index on `PublicKey`.

### HTTP scratch file

Location: `AegisLink.Server/WebApplication1.http`

- Simple REST client snippet for quick manual calls during development.

## See also

- [Shared DTOs](09-shared-dtos.md)
- [Configuration and hosting assets](10-config-hosting.md)
- [Back to index](README.md)
