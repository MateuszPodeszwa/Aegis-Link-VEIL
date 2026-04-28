# Project Overview and Tech Choices

Plain‑spoken summary of what the system does and why each tech piece exists.

## Purpose and scope

Aegis Link (V.E.I.L.) is a peer‑to‑peer messaging app with end‑to‑end encryption. The server acts as a blind relay, so message content stays client‑side. Identity is lightweight: an Aegis ID comes from a hash of the public key, with no usernames or emails.

## Core security model

- Keypairs are generated in the browser using NaCl (libsodium).
- Shared secrets come from ECDH (`nacl.box.before`) and never leave the browser.
- Messages are encrypted with XSalsa20‑Poly1305 (`nacl.secretbox`).
- The server forwards ciphertext only and has no decrypt path.

## Tech choices and justifications

- **Blazor WebAssembly** — keeps crypto in the browser and ships a PWA‑style client without a full back‑end render pipeline.
- **ASP.NET Core 10** — simple API hosting and native SignalR support in one stack.
- **SignalR** — low‑friction real‑time relay for ciphertext delivery.
- **Entity Framework Core + SQLite** — tiny footprint storage for public keys only.
- **NaCl/libsodium (nacl‑fast + nacl‑util)** — battle‑tested primitives with a small API surface.
- **Web Crypto API** — SHA‑256 hashing for Aegis ID and room ID creation.
- **Local/session storage** — local identity and sessions stay on the device; no server‑side message storage.

## See also

- [Solution layout and runtime flow](02-solution-structure.md)
- [Crypto interop](07-crypto-interop.md)
- [Server API and hub](08-server-api.md)
- [Back to index](README.md)
