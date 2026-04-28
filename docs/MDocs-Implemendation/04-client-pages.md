# Client Pages and UI Flows

Each page is a Razor component with inline logic. This section lists the key state and methods per page.

## Home.razor (`/`)

Purpose: identity creation, Aegis ID display, and entry point to contacts.

Key state:

- `AegisIdentity? identity` — local identity snapshot.
- `bool isRegistering` — UI loading state.
- `string errorMessage` — registration error text.
- `List<Contact> contacts` — saved contacts list.

Key methods:

- `OnInitializedAsync()` — loads identity and contacts from local storage.
- `OpenContact(string aegisId)` — navigates to contact detail page.
- `CreateAccount()` — generates keypair, derives Aegis ID, registers identity, saves local identity.

Local records:

- `AegisIdentity` (AegisId, PublicKey, SecretKey).
- `GeneratedKeypair` (PublicKey, SecretKey).
- `StoredIdentity` (AegisId, PublicKey, SecretKey).
- `ErrorResponse` (Error).

## Contacts.razor (`/contacts`)

Purpose: start a session by resolving a partner Aegis ID and creating a room ID.

Key state:

- `string partnerAegisId` — input Aegis ID.
- `string sessionName` — display name.
- `bool isConnecting` — UI loading state.
- `string errorMessage` — error text.

Key methods:

- `OnInitializedAsync()` — initialises `ChatStore`.
- `Connect()` — validates input, loads identity, looks up partner key, computes room ID, creates chat session.

Local record:

- `StoredIdentity` (AegisId, PublicKey, SecretKey).

## AddContact.razor (`/contacts/add`)

Purpose: resolve a partner ID, verify key, and save a contact.

Key state:

- `LookupState` enum — Idle, Looking, Found, NotFound, AlreadyAdded, IsSelf.
- `string aegisIdInput` — ID input.
- `string foundPublicKey` — resolved partner key.
- `string nickname` — contact name.
- `string note` — contact note.
- `string errorMessage` — validation error.
- `bool isSaving` — save in progress.
- `string myAegisId` — local identity ID.

Key methods:

- `OnInitializedAsync()` — loads local identity for self‑ID checks.
- `OnIdInput(ChangeEventArgs e)` — validates, checks duplicates, queries server, verifies key by re‑deriving ID.
- `SaveContact()` — builds `Contact` and saves via `ContactService`.

Local record:

- `StoredIdentity` (AegisId, PublicKey, SecretKey).

## ContactsDetail.razor (`/contacts/{AegisId}`)

Purpose: view and edit a saved contact, then start a chat.

Key state:

- `Contact? contact` — contact details.
- `bool isLoading` — load state.
- `string errorMessage` — errors.
- `bool isEditingNickname`, `string editNickname` — nickname edit.
- `bool isEditingNote`, `string editNote` — note edit.
- `bool confirmingDelete` — delete confirmation.
- `bool idCopied` — copy feedback.

Key methods:

- `OnInitializedAsync()` — loads contact by ID.
- `StartEditNickname()`, `SaveNickname()`, `CancelNicknameEdit()` — nickname edit flow.
- `StartEditNote()`, `SaveNote()`, `CancelNoteEdit()` — note edit flow.
- `CopyId()` — copies Aegis ID to clipboard.
- `StartChat()` — loads local identity, computes room ID, creates chat session.
- `ConfirmDelete()` — two‑step delete action.

## Chat.razor (`/chat/{SessionId}`)

Purpose: encrypted chat UI with SignalR relay.

Key state:

- `string SessionId` — room ID parameter.
- `ChatSession? chatSession` — current chat.
- `string? connectionError` — connection errors.
- `Guid? editingMessageId` — message edit state.
- `string messageInput` — input text.
- `string? sharedKey` — ECDH shared secret.
- `bool _eventsRegistered` — event subscription guard.

Key methods and fragments:

- `RenderMessage` — renders a single message bubble and action buttons.
- `GetMessageSeneder(ChatMessage msg)` — maps sender to CSS class.
- `OnInitializedAsync()` — initialises chat, loads identity, sets up crypto and SignalR.
- `InitializeSession()` — loads `ChatStore` and resolves the session.
- `LoadIdentity()` — loads local identity from JS.
- `SetupCrypto(StoredIdentity)` — computes shared key.
- `SetupSignalR()` — builds hub URL and connects.
- `RegisterHubEvents()` — wires `ChatHubService` event handlers.
- `HandleIncomingMessage(string decryptedJson)` — deserialises, stores, and updates UI.
- `HandleDeletedMessage(Guid id)` — removes message and updates UI.
- `HandleUserJoined()` — adds a system message.
- `HandleStatusChanged(string status)` — handles connection status.
- `SendMessage()` — builds message, encrypts, sends, stores.
- `EditMessage()` — edits, re‑encrypts, and updates message.
- `SendEncryptedMessage(ChatMessage message)` — serialise + `Crypto.Encrypt` + hub send.
- `DeleteMessage(Guid id)` — deletes message locally and via hub.
- `StartEdit(ChatMessage msg)`, `CancelEdit()` — edit UI flow.
- `ShowWaitingMessage()` — adds a system message on connect.
- `GetConnectionText()` — returns status label.
- `DisposeAsync()` — removes event handlers and disposes hub.
- `HandleKeyDown(KeyboardEventArgs e)` — enter‑to‑send/edit.
- `GoBack()` — returns to contacts list.

Local record:

- `StoredIdentity` (AegisId, PublicKey, SecretKey).

## Settings.razor (`/settings`)

Purpose: local settings for message auto‑delete.

Key state:

- `AppSettings settings` — current settings data.

Key methods:

- `OnInitializedAsync()` — loads settings via `SettingsService`.
- `SaveSettings()` — saves updates to local storage.
- `GoBack()` — returns to home.

## Killswitch.razor (`/killswitch`)

Purpose: wipe all local Aegis data and attempt server deregistration.

Key state:

- `string confirmInput` — confirmation text.
- `bool isExecuting` — running state.
- `bool confirmed` — completed state.
- `string errorMessage` — failure text.

Key methods:

- `Execute()` — optional server deregistration, local wipe, and UI update.

Local record:

- `StoredIdentity` (AegisId, PublicKey, SecretKey).

## NotFound.razor (`/not-found`)

Purpose: simple not‑found page for bad routes.

## Component.razor

Purpose: placeholder component with no content.

## Styling notes

- Page‑specific styles live in `Chat.razor.css`, `Contacts.razor.css`, `Home.razor.css`, and `Settings.razor.css`.

## See also

- [Client services](05-client-services.md)
- [Client models](06-client-models.md)
- [Back to index](README.md)
