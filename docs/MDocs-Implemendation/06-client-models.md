# Client Models

Core data objects used by the client app.

## AppSettings

Location: `Aegis-Link-VEIL/Models/AppSettings.cs`

Properties:

- `bool AutoDeleteEnabled` — toggle for auto delete.
- `string AutoDeleteDuration` — label for delete duration.

## ChatMessage

Location: `Aegis-Link-VEIL/Models/ChatMessage.cs`

Properties:

- `Guid Id` — message identifier.
- `MessageSender From` — sender enum (`Me`, `Partner`, `System`).
- `string Content` — message body.
- `bool IsEdited` — edit flag.
- `DateTime SendAt` — timestamp (UTC).

## ChatSession

Location: `Aegis-Link-VEIL/Models/ChatSession.cs`

Properties:

- `string PartnerName` — display name.
- `string PartnerAegisId` — partner Aegis ID.
- `string PartnerPublicKey` — partner public key (base64).
- `string SessionKey` — room ID.
- `List<ChatMessage> Messages` — message history.

Methods:

- `BuildMessage(string content, MessageSender from)` — creates a new `ChatMessage`.
- `StoreMessage(ChatMessage message)` — appends to history.
- `DeleteMessage(Guid messageId)` — removes a message by ID.

## MessageSender

Location: `Aegis-Link-VEIL/Models/MessageSender.cs`

Enum values:

- `Me` — local user.
- `Partner` — remote user.
- `System` — system message.

## See also

- [Client services](05-client-services.md)
- [Client pages](04-client-pages.md)
- [Back to index](README.md)
