---
name: identity-audit
description: Audit ActionFit Identity source and project adapters for canonical storage priority, ordered legacy migration, failure propagation, explicit replacement guards, and privacy-safe diagnostics without reading or changing identifier values. Use when reviewing identity package changes or integrations.
---

# Audit ActionFit Identity

Keep the audit read-only and metadata-only. Never read PlayerPrefs, databases, cloud records, logs containing raw IDs, or other stored identifier values. Do not resolve, generate, replace, migrate, normalize, hash, copy, or delete an identifier.

1. Read the repository instructions only, so project routing and safety rules apply before inspection.
2. From the repository root, capture `git status --short --untracked-files=all` as the audit baseline. Preserve all pre-existing changes.
3. Resolve the physical package root from `Packages/com.actionfit.identity`; otherwise use `Library/PackageCache/com.actionfit.identity@*` without editing it. Then read the package `README.md` and `AI_GUIDE.md`, plus the consuming project's identity architecture document when present.
4. Use `rg` and read-only file inspection to trace `IInstallationIdStore`, `IInstallationIdMigrationSource`, `IInstallationIdGenerator`, `InstallationIdentityService`, resolution metadata, project adapters, direct identifier consumers, diagnostics, and tests. Inspect source definitions, never runtime values.
5. Verify and report evidence for these contracts:
   - Canonical storage wins before any migration source, and migration sources are evaluated in registration order.
   - Generation occurs only when canonical storage and every migration source have no usable value.
   - Storage and migration-source exceptions propagate instead of silently creating a replacement ID.
   - Explicit replacement rejects blank input and is not invoked during normal startup or package adoption.
   - Legacy sources remain read-only and the selected value is persisted only through the canonical store.
   - Diagnostics use resolution kind and non-sensitive source names without raw installation, account, advertising, or recovery identifiers. Treat raw `Exception.Message` and exception chains as potential identifier-bearing paths until source proves otherwise.
6. Inspect package dependencies, asmdefs, and deterministic test coverage, including canonical-save failures, adapter readiness and compile branches, and diagnostic redaction. For Cat Merge Cafe, verify project readiness, migration priority, and direct-consumer diagnostics from source and docs without querying `DataStore` or PlayerPrefs.
7. Capture the same Git status command again and compare it with the baseline. If state changed during the audit, report the paths and do not claim a no-change result.
8. Return findings grouped as passed contracts, risks, missing evidence, and recommended validation. Mention package or project tests as follow-up commands; do not run them from this skill.
