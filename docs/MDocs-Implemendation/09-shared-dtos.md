# Shared DTOs

Shared types live in `AegisLink.Shared/` and are used by both client and server.

## Contact

Location: `AegisLink.Shared/Contact.cs`

Fields:

- `string AegisId` — contact identifier.
- `string PublicKey` — partner public key (base64).
- `string Nickname` — display name.
- `string Note` — optional note.
- `string Colour` — avatar colour.

## UserKeyReg

Location: `AegisLink.Shared/UserKeyReg.cs`

Record fields:

- `string AegisId` — claimed identity.
- `string PublicKey` — public key for registration.

## UserKeyGetResult

Location: `AegisLink.Shared/UserKeyReg.cs`

Record fields:

- `string AegisId` — identity.
- `string PublicKey` — public key returned by lookup.

## See also

- [Server API and hub](08-server-api.md)
- [Client pages](04-client-pages.md)
- [Back to index](README.md)
