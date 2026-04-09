# Bazel on Windows — Known Issues

## "Repository will be fetched again since the file has been modified externally"

### Symptom

```
WARNING: Repository '@@rules_dotnet++dotnet+dotnet_x86_64-pc-windows-msvc' will be fetched again
since the file '...Microsoft.CodeAnalysis.Workspaces.resources.dll' has been modified externally.
```

This warning appears frequently and causes Bazel to re-fetch repositories, slowing builds.

### Root Cause

**Windows Defender real-time protection** scans DLL files in Bazel's output base, modifying file metadata (timestamps or alternate data streams). Bazel detects the checksum/metadata change and assumes the file was tampered with.

### Bazel Output Base Location

```
C:\users\<username>\_bazel_<username>\<hash>
```

Find it with: `bazelisk info output_base`

### Fix (Requires Admin PowerShell)

```powershell
Add-MpPreference -ExclusionPath "C:\users\<username>\_bazel_<username>"
Add-MpPreference -ExclusionPath "<workspace>\bazel-bin"
Add-MpPreference -ExclusionPath "<workspace>\bazel-out"
```

### Other Potential Causes

- **dotnet CLI** touching Bazel-managed SDK files (if .csproj files coexist)
- **VS Code C# extension** background indexing/scanning

## General Windows + Bazel Friction

| Issue | Description |
|---|---|
| Symlinks | Bazel relies on symlinks; Windows requires developer mode or admin privileges |
| Path length | Windows 260-char path limit can break deep Bazel output paths |
| Defender scanning | Causes "modified externally" warnings (see above) |
| File locking | Windows file locking conflicts with Bazel's output management |
| Performance | Bazel is primarily optimized and tested on Linux |

## Recommendation

| Environment | Suitability |
|---|---|
| **WSL2 + Bazel** | Best experience on Windows machines |
| **Dev Containers** | Consistent team environment, VS Code Remote |
| **Linux CI** | Always use Linux for Bazel CI |
| **Windows native** | Expect friction; use only if required |
