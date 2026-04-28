# Storage and Data Handling

Where data lives and how it moves between client and server.

## Browser storage keys

Local storage:

- `aegis_identity` — identity object `{ aegisId, publicKey, secretKey }`.
- `aegis_contacts` — saved contacts list.
- `aegis_settings` — settings object for auto delete.

Session storage:

- `aegis_chats` — list of `ChatSession` records and message history.

## Client data flow

- Identity is created in the browser and stored in `aegis_identity`.
- Contacts are stored in `aegis_contacts` for quick lookup and display.
- Sessions and messages are stored in `aegis_chats` and cleared when the tab closes.
- Killswitch wipes all Aegis storage keys in local and session storage.

## Server data flow

- SQLite stores only the `(AegisId → PublicKey)` mapping.
- Messages are never stored server‑side; the hub relays ciphertext only.

## See also

- [Crypto interop](07-crypto-interop.md)
- [Server API and hub](08-server-api.md)
- [Back to index](README.md)
