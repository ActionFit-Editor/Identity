# AI Guide - ActionFit Identity

This file is shipped inside the UPM package so an AI assistant in a consuming Unity project can understand the package without access to the source project's `Docs/AI` folder.

## Package Identity

- Package ID: `com.actionfit.identity`
- Display name: ActionFit Identity
- Repository: `https://github.com/ActionFitGames/Identity.git`
- Current package version at generation time: `1.0.3`
- Unity version: `6000.2`

## Purpose

ActionFit Identity provides a reusable installation ID resolver with pluggable durable storage, ordered legacy migration sources, explicit recovery replacement, and diagnostics that do not require logging the raw identifier.

It owns installation identity resolution only. It does not own authenticated account login, account merge UI, cloud save, analytics SDK setup, Firebase access, or project-specific legacy key definitions.

## Agent Skills

- `Skills~/manifest.json` registers schema v2 `identity-help` and `identity-audit` for Codex and Claude with read-only access.
- Help reads the generated `PACKAGE_SKILLS.md` inventory before explaining canonical storage, ordered migration, replacement, tests, and privacy boundaries.
- Audit inspects source and adapter definitions only. It must not read stored identifier values, resolve or replace an ID, access persistence, edit files, or expose raw installation, account, advertising, or recovery identifiers.

## Project Router Registration

This package should be listed in `Packages/com.actionfit.custompackagemanager/PACKAGE_AI_GUIDE_ROUTER.md`.

Requested router entry:

- `Packages/com.actionfit.identity/AI_GUIDE.md` - ActionFit Identity resolves and persists a stable installation ID. Read when changing installation ID storage, ordered legacy identity migration, explicit recovery replacement, or project identity adapters.

If the router file is not already included in the AI assistant's default reading sequence, ask the user where the package router should be linked instead of silently creating a new project AI entry point.

Read this file when:

- changing files under `Packages/com.actionfit.identity/`
- integrating `InstallationIdentityService` into a project
- changing installation ID storage or migration order
- changing explicit ID replacement behavior used by account recovery
- preparing a release for `com.actionfit.identity`

## Runtime Architecture

- `IInstallationIdStore` owns the canonical durable read/write boundary.
- `IInstallationIdMigrationSource` exposes one read-only legacy candidate and a non-sensitive source name.
- `IInstallationIdGenerator` creates an ID only after canonical storage and every migration source return no usable value.
- `InstallationIdentityService.Resolve()` preserves canonical storage first, evaluates migration sources in registration order, persists the first usable candidate, and generates a GUID only as the final fallback.
- `InstallationIdentityService.ReplaceId()` is reserved for an explicit recovery or conflict-resolution decision. It rejects null, empty, and whitespace-only values.
- `InstallationIdResolutionKind` and `MigrationSource` are safe diagnostic metadata. Do not log `InstallationIdResolution.InstallationId`.

The service deliberately propagates storage and migration-source exceptions. Generating a new ID after a storage read failure could detach an existing player from server-side data.

## Integration Rules

- Keep project-specific persistence, SDK legacy keys, and migration ordering in a project adapter.
- Preserve existing canonical IDs exactly. Do not normalize, hash, delete, or regenerate them during package adoption.
- Keep legacy sources read-only. Persist the selected value only through `IInstallationIdStore`.
- Configure one long-lived service instance per installation identity domain.
- Do not use the installation ID as an authentication secret.
- Never include raw installation IDs in logs, exceptions, analytics events, or package diagnostics.
- Add tests for every project-specific migration source and its priority before changing the order.

## Package Tools Menu

- Unity menu root: `Tools/Package/ActionFit Identity/`.
- `README`: opens the installed package README.
- This package has no settings ScriptableObject and therefore does not expose `Setting SO`.

## Release Notes

- Publishing is manual through Custom Package Manager.
- Before reusing a version, check remote Git tags. Published tags are immutable.
- Update `package.json`, this guide's current version, README usage, and `Editor/PackageInfo/ActionFitPackageInfo_SO.asset` together for behavior changes.
- The package repository should include this `AI_GUIDE.md` so consuming projects receive the same integration rules.
