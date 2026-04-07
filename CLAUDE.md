# CLAUDE.md — Aegis Link (V.E.I.L.)

## Project Overview

**Aegis Link** is a secure, encrypted peer-to-peer messaging service built on a blind-relay architecture. The project acronym **V.E.I.L.** stands for Verified Encrypted Instant Link. The goal is to deliver end-to-end encrypted communication where the server never has access to message content.

### Core Security Model

- Client-side key generation only (NaCl.box / libsodium)
- ECDH key exchange during handshake to derive a shared secret
- All messages encrypted client-side before transmission
- Server acts as a **blind relay** — it never sees plaintext messages
- Two storage modes: **Ephemeral** (session-only, no persistence) and **Persistent** (user-controlled)

### Current Phase

MVP — core encrypted messaging pipeline is working. The priority is:
1. ~~Fix existing bugs and build errors~~ — done
2. ~~Implement end-to-end encrypted message flow (ECDH handshake → NaCl box encrypt/decrypt)~~ — done
3. Improve the session join UX — the receiving party currently enters the initiator's Aegis ID manually; a proper invite/notification flow is pending
4. Remove remaining boilerplate (WeatherForecast controller and shared model)
5. Implement persistent storage mode

---

## Architecture

### Solution Structure

| Project | Type | Purpose |
|---------|------|---------|
| `Aegis-Link-VEIL/` | Blazor WebAssembly PWA | Client UI, crypto, session management |
| `AegisLink.Server/` | ASP.NET Core 10 | REST API, SignalR relay hub, identity storage |
| `AegisLink.Shared/` | .NET Class Library | Shared DTOs between client and server |

### Tech Stack

- **.NET 10 / C# 14** — backend and Blazor WASM client
- **SignalR** — real-time message relay at `/chatHub`
- **Entity Framework Core 9 + SQLite** — identity/key storage only
- **NaCl.js (libsodium.js)** — client-side cryptography via JS interop
- **Blazor WASM** — SPA with `IJSRuntime` for crypto operations
- **SessionStorage** — ephemeral client-side message storage

### Key Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/identity/register` | POST | Register Aegis ID + public key |
| `/identity/lookup/{id}` | GET | Retrieve public key for a given Aegis ID |
| `/chatHub` | WS | SignalR hub for real-time message relay |

### Client Ports (Dev)

- Client: `https://localhost:7035`
- Server: `https://localhost:7069`

---

## Cryptography Design

### Key Generation

- NaCl.box keypair generated in the browser via `crypto-interop.js`
- Public key hashed with SHA-256, encoded as Base32 → produces the 8-char **Aegis ID**
- Private key never leaves the browser (stored in `localStorage` only)

### Intended Message Flow

1. Alice registers her public key on the server (Aegis ID → public key mapping)
2. Alice retrieves Bob's public key via `/identity/lookup/{bob-id}`
3. Alice and Bob perform ECDH to derive a shared secret (NaCl.box)
4. Every message is encrypted with the shared secret before being sent via SignalR
5. Server relays the ciphertext blob — it cannot decrypt anything
6. Recipient decrypts locally using their private key

### Current State

- Key generation works
- Identity registration and lookup work
- **ECDH key exchange is implemented** — `nacl.box.before(partnerPublicKey, mySecretKey)` derives a shared secret; both parties compute the same key independently
- **Messages are encrypted** — `nacl.secretbox` (XSalsa20-Poly1305) with the ECDH-derived key; server only relays `base64(nonce || ciphertext)`
- Establishing a session requires the partner's Aegis ID — the client looks up their public key before creating the chat
- Chat logic lives in `Chat.razor` inline `@code` block (no separate code-behind file)

---

## Development Guidelines

### Commit Policy

- Create a git commit on every major change (feature, bug fix, refactor milestone)
- Use clear, descriptive commit messages following the existing style
- Reference issue/feature numbers where applicable (e.g., `feat: implement ECDH handshake (#12)`)
- Never commit broken builds — verify the project compiles before committing

### Code Style

- Follow existing C# conventions in the codebase (nullable enabled, implicit usings)
- Keep crypto logic in JS interop (`crypto-interop.js`) — do not attempt to do cryptography in C#
- Blazor components use code-behind `.cs` files where logic is non-trivial
- Services are registered in `Program.cs` via dependency injection

### Security Rules

- **Never log or store plaintext message content on the server**
- **Never transmit private keys** — they must stay in the browser
- **Never trust server-side decryption** — the server is a relay, not a participant
- Validate all data at API boundaries (user input, SignalR messages)
- Do not add features that would give the server visibility into message content

### What Not to Do

- Do not add demo/boilerplate endpoints (WeatherForecast, Counter — these should be removed)
- Do not add backwards-compatibility shims or unused code
- Do not add error handling for scenarios that cannot happen
- Do not over-engineer — build the minimum needed for the current task
- Do not add docstrings or comments to code that was not changed

### Bug Fixing Priority

1. Fix compile errors and runtime crashes first
2. Then fix logic bugs in the crypto/messaging pipeline
3. Do not refactor surrounding code while fixing a bug — stay focused

---

## File Reference

| File | Role |
|------|------|
| `Aegis-Link-VEIL/wwwroot/crypto-interop.js` | All client-side crypto (key gen, ECDH, encrypt, decrypt) |
| `Aegis-Link-VEIL/Pages/Chat.razor` | Main chat UI and logic (inline `@code`) |
| `Aegis-Link-VEIL/Pages/Home.razor` | Identity creation and display |
| `Aegis-Link-VEIL/Services/ChatStore.cs` | Session storage service |
| `Aegis-Link-VEIL/Models/ChatSession.cs` | Chat session data model |
| `AegisLink.Server/Hubs/SecureMessagingHub.cs` | SignalR relay hub |
| `AegisLink.Server/Controllers/IdentityController.cs` | Identity register/lookup API |
| `AegisLink.Server/Data/AegisLinkDbContext.cs` | EF Core DbContext |
| `AegisLink.Shared/UserKeyReg.cs` | Shared DTO for identity registration |

---

## Identity Conventions

- **Aegis ID**: 8-character Base32 string derived from SHA-256 hash of a user's public key
- **Session ID**: generated per chat session (used as SignalR group name)
- **Storage keys**: `aegis_identity` (localStorage), `aegis_chats` (sessionStorage)
