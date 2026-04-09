# Debugging Bazel-built .NET Tests

## Problem

Bazel builds .NET test DLLs but provides no built-in debugger integration. IDEs like VS/Rider rely on MSBuild/.csproj for their debug workflows, which don't exist in a Bazel-only setup.

## How IDE Debugging Works (First Principles)

IDE debugging is fundamentally 4 steps:

1. **Build** — Produce DLL + PDB
2. **Launch** — Start process: `dotnet exec <test>.dll`
3. **Attach** — Debugger (vsdbg) connects via DAP protocol
4. **Source Map** — PDB maps IL offsets ↔ source file paths + line numbers

VS/Rider automate all 4 steps. With Bazel, we must wire them manually.

## Key Finding: PDB Source Paths

Bazel's `csc` compiler stores **relative paths** in the PDB. Verified by inspecting the `.params` file:

```
# From bazel-bin/src/LibA2.Test/LibA2.Test/net10.0/LibA2.Test.dll-0.params
src/LibA2.Test/AdvancedCalculatorTests.cs
```

No `/pathmap` argument is set by `rules_dotnet`. This means the debugger must resolve `src/LibA2.Test/AdvancedCalculatorTests.cs` relative to the working directory.

## Bazel Test Binary Structure

After `bazelisk build //src/LibA2.Test:LibA2.Test`, the output is:

```
bazel-bin/src/LibA2.Test/LibA2.Test/net10.0/
├── LibA2.Test.dll              # Test assembly
├── LibA2.Test.pdb              # Debug symbols
├── LibA2.Test.dll.bat          # Bazel's test runner wrapper
├── LibA2.Test.dll.bat.runfiles/
│   ├── _main/src/...           # Source project outputs
│   └── rules_dotnet++dotnet+dotnet_x86_64-pc-windows-msvc/
│       └── dotnet.exe          # Bazel-managed .NET SDK
```

The `.bat` wrapper runs: `dotnet.exe exec LibA2.Test.dll`

## VS Code Debug Configuration

### launch.json

```json
{
  "name": "Debug Bazel Test (LibA2.Test)",
  "type": "coreclr",
  "request": "launch",
  "preLaunchTask": "bazel build //src/LibA2.Test:LibA2.Test",
  "program": "${workspaceFolder}/bazel-bin/src/LibA2.Test/LibA2.Test/net10.0/LibA2.Test.dll.bat.runfiles/rules_dotnet++dotnet+dotnet_x86_64-pc-windows-msvc/dotnet.exe",
  "args": [
    "exec",
    "${workspaceFolder}/bazel-bin/src/LibA2.Test/LibA2.Test/net10.0/LibA2.Test.dll"
  ],
  "cwd": "${workspaceFolder}",
  "env": {
    "DOTNET_ROOT": "${workspaceFolder}/bazel-bin/src/LibA2.Test/LibA2.Test/net10.0/LibA2.Test.dll.bat.runfiles/rules_dotnet++dotnet+dotnet_x86_64-pc-windows-msvc"
  },
  "justMyCode": false,
  "sourceFileMap": {
    "src": "${workspaceFolder}/src"
  }
}
```

### tasks.json (preLaunchTask)

```json
{
  "label": "bazel build //src/LibA2.Test:LibA2.Test",
  "type": "shell",
  "command": "bazelisk",
  "args": ["build", "//src/LibA2.Test:LibA2.Test"]
}
```

## What Didn't Work

| Approach | Result |
|---|---|
| `--test_env=VSTEST_HOST_DEBUG=1` | No effect — `csharp_nunit_test` uses NUnitLite, not vstest |
| `sourceFileMap: { ".": "${workspaceFolder}" }` | Error: "Specified path '.' in sourceFileMap is invalid" — vsdbg requires valid path prefixes |
| Maintaining parallel `.csproj` files | Not scalable for 1000+ projects |

## Open Issue

On Windows, the `sourceFileMap` with `"src"` as key may still produce `Could not load source` errors. This needs further investigation — may require `/pathmap` compiler argument in `rules_dotnet` or running on Linux/WSL2.
