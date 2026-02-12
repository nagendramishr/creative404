# Test Suite

This directory will contain test files for the creative404 .NET Blazor project.

## Structure

```
tests/
├── unit/              # Unit tests
├── integration/       # Integration tests
└── e2e/              # End-to-end tests
```

## Running Tests

Tests can be added using .NET testing frameworks (xUnit, NUnit, or MSTest).

```bash
dotnet test
```

## Test Coverage

Aim for:
- Unit tests: Component logic and business rules
- Integration tests: Component interactions
- E2E tests: User workflows with bUnit or Playwright
