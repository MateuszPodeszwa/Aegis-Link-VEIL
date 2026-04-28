# Crypto Interop (Browser JavaScript)

Client‑side crypto and storage helpers live in `wwwroot/crypto-interop.js`. Functions are grouped below with their roles.

## Keypair and identity

- `generateKeypair()` — creates a NaCl `box` keypair and returns base64 keys.
- `createAegisId(publicKeyB64)` — hashes the public key with SHA‑256, takes the first 40 bits, and encodes Base32 to get the 8‑char Aegis ID.
- `saveIdentity(aegisId, publicKeyB64, secretKeyB64)` — stores identity in local storage key `aegis_identity`.
- `loadIdentity()` — loads identity from local storage, or returns `null`.
- `clearIdentity()` — removes `aegis_identity`.

## Device wipe

- `wipeDevice()` — removes Aegis keys and chats from both local and session storage.

## Shared key and encryption

- `computeSharedKey(mySecretKeyB64, partnerPublicKeyB64)` — derives the ECDH shared key with `nacl.box.before`.
- `boxEncrypt(plaintext, sharedKeyB64)` — encrypts plaintext with XSalsa20‑Poly1305 and returns `base64(nonce || ciphertext)`.
- `boxDecrypt(ciphertextB64, sharedKeyB64)` — decrypts a `boxEncrypt` payload and returns plaintext or `null`.

## Room ID

- `computeRoomId(myAegisId, partnerAegisId)` — SHA‑256 of sorted Aegis IDs, returns first 16 bytes as 32‑char hex string.

## Session‑key helpers (general)

- `deriveKey(sessionKeyString)` — SHA‑256 of a session key string.
- `encryptMessage(plaintext, sessionKeyString)` — `secretbox` encryption with derived key.
- `decryptMessage(ciphertextB64, sessionKeyString)` — `secretbox` decryption with derived key.
- `generateSessionKey()` — random 16‑byte hex session key.

## Settings storage

- `loadSettings()` — loads `aegis_settings` from local storage.
- `saveSettings(settings)` — writes settings to local storage.
- `deleteSettings()` — removes `aegis_settings`.

## Contacts storage

- `loadContacts()` — loads `aegis_contacts` or returns an empty list.
- `saveContact(contact)` — inserts or updates a contact by Aegis ID.
- `deleteContact(aegisId)` — removes a contact by Aegis ID.

## Visual helpers

- `colourForId(aegisId)` — deterministic avatar colour based on ID hash.

## Dependencies

- `nacl-fast.min.js` — core NaCl implementation.
- `nacl-util.min.js` — UTF‑8 helpers for NaCl.
- Web Crypto API — SHA‑256 hashing.

## See also

- [Client services](05-client-services.md)
- [Client pages](04-client-pages.md)
- [Back to index](README.md)
