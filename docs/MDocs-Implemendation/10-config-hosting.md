# Configuration and Hosting Assets

Configuration files and static host assets used by the client and server.

## Client configuration

### `wwwroot/appsettings.json`

- `WebApi:BaseAddress` — base URL for the API and SignalR hub.

### `Aegis-Link-VEIL.csproj`

- Targets `net10.0` with nullable and implicit usings enabled.
- Packages: `Microsoft.AspNetCore.Components.WebAssembly`, `DevServer`, `SignalR.Client`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Http`, `System.Net.Http`.
- Declares the service worker assets and references `AegisLink.Shared`.

### `Aegis-Link-VEIL/Properties/launchSettings.json`

- `https` profile exposes `https://localhost:7035` and `https://localhost:5150`.
- `http` profile uses `https://localhost:5150` for development.

## Server configuration

### `AegisLink.Server/appsettings.json`

- Default logging levels.
- `AllowedOrigins` array is empty by default.

### `AegisLink.Server/appsettings.Development.json`

- `AllowedOrigins` includes the client dev ports.
- `Kestrel.Endpoints` exposes `https://localhost:7069` and `http://localhost:5162`.

### `AegisLink.Server/Properties/launchSettings.json`

- Both `http` and `https` profiles expose `https://localhost:7069` and `http://localhost:5162`.

### `AegisLink.Server.csproj`

- Targets `net10.0` with nullable and implicit usings enabled.
- Packages: `Microsoft.AspNetCore.OpenApi`, `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`.
- References `AegisLink.Shared`.

### `AegisLink.Shared.csproj`

- Targets `net10.0` with nullable and implicit usings enabled.

## Client host assets

### `wwwroot/index.html`

- Loads Bootstrap CSS, app CSS, and the generated Blazor styles.
- Loads NaCl scripts and `crypto-interop.js`.
- Registers `service-worker.js`.

### Service workers

- `wwwroot/service-worker.js` — dev mode, no caching.
- `wwwroot/service-worker.published.js` — production offline cache based on the assets manifest.

### `wwwroot/manifest.webmanifest`

- PWA metadata and app icons.

## See also

- [UI assets and styling](12-ui-assets.md)
- [Build and run notes](13-build-run.md)
- [Back to index](README.md)
