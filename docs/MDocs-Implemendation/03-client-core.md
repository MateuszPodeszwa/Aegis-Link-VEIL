# Client App Core (Blazor WASM)

Core wiring for the client app: dependency injection, routing, and layouts.

## Program.cs

Location: `Aegis-Link-VEIL/Program.cs`

Key behaviour:

- Creates the WebAssembly host and registers root components (`App`, `HeadOutlet`).
- Registers client services via DI: `ChatHubService`, `ChatCryptoService`, `ChatStore`, `ContactService`, `SettingsService`.
- Registers default `HttpClient` for static assets and a named `WebAPI` client for the server base address.
- Reads `WebApi:BaseAddress` from `wwwroot/appsettings.json` and fails fast if missing.

## App.razor

Location: `Aegis-Link-VEIL/App.razor`

Key behaviour:

- Sets up the router and default layout (`MainLayout`).
- Initialises `ChatStore` on app start so session data is ready for pages.

## _Imports.razor

Location: `Aegis-Link-VEIL/_Imports.razor`

Purpose:

- Central list of common `@using` statements for pages and components.

## Layouts

### MainLayout.razor

Location: `Aegis-Link-VEIL/Layout/MainLayout.razor`

- Wraps the page in a sidebar + main content shell.
- Includes the navigation menu and a top row.

### NavMenu.razor

Location: `Aegis-Link-VEIL/Layout/NavMenu.razor`

- Shows the app brand and toggles nav visibility on small screens.
- Provides links to Home and Contacts.

### EmptyLayout.razor

Location: `Aegis-Link-VEIL/Layout/EmptyLayout.razor`

- Bare layout for full‑screen pages (chat, killswitch, settings).

## See also

- [Client pages and UI flows](04-client-pages.md)
- [Client services](05-client-services.md)
- [Back to index](README.md)
