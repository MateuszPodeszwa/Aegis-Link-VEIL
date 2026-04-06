# Aegis Link (Project V.E.I.L.)

> **CLASSIFICATION:** PRIVATE / PROPRIETARY
> **STATUS:** ACTIVE DEVELOPMENT (MVP Phase)
> **FRAMEWORK:** .NET 10 / C# 14

## Executive Summary

**Aegis Link** is a secure, end-to-end encrypted (E2EE) messaging platform built on the **V.E.I.L. (Verified Encrypted Instant Link)** architecture. All cryptography runs client-side in the browser (Blazor WebAssembly) via NaCl/libsodium. The server acts as a blind relay — it never sees plaintext messages or private keys.

---

## Current Status

### Implemented
- **Identity system** — NaCl keypair generated in-browser; Aegis ID (8-char Base32) derived from SHA-256 of the public key; registered with the server at startup
- **ECDH key exchange** — `nacl.box.before()` (Curve25519) derives a shared secret from own secret key + partner's public key; both parties independently arrive at the same key without transmitting it
- **End-to-end encryption** — messages encrypted with XSalsa20-Poly1305 (`nacl.secretbox`) using the ECDH-derived key; server only ever relays ciphertext
- **Identity-bound sessions** — establishing a connection requires the partner's Aegis ID; the server is queried to verify the ID exists and retrieve their public key before a session is created
- **SignalR relay** — real-time message delivery via `/chatHub`; server forwards ciphertext blobs, has no ability to decrypt
- **Session storage** — chat sessions and message history held in `sessionStorage` (ephemeral by default, cleared on tab close)

### Not Yet Implemented
- ECDH handshake initiated by the *receiving* party (currently the recipient must also enter the initiator's Aegis ID manually to join the same session — a proper invite/notification flow is pending)
- Persistent storage mode
- QR code out-of-band key verification
- Audit/compliance extension
- Killswitch (session wipe)
- WeatherForecast stub endpoints not yet removed

---

## Architecture

### Solution Structure

| Project | Type | Purpose |
|---------|------|---------|
| `Aegis-Link-VEIL/` | Blazor WebAssembly PWA | Client UI, crypto, session management |
| `AegisLink.Server/` | ASP.NET Core 10 | REST API, SignalR relay hub, identity storage |
| `AegisLink.Shared/` | .NET Class Library | Shared DTOs |

### Key Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/identity/register` | POST | Register Aegis ID + public key |
| `/identity/lookup/{id}` | GET | Retrieve public key for a given Aegis ID |
| `/chatHub` | WS | SignalR hub — blind message relay |

### Dev Ports

| Service | URL |
|---------|-----|
| Client (Blazor WASM) | `https://localhost:7035` |
| Server (API + SignalR) | `https://localhost:7069` |

---

## Cryptography

### Key Generation
- NaCl keypair generated in-browser via `crypto-interop.js` (libsodium.js)
- Aegis ID = SHA-256(public key) → first 40 bits → Base32 → 8 characters
- Private key stored in `localStorage` only, never transmitted

### Message Encryption Flow
1. Alice registers her public key → server stores `(AegisId → PublicKey)` mapping
2. Alice enters Bob's Aegis ID → client fetches Bob's public key from `/identity/lookup/`
3. `sharedKey = nacl.box.before(bob_pk, alice_sk)` — Curve25519 ECDH
4. Bob does the same: `nacl.box.before(alice_pk, bob_sk)` — same shared key, derived independently
5. Every message: `nacl.secretbox(plaintext, randomNonce, sharedKey)` → ciphertext transmitted
6. Recipient: `nacl.secretbox.open(ciphertext, nonce, sharedKey)` → plaintext locally
7. Server relays `base64(nonce || ciphertext)` blobs — no decryption capability

---

## Development Setup

1. **Prerequisites:** .NET 10 SDK
2. **Clone:** `git clone https://github.com/MateuszPodeszwa/Aegis-Link-VEIL`
3. **Restore:** `dotnet restore`
4. **CORS / Base Address:** Ensure `AegisLink.Server/appsettings.Development.json` `AllowedOrigins` matches the client URL (`https://localhost:7035`). Confirm `Aegis-Link-VEIL/Program.cs` `BaseAddress` points to the server (`https://localhost:7069`).
5. **Run both projects** — server must start before the WASM client. Use a multi-project launch profile or run each in a separate terminal:
   ```
   dotnet run --project AegisLink.Server
   dotnet run --project Aegis-Link-VEIL
   ```

---

## Contact

- **Lead Developer:** Mateusz Podeszwa
- **Organisation:** Teesside University (Software Engineering Y2) — Aegis Link Group
