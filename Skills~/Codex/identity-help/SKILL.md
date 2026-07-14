---
name: identity-help
description: Explain ActionFit Identity, its installed skills, canonical storage and ordered migration contracts, explicit replacement, privacy-safe diagnostics, tests, and integration boundaries. Use when a user asks how installation identity resolution works or which package skill applies.
---

# ActionFit Identity Help

Answer in the user's language. Explain the package without reading stored identifier values, running an audit, changing identity state, or executing tests unless the user separately requests that operation.

1. Read `PACKAGE_SKILLS.md` first. Treat its generated package identity, complete related-skill table, `$skill-name` invocations, descriptions, and access boundaries as authoritative.
2. Read `Packages/com.actionfit.identity/README.md` and `Packages/com.actionfit.identity/AI_GUIDE.md` when present. If downloaded, resolve `Library/PackageCache/com.actionfit.identity@*` without editing it.
3. Explain canonical storage priority, ordered read-only migration sources, final fallback generation, propagated read failures, and explicit recovery-only replacement.
4. Keep account login, cloud save, analytics SDK setup, Firebase, project persistence keys, and migration order in consuming-project adapters. Do not treat an installation ID as an authentication secret.
5. List `README` under `Tools > Package > ActionFit Identity` and identify `com.actionfit.identity.Tests` as the deterministic test assembly.
6. State that package help and audit must not print or copy raw IDs, replace identifiers, migrate storage, delete legacy sources, publish, tag, or update the package catalog.
