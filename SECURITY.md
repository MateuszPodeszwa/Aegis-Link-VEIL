# Security Policy

## Supported Versions
| Version | Supported |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability
**DO NOT OPEN A PUBLIC ISSUE** for security vulnerabilities.

If you discover a potential leak, broken encryption, or logic flaw:
1.  Stop development on the affected module.
2.  Contact the Project Lead directly via secure channel.
3.  Draft a fix in a private branch (`hotfix/security-patch`).

## Cryptographic Standards
*   **Randomness:** Use `System.Security.Cryptography.RandomNumberGenerator`, NEVER `System.Random`.
*   **Key Size:** Minimum 256-bit for AES; Minimum 256-bit for Elliptic Curves.
