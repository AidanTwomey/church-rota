<!--
  Sync Impact Report
  Version change: 0.0.0 → 1.0.0 (initial ratification)
  Added sections:
    - Principle I: Headless SMS-First
    - Principle II: Cost-Minimized Cloud Architecture
    - Principle III: .NET LTS Only
    - Principle IV: Test-Driven Development (NON-NEGOTIABLE)
    - Principle V: Simplicity
    - Section: Technology Constraints
    - Section: Development Workflow
    - Governance
  Removed sections: none
  Templates requiring updates:
    - .specify/templates/plan-template.md ✅ no changes needed (generic)
    - .specify/templates/spec-template.md ✅ no changes needed (generic)
    - .specify/templates/tasks-template.md ✅ no changes needed (generic)
  Follow-up TODOs: none
-->

# Church Rota Constitution

## Core Principles

### I. Headless SMS-First

All user interaction MUST occur via SMS messages. There MUST be no web UI,
mobile app, or other graphical interface. The system is headless: it receives
SMS commands, processes them, and responds via SMS. Any administrative or
operational task (viewing the rota, requesting swaps, confirming availability)
MUST be achievable through SMS alone.

**Rationale**: Volunteers span a wide range of technical ability and device
ownership. SMS is universally accessible, requires no app installation, and
works on every phone.

### II. Cost-Minimized Cloud Architecture

Every architectural decision MUST favour the option that runs at the lowest
possible cost on Azure. When evaluating components the team MUST prefer:

- Consumption/serverless plans over always-on compute (e.g. Azure Functions
  Consumption plan over App Service)
- File-based or table storage over relational databases when the data model
  permits (e.g. Azure Table Storage or Blob Storage over Azure SQL)
- Built-in platform features over third-party paid services
- Free-tier or pay-per-execution services over reserved capacity

Cost MUST be treated as a first-class architectural constraint, not an
afterthought. Any component that introduces recurring fixed cost MUST be
justified in writing with evidence that no cheaper alternative exists.

**Rationale**: This is a volunteer-run church project with minimal budget.
Traffic is low and sporadic. The architecture MUST reflect that reality.

### III. .NET LTS Only

The project MUST target the current Long Term Support (LTS) release of .NET.
Non-LTS (Standard Term Support) versions MUST NOT be used in production. When
a new LTS version becomes available, the project SHOULD upgrade within a
reasonable timeframe but MUST NOT skip directly from one LTS to another
without verifying compatibility.

**Rationale**: LTS releases receive three years of security patches and bug
fixes, providing stability appropriate for a low-maintenance volunteer
project.

### IV. Test-Driven Development (NON-NEGOTIABLE)

All production code MUST be developed using the Red-Green-Refactor cycle:

1. **Red**: Write a failing test that describes the desired behaviour.
2. **Green**: Write the minimum production code to make the test pass.
3. **Refactor**: Improve the code while keeping all tests green.

No production code may be written without a corresponding failing test first.
Tests MUST be committed alongside (or before) the production code they cover.
Test names MUST clearly describe the behaviour under test.

**Rationale**: TDD produces code that is correct by construction, well-
factored, and safe to change. For a volunteer-maintained project with
infrequent development bursts, comprehensive tests are the primary safety
net.

### V. Simplicity

The simplest solution that meets the current requirement MUST be chosen.
YAGNI (You Aren't Gonna Need It) applies at every level: no speculative
abstractions, no premature generalisation, no features beyond what is
explicitly requested. Prefer fewer projects, fewer layers, and fewer
dependencies. Three similar lines of code are better than a premature
abstraction.

**Rationale**: Complexity is the primary enemy of a volunteer-maintained
codebase. Every unnecessary abstraction is a maintenance burden.

## Technology Constraints

- **Runtime**: .NET LTS (currently .NET 8; upgrade to .NET 10 when stable)
- **Cloud Provider**: Microsoft Azure (non-negotiable)
- **Compute**: Azure Functions on the Consumption plan
- **SMS Provider**: Azure Communication Services or equivalent lowest-cost
  option capable of sending/receiving SMS in the target region
- **Storage**: Azure Table Storage or Blob Storage preferred; Azure SQL
  permitted only if a relational model is demonstrably required and justified
- **Infrastructure as Code**: Terraform (already in use in `devops/`)
- **CI/CD**: GitHub Actions (already in use in `.github/workflows/`)

## Development Workflow

- All code changes MUST follow the TDD Red-Green-Refactor cycle
  (Principle IV)
- All code MUST compile against the current .NET LTS SDK
- All tests MUST pass before a pull request is merged
- Pull requests MUST be reviewed for adherence to this constitution
- Commits SHOULD be small, focused, and independently meaningful
- The `master` branch MUST always be in a deployable state

## Governance

This constitution is the supreme authority for architectural and process
decisions in the Church Rota project. It supersedes any conflicting guidance
in READMEs, ADRs, or inline comments.

**Amendments**: Any change to this constitution MUST be documented with a
version bump, rationale, and migration plan if existing code is affected.
Version numbers follow semantic versioning:

- **MAJOR**: Removal or redefinition of a principle
- **MINOR**: New principle or materially expanded guidance
- **PATCH**: Clarifications, wording, or non-semantic refinements

**Compliance**: All pull requests and code reviews MUST verify adherence to
these principles. Violations MUST be resolved before merge.

**Version**: 1.0.0 | **Ratified**: 2026-02-13 | **Last Amended**: 2026-02-13
