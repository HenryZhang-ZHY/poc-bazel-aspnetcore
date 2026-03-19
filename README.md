# POC: Bazel for ASP.NET Core Web Application

This is a proof of concept for using Bazel to build a .NET web application.

## Benefits of building with Bazel

- Fast incremental builds: Bazel only rebuilds what has changed, which can significantly reduce build times.
- Smart tests selection: Bazel can automatically determine which tests need to be run based on the changes made, which can save time and resources.

## Disadvantages of building with Bazel

- Poor IDE support: Visual Studio / Rider do not have good support for Bazel, which can make development more difficult.
  - Someone tried to make the IDE support great, but they abandoned the [project](https://github.com/AFASResearch/rules_dotnet) since 2024-10-31.
