# Security Policy

## Supported .NET Versions
| Version | Supported |
| ------- | ------------------ |
| > 10.x   | :white_check_mark: |
| < 10.0   | :x:                |

## Required Dependencies & Versions
| Version | Name |
| ------- | ------------------ |
| 10.0.00   | Microsoft.AspNetCore.Components.WebAssembly |
| 10.0.00   | Microsoft.AspNetCore.Components.WebAssembly.DevServer |
| 10.0.00   | Microsoft.AspNetCore.OpenApi |
| 10.0.100  | Microsoft.DotNet.HotReload.WebAssembly.Browser |
| 10.0.00    | Microsoft.AspNetCore.App.Internal.Assets |
| 10.0.3   | Microsoft.Extensions.DependencyInjection |
| 10.0.3   | Microsoft.Extensions.Http |
| 10.0.0   | Microsoft.NET.ILLink.Tasks |
| 10.0.0   | Microsoft.NET.Sdk.WebAssembly.Pack |
| 4.3.4   | System.Net.Http |

## Reporting a Vulnerability
**DO NOT OPEN A PUBLIC ISSUE** for security vulnerabilities.

If you discover a potential leak, broken encryption, or logic flaw:
1.  Stop development on the affected module.
2.  Contact the Project Lead directly via secure channel.
3.  Draft a fix in a private branch (`hotfix/security-patch`).

## Cryptographic Standards
*   **Randomness:** Use `System.Security.Cryptography.RandomNumberGenerator`, NEVER `System.Random`. (Depracitated)
*   **Key Size:** Minimum 256-bit for AES; Minimum 256-bit for Elliptic Curves.
