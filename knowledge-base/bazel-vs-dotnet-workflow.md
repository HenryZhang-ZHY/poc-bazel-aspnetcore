# Bazel vs dotnet CLI Workflow

## Context

For a large monorepo (1000+ .csproj), `dotnet build` can take 50+ minutes. Bazel provides incremental builds and caching. But Bazel lacks IDE integration for debugging .NET code.

## Two Approaches Explored

### Approach 1: Bazel + Parallel .csproj/.sln (Dual Build System)

Maintain both BUILD.bazel and .csproj files side by side.

- **Bazel** for CI, hermetic builds, incremental compilation
- **dotnet CLI** for local debugging via IDE

**Pros:** Full IDE debugging support, familiar `dotnet test` workflow  
**Cons:** Must keep two build systems in sync. Not scalable at 1000+ projects unless auto-generated (e.g., via Gazelle).

### Approach 2: Bazel-Only (No .csproj)

Use Bazel for everything. Debug by running Bazel-built DLLs directly with `dotnet exec` under a debugger.

**Pros:** Single source of truth for builds  
**Cons:** Debugging setup is manual and fragile. Source mapping issues on Windows.

## When to Use Each Tool

| Task | Tool | Why |
|---|---|---|
| Full build | `bazelisk build //...` | Incremental, cached |
| Run all tests | `bazelisk test //...` | Cached test results |
| Debug a specific test | `dotnet test --filter <TestName>` (if .csproj exists) or VS Code launch config | Debugger support |
| CI | `bazelisk test //...` | Hermetic, reproducible |

## Bazel Performance Benefits (at Scale)

| Scenario | dotnet | Bazel |
|---|---|---|
| Changed 1 file in 50-project monorepo | Rebuilds many projects | Rebuilds only affected targets |
| Re-run tests with no code change | Re-runs all tests | Returns cached results instantly |
| CI build after small PR | Full rebuild | Only affected targets |
| Cross-team builds | N/A | Remote cache sharing |

## Key Insight

Bazel is a **build and test system**, not a development environment. The ideal setup:

- **Inner loop** (code → debug → iterate): Use IDE-native tooling
- **Outer loop** (build → test → ship): Use Bazel

For .NET specifically, `rules_dotnet` is less mature than Bazel rules for Java/Go/Python. The IDE integration gap is a real limitation that must be worked around.
