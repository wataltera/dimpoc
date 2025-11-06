# dimpoc

POC project demonstrating annotation-driven sanitization and validation of string fields in C#.

Targets: net9.0 (auto-detected from environment where this POC was created).

How to build and run:

```powershell
# from repository root
dotnet build
dotnet run --project .\dimpoc.csproj
```

How to run tests:

```powershell
dotnet test
```

What is included:
- Attributes for `TIN`, `PAN`, `Tel`, and `USNUM`.
- `AnnotationProcessor.Sanitize(object)` which sanitizes annotated string fields and returns `FieldResults`.
- `AnnotationProcessor.Validate(object)` which validates annotated fields (and may normalize phone numbers by adding leading `1` for US numbers).
- A `Program` with sample `DemoData` containing valid and invalid examples.

Notes:
- `Sanitize` will convert null strings to empty string and mark them as empty.
- `Sanitize` sets `FieldResultItem.Valid` to true when the field did not change, false otherwise.
- `Validate` performs lightweight checks (Luhn for PAN, 9 digits for TIN, US phone rules, numeric parse for USNUM).

Next steps (optional): add more international phone handling, add more robust TIN rules, and expand number formatting support.
