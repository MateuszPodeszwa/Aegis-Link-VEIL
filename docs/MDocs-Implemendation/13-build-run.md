# Build, Run, and Local Dev Notes

Quick notes for building and running the solution in development.

## Prerequisites

- .NET 10 SDK

## Common commands

- `dotnet restore` — restores packages.
- `dotnet build Aegis-Link-VEIL.sln` — builds all projects.
- `dotnet run --project AegisLink.Server` — runs the API + SignalR hub.
- `dotnet run --project Aegis-Link-VEIL` — runs the Blazor client.
- `dotnet test Aegis-Link-VEIL.sln` — runs test targets (none defined).

## Dev URLs

- Client: `https://localhost:7035`
- Server: `https://localhost:7069`

## See also

- [Configuration and hosting assets](10-config-hosting.md)
- [Back to index](README.md)
