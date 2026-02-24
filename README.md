# Aegis Link (Project V.E.I.L.)

> **CLASSIFICATION:** PRIVATE / PROPRIETARY  
> **STATUS:** ACTIVE DEVELOPMENT (PoC Phase)  
> **FRAMEWORK:** .NET 10 / C# 14

## Executive Summary
**Aegis Link** is a secure, end-to-end encrypted (E2EE) messaging platform designed for zero-trust environments. Utilising the **V.E.I.L. (Virtual Encrypted Information Link)** architecture, the system ensures that message payloads are encrypted client-side within the browser (Blazor WebAssembly) before transmission.

The server acts solely as a blind relay. No cryptographic keys or plaintext messages are ever stored on the server in "Standard Mode."

## Key Capabilities

### 🛡️ Military Mode (Default)
*   **Zero-Trace:** Messages exist only in Volatile Random Access Memory (VRAM).
*   **Offline Handshake:** Public key exchange occurs via physical QR scanning to prevent Man-in-the-Middle (MitM) attacks.
*   **Ephemeral Keys:** Session keys are destroyed immediately upon session termination.

### 🏥 NHS / Audit Extension
*   **Compliance Logging:** Optional dual-encryption protocols allow authorised auditors to recover message trails for legal compliance.
*   **Identity Verification:** Integration with specific institutional requirements.

## Technical Stack
*   **Client:** Blazor WebAssembly (PWA)
*   **Server:** ASP.NET Core Web API
*   **Real-time Transport:** SignalR (WebSockets)
*   **Cryptography:** ECDH (Key Exchange) & AES-GCM (Payload Encryption)
*   **Database:** 
    *   *Military Mode:* In-Memory / Redis (Ephemeral)
    *   *Audit Mode:* SQL Server (Entity Framework Core)

## Development Setup
1.  **Prerequisites:** .NET 10 SDK, Visual Studio 2026 / VS Code.
2.  **Clone:** `git clone [REPO_URL]`
3.  **Restore:** `dotnet restore`
4.  **Run:** `dotnet run --project src/Aegis.Server`

## Contact
*   **Project Lead:** Mateusz Podeszwa
*   **Organisation:** Teesside University (Software Engineering Group)
