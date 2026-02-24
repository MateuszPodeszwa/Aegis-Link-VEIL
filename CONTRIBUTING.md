# Developer Contribution Guidelines

## 1. Workflow Strategy
We utilise the **Feature Branch** workflow.
*   **Main Branch:** `main` (Production-ready code only. DO NOT PUSH DIRECTLY HERE.)
*   **Development Branch:** `develop` (Integration branch.)
*   **Feature Branches:** `feature/secure-handshake`, `feature/ui-update`, etc.

## 2. Commit Standards
Commits must be clear and atomic.
*   ✅ `feat: implemented ECDH key generation logic`
*   ✅ `fix: resolved SignalR reconnection timeout`
*   ❌ `updated stuff`

## 3. Security Protocol (CRITICAL)
*   **NEVER** commit secrets, API keys, or private certificates to the repository.
*   If you accidentally commit a secret, report it to the team lead immediately for rotation.
*   Ensure all cryptographic logic is unit tested before merging.

## 4. Code Style
*   Follow standard C# .NET conventions.
*   Use `async/await` properly (avoid `.Result`).
*   Comment on all cryptographic methods, explaining *why* a specific approach was taken.
