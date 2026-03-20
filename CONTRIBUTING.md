# Developer Contribution Guidelines

## 1. Workflow Strategy
We utilise the **Feature Branch** workflow.
*   **Main Branch:** `main` (Production-ready code only. DO NOT PUSH DIRECTLY HERE.)
*   **Development Branch:** `develop` (Integration branch.)
*   **Default Feature Branches:** `feature/secure-handshake`, `feature/ui-update`, etc.
*   **Backlog Feature Branches:** Pattern: `feature-[backlog_ID]/secure-handshake`, Example: `feature-AL-002/UI-Update`, etc.

### Information to the Team and Developers working on this project.
Please refer to the backlog when creating new branches. If a feature does not exist in the backlog, use the default feature branch naming. If a feature exists in the backlog, including the backlog feature ID in the branch name (as specified above) is mandatory.

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
