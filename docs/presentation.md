---
title: dimpoc POC — Semantic Sanitization & Validation
author: Security Engineering / Data Safety
date: November 2025
---

# dimpoc — Data Sanitization POC

**Goal:** Demonstrate annotation-driven sanitization and validation of string fields in C# to reduce injection/XSS risk and make data-safe for downstream processing.

_Speaker note:_ Quick intro / 30s explanation of why field-level sanitization is useful beyond request-parameter validators.

---

## Problem Statement

- Security scanners find many injection/XSS opportunities in legacy apps.
- Validation libraries often operate only on HTTP request parameters, not arbitrary object fields.
- Fields may come from DBs, files, or external systems and still carry attack payloads.

_Speaker note:_ Emphasize risk, compliance exposure, and remediation backlog pain.

---

## Our Approach (POC)

- Use small C# attributes on string fields to encode *semantic types* (e.g., `TIN`, `PAN`, `Tel`, `USNUM`).
- A central processor inspects annotated fields and applies per-type handlers:
  - `Sanitize(object)` — strips attack characters and normalizes values
  - `Validate(object)` — lightweight checks (Luhn for PAN, length checks for TIN/Tel, numeric parse for USNUM)
- Returns `FieldResults` with per-field `FieldResultItem` entries: name, empty, valid

_Speaker note:_ This is low-friction for devs — annotate fields once, get consistent behavior across sources.

---

## Demo Summary (what we show today)

- Example object with annotated fields containing valid and attack-laden values
- Run `Sanitize(demo)` → fields are cleaned and nulls converted to empty
- Run `Validate(demo)` → lightweight validation (Luhn check, phone normalization)
- Output shows which fields changed (sanitized) and which validated OK

_Placeholder:_ add screenshot(s) of console output here: `docs/screenshots/sanitize-validate.png`

---

## Technical notes (for engineers)

- Implemented as a .NET 9 console project `dimpoc`
- Core files:
  - `Attributes.cs` — attribute types (`TIN`, `PAN`, `Tel`, `USNUM`)
  - `Sanitizer.cs` — `AnnotationProcessor` with `Sanitize` / `Validate` implementations
  - `FieldResults.cs` — result model
  - `Program.cs` — demo runner
- Design choices:
  - Simple, explicit attribute names for clarity in POC
  - Handler-based or hashmap registry approach is ready as next step for scale

_Speaker note:_ Mention potential next steps: registry of handlers, masking, role-based redaction, async validation.

---

## Business Benefits & Ask

- Benefits:
  - Reduces likelihood of SQLi/XSS from non-HTTP inputs
  - Standardizes sanitization across apps
  - Low developer friction — annotate fields; reuse handlers
- Ask / Request for Pilot Approval:
  1. Approve a 4-week pilot integrating annotations into one critical service
  2. Allocate 1-2 devs for implementation + 1 security SME for review
  3. Decide success criteria (reduction in high/critical findings, dev effort/time-to-fix)

_Speaker note:_ Provide rough estimate: small POC integration ~2-3 dev-days, pilot ~4–6 dev-weeks depending on scope.

---

## Next Steps / Roadmap

- Short-term (this quarter)
  - Pilot in one service, collect metrics, gather developer feedback
  - Add a handler registry and masking support (role-based)
- Mid-term
  - CI integration to surface missing annotations or inconsistent field handling
  - Developer docs, code snippets, and template PRs for common fixes
- Long-term
  - Expand to library/reusable package, integrate with SCA/scan workflows

---

## Appendix: Quick sample code (Sanitize / Validate)

```csharp
// Example attribute usage
public class DemoData {
  [TIN] public string SSN = "123-45-6789<script>";
  [PAN] public string Card = "4111 1111 1111 1111";
}

// Call from demo
var san = AnnotationProcessor.Sanitize(demo);
var val = AnnotationProcessor.Validate(demo);
```

_Speaker note:_ Point managers to `README.md` and full repo if they want to see the working demo.

---

*File created: `docs/presentation.md`. Edit this file in the repo; when ready I can export to PPTX or generate slides for VS Code/Reveal.*
