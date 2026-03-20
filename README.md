# Aegis Link (Project V.E.I.L.)

> **CLASSIFICATION:** PRIVATE / PROPRIETARY  
> **STATUS:** ACTIVE DEVELOPMENT (PoC Phase)
> **FRAMEWORK:** .NET 10 / C# 14

## Executive Summary
**Aegis Link** is a secure, ephemeral, end-to-end encrypted (E2EE) messaging platform designed for zero-trust environments. Utilising the **V.E.I.L. (Virtual Encrypted Information Link)** architecture, the system ensures that message payloads are encrypted client-side within the browser (Blazor WebAssembly) before transmission.

The server acts solely as a blind relay. No cryptographic keys or plaintext messages are ever stored on the server in "Ephemeral Mode" (Subject to change)

## Key Capabilities

### Ephemeral (Default)
*   **Zero-Trace:** Messages exist only in Volatile Random Access Memory (VRAM).
*   **Offline Handshake:** Public key exchange occurs via physical QR scanning to prevent Man-in-the-Middle (MitM) attacks. (Subject to change)
*   **Ephemeral Keys:** Session keys are destroyed immediately upon session termination.

### Audit Extension
*   **Compliance Logging:** Optional dual-encryption protocols allow authorised auditors to recover message trails for legal compliance. Decentralised storage.
*   **Identity Verification:** Integration with specific institutional requirements.

## Technical Stack
*   **Client:** Blazor WebAssembly (Progressive Web Application)
*   **Server:** ASP.NET Core Web API
*   **Real-time Transport:** SignalR (WebSockets)
*   **Cryptography:** ECDH (Key Exchange) & AES-GCM (Payload Encryption)
*   **Database:** 
    *   *Ephemeral Mode:* In-Memory / Redis (Subject to change)
    *   *Audit Mode:* On device memory. 
      
## Development Setup
1.  **Prerequisites:** .NET 10 SDK
2.  **Clone:** `git clone https://github.com/MateuszPodeszwa/Aegis-Link-VEIL`
3.  **Checkout to Develop or Master** `Depending on the current project status, Master branch may not contain up-to-date solution`
4.  **Restore:** `dotnet restore`
5.  **Create Launch Profile** `Create Run-Multiple run profile. On HTTP (for development). Ensure AegisLink.Server is running before WASM`
6.  **Update CORS, API BaseAddress:** `Make sure that Server/AppSettings.Development.json have correct AllowedOrigins URL for the Client WASM. Accordingly, see the client.BaseAddress in the Aegis-Link-VEIL/Program.cs to contain correct API URL`
7.  **Run:** `dotnet run --project src/Aegis.Server`

## Contact
*   **Lead Developer:** Mateusz Podeszwa
*   **Organisation:** Teesside University (Software Engineering Y2) the **Aegis Link Gr.**
