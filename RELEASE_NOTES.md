# Release notes

## v1.1 - Self-contained builds

This release includes pre-built self-contained versions of FileConverter for the following platforms:

- Windows x86 (32-bit)
- Windows x64 (64-bit)
- Linux x64 (64-bit)

Checksums (SHA256):

- FileConverter-win-x86-selfcontained.zip: 4FB1BD96AF28CDBC340EE4A816E03B29B64D72E4F474B280D8EB2DBFAFE7AAC8
- FileConverter-win-x64-selfcontained.zip: CF8EEF50B1215FB49CE1970F0111B15BE9D3DE306CBAD4B33CC6AE1DC07A88DB
- FileConverter-linux-x64-selfcontained.zip: C182B52E921D130955A44625B20301C1600F5BFF60AF07FC1E7899E9C3C4F798

Notes:
- These builds include the .NET runtime and are larger than framework-dependent builds.
- FFmpeg is still required. Make sure FFmpeg is installed and available on PATH or configure the path in `Program.cs`.
